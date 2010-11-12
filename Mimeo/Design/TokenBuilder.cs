using System;
using System.Collections.Generic;
using Mimeo.Design.Syntax;
using Mimeo.Internal;

namespace Mimeo.Design
{
    public class TokenBuilder<TModel> : ITokenRoot<TModel>, ISimpleToken<TModel>
    {
        private readonly ICollection<Token<TModel>> _tokens;
        private Token<TModel> _currentToken;

        public TokenBuilder()
        {
            _tokens = new List<Token<TModel>>();
        }

        public ISimpleToken<TModel> Tokenize(Func<TModel, string> replacement, string identifier)
        {
            Ensure.ArgumentNotNull(replacement, "replacement");
            Ensure.ArgumentNotNullOrEmpty(identifier, "identifier");

            var token = new Token<TModel>
            {
                Resolve = replacement,
                Identifier = string.Intern(identifier)
            };

            _currentToken = token;
            _tokens.Add(token);

            return this;
        }

        public ITokenBegin<TModel, TChild> Tokenize<TChild>(Func<TModel, IEnumerable<TChild>> children, string identifier)
        {
            Ensure.ArgumentNotNull(children, "children");
            Ensure.ArgumentNotNullOrEmpty(identifier, "identifier");

            return new TokenBlockBuilder<TModel, TChild>(this);
        }

        public ISimpleToken<TModel> Encode(bool shouldEncode)
        {
            return this;
        }
    }
}
