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

        public TokenBuilder()
        {
            Token = new Token<TModel>();
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

        public ITokenBegin<TModel, TChild> Tokenize<TChild>(Func<TModel, IEnumerable<TChild>> children, string identifier)
        {
            Ensure.ArgumentNotNull(children, "children");
            Ensure.ArgumentNotNullOrEmpty(identifier, "identifier");

            _currentToken = new BlockToken<TModel, TChild>
            {
                Resolve = p => string.Empty,
                Identifier = identifier,
                Items = item => children as IEnumerable<object>
            };
            Token.Children.Add(_currentToken);

            return new TokenBlockBuilder<TModel, TChild>(_currentToken);
        }

        public ISimpleToken<TModel> Encode(bool shouldEncode)
        {
            // come back to this to add filters
            // consider adding filters collection to Token
            // and during GetValue, run the result through the collection of filters
            return this;
        }
    }
}
