using Mimeo.Design.Syntax;
using Mimeo.Internal;
using System;

namespace Mimeo.Design
{
    public class TokenBlockBuilder<TModel, TChild> : IConditionalToken<TModel, TChild>
    {
        private readonly IToken _token;
        public IToken Token { get { return _token; } }
        public TokenBlockBuilder(IToken token)
        {
            Ensure.ArgumentNotNull(token, "token");
            _token = token;
        }

        public ITokenBlock<TModel, TChild> EndsWith(string terminator)
        {
            Ensure.ArgumentNotNullOrEmpty(terminator, "terminator");

            _token.Terminator = terminator;
            return this;
        }

        public ITokenBlock<TModel, TChild> Interpolate(string start, string end, Func<dynamic, string> inject)
        {
            Ensure.ArgumentNotNull(inject, "inject");
            Ensure.ArgumentNotNullOrEmpty(start, "start");
            Ensure.ArgumentNotNullOrEmpty(end, "end");

            return this;
        }
    }
}
