using System;
using System.Collections.Generic;
using Mimeo.Design.Syntax;
using Mimeo.Internal;

namespace Mimeo.Design
{
    public class TokenBuilder<TModel> : ITokenRoot<TModel>, ISimpleToken<TModel>
    {
        public IToken Token { get; protected set; }
        private Token<TModel> _currentToken;

        public TokenBuilder() : this(new Token<TModel>()) { }

        public TokenBuilder(IToken<TModel> token)
        {
            Token = token;
            if (token.Resolve == null)
                token.Resolve = p => string.Empty;
        }

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

        public ITokenBlock<TModel, TChild> Block<TChild>(Func<TModel, IEnumerable<TChild>> children, string identifier, Action<ITokenRoot<TChild>> context)
        {
            Ensure.ArgumentNotNull(children, "children");
            Ensure.ArgumentNotNullOrEmpty(identifier, "identifier");
            Ensure.ArgumentNotNull(context, "context");

            var builder = new TokenBuilder<TChild>();
            context(builder);
            return Block(children, identifier, builder);
        }

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

        private IEnumerable<object> GetEachChild<TChild>(TModel model, Func<TModel, IEnumerable<TChild>> children)
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

        //public ITokenBlock<TModel, TChild> Tokenize<TChild>(Func<TModel, IEnumerable<TChild>> children, string identifier)
        //{
        //    Ensure.ArgumentNotNull(children, "children");
        //    Ensure.ArgumentNotNullOrEmpty(identifier, "identifier");

        //    _currentToken = new BlockToken<TModel, TChild>
        //    {
        //        Resolve = p => string.Empty,
        //        Identifier = identifier,
        //        Items = item => GetEachChild(item, children)
        //    };
        //    Token.Children.Add(_currentToken);

        //    return new TokenBlockBuilder<TModel, TChild>(_currentToken);
        //}

        public ISimpleToken<TModel> Encode(bool shouldEncode)
        {
            // come back to this to add filters
            // consider adding filters collection to Token
            // and during GetValue, run the result through the collection of filters
            return this;
        }
    }
}
