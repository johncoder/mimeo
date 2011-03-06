using System;
using System.Collections.Generic;
using Mimeo.Design.Syntax;
using Mimeo.Internal;

namespace Mimeo.Design
{
    /// <summary>
    /// An object that builds tokens.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class TokenBuilder<TModel> : ITokenRoot<TModel>, ISimpleToken<TModel>
    {
        private Token<TModel> _currentToken;

        /// <summary>
        /// The root token.
        /// </summary>
        public IToken Token { get; protected set; }

        /// <summary>
        /// Initializes the token builder.
        /// </summary>
        public TokenBuilder() : this(new Token<TModel>()) { }

        /// <summary>
        /// Initializes the token builder with a set of children tokens.
        /// </summary>
        /// <param name="token"></param>
        public TokenBuilder(IToken<TModel> token)
        {
            Token = token;
            if (token.Resolve == null)
                token.Resolve = p => string.Empty;
        }

        /// <summary>
        /// Defines a child interpolation token.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="argumentPattern"></param>
        /// <param name="end"></param>
        /// <param name="inject"></param>
        /// <returns></returns>
        public ISimpleToken<TModel> Interpolate(string start, string argumentPattern, string end, Func<dynamic, string> inject)
        {
            Ensure.ArgumentNotNullOrEmpty(start, "start");
            Ensure.ArgumentNotNullOrEmpty(end, "end");
            Ensure.ArgumentNotNull(inject, "inject");

            var interpolationData = new InterpolationData
            {
                Start = start,
                ArgumentPattern = argumentPattern,
                End = end,
                Injection = inject
            };

            return Interpolate(interpolationData);
        }
        
        /// <summary>
        /// Defines a child interpolation token.
        /// </summary>
        /// <param name="interpolationData"></param>
        /// <returns></returns>
        public ISimpleToken<TModel> Interpolate(InterpolationData interpolationData)
        {
            Ensure.ArgumentNotNull(interpolationData, "interpolationData");

            _currentToken = new InterpolationToken<TModel>()
            {
                Interpolation = interpolationData,
                Identifier = interpolationData.GetIdentifier()
            };
            Token.AddChild(_currentToken);

            return this;
        }

        /// <summary>
        /// Defines a child token for simple replacement.
        /// </summary>
        /// <param name="replacement"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public ISimpleToken<TModel> Tokenize(Func<TModel, string> replacement, string identifier)
        {
            Ensure.ArgumentNotNull(replacement, "replacement");
            Ensure.ArgumentNotNullOrEmpty(identifier, "identifier");

            _currentToken = new Token<TModel>
            {
                Resolve = replacement,
                Identifier = string.Intern(identifier)
            };
            Token.AddChild(_currentToken);

            return this;
        }

        /// <summary>
        /// Defines a child conditional token for replacement.
        /// </summary>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="replacement"></param>
        /// <param name="identifier"></param>
        /// <param name="condition"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public IConditionalToken<TModel, TChild> TokenizeIf<TChild>(Func<TModel, TChild> replacement, string identifier, Func<TModel, bool> condition, Action<ITokenRoot<TChild>> context)
        {
            Ensure.ArgumentNotNull(replacement, "replacement");
            Ensure.ArgumentNotNullOrEmpty(identifier, "identifier");
            Ensure.ArgumentNotNull(condition, "condition");
            Ensure.ArgumentNotNull(context, "context");

            var builder = new TokenBuilder<TChild>();
            context(builder);
            return TokenizeIf(replacement, identifier, condition, builder);
        }

        /// <summary>
        /// Defines a child conditional token for replacement.
        /// </summary>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="replacement"></param>
        /// <param name="identifier"></param>
        /// <param name="condition"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public IConditionalToken<TModel, TChild> TokenizeIf<TChild>(Func<TModel, TChild> replacement, string identifier, Func<TModel, bool> condition, ITokenRoot<TChild> context)
        {
            Ensure.ArgumentNotNull(replacement, "replacement");
            Ensure.ArgumentNotNullOrEmpty(identifier, "identifier");
            Ensure.ArgumentNotNull(condition, "condition");
            Ensure.ArgumentNotNull(context, "context");

            var blockToken = new BlockToken<TModel, TChild>
            {
                Child = replacement,
                Resolve = p => string.Empty,
                Identifier = string.Intern(identifier),
                Condition = condition
            };

            _currentToken = blockToken;
            Token.Children.Add(_currentToken);

            foreach (var token in context.Token.Children)
                _currentToken.AddChild(token);

            return new TokenBlockBuilder<TModel, TChild>(_currentToken);
        }

        /// <summary>
        /// Defines a child block of tokens.
        /// </summary>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="children"></param>
        /// <param name="identifier"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public ITokenBlock<TModel, TChild> Block<TChild>(Func<TModel, IEnumerable<TChild>> children, string identifier, Action<ITokenRoot<TChild>> context)
        {
            Ensure.ArgumentNotNull(children, "children");
            Ensure.ArgumentNotNullOrEmpty(identifier, "identifier");
            Ensure.ArgumentNotNull(context, "context");

            var builder = new TokenBuilder<TChild>();
            context(builder);
            return Block(children, identifier, builder);
        }

        /// <summary>
        /// Defines a child block of tokens.
        /// </summary>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="children"></param>
        /// <param name="identifier"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public ITokenBlock<TModel, TChild> Block<TChild>(Func<TModel, IEnumerable<TChild>> children, string identifier, ITokenRoot<TChild> context)
        {
            Ensure.ArgumentNotNull(children, "children");
            Ensure.ArgumentNotNullOrEmpty(identifier, "identifier");
            Ensure.ArgumentNotNull(context, "context");

            var blockToken = new BlockToken<TModel, TChild>
            {
                Resolve = p => string.Empty,
                Identifier = string.Intern(identifier),
                Items = model => GetEachChild(model, children)
            };

            _currentToken = blockToken;
            Token.Children.Add(_currentToken);

            foreach (var token in context.Token.Children)
                _currentToken.AddChild(token);

            return new TokenBlockBuilder<TModel, TChild>(_currentToken);
        }

        private static IEnumerable<object> GetEachChild<TChild>(TModel model, Func<TModel, IEnumerable<TChild>> children)
        {
            var items = children(model);

            if (items == null)
                yield break;

            foreach (var child in children(model))
            {
                if (child != null)
                    yield return child;
                else
                    yield break;
            }
        }

        /// <summary>
        /// Defines a child token for conditional replacement.
        /// </summary>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="replacement"></param>
        /// <param name="identifier"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IConditionalToken<TModel, TChild> Tokenize<TChild>(Func<TModel, TChild> replacement, string identifier, Func<TModel, bool> condition)
        {
            Ensure.ArgumentNotNull(replacement, "replacement");
            Ensure.ArgumentNotNullOrEmpty(identifier, "identifier");
            Ensure.ArgumentNotNull(condition, "condition");

            _currentToken = new Token<TModel>
            {
                Resolve = p => string.Empty,
                Identifier = identifier,
                Condition = condition
            };
            Token.Children.Add(_currentToken);

            return new TokenBlockBuilder<TModel, TChild>(_currentToken);
        }

        /// <summary>
        /// Not implemented yet. Has no affect on output.
        /// </summary>
        /// <param name="shouldEncode"></param>
        /// <returns></returns>
        public ISimpleToken<TModel> Encode(bool shouldEncode)
        {
            // come back to this to add filters
            // consider adding filters collection to Token
            // and during GetValue, run the result through the collection of filters
            return this;
        }
    }
}
