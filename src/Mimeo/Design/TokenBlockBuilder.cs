using Mimeo.Design.Syntax;
using Mimeo.Design.Tokens;
using Mimeo.Internal;
using System;

namespace Mimeo.Design
{
    /// <summary>
    /// A token block builder.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TChild"></typeparam>
    public class TokenBlockBuilder<TModel, TChild> : IConditionalToken<TModel, TChild>
    {
        private readonly IToken _token;

        /// <summary>
        /// The token that represents the root of this token block.
        /// </summary>
        public IToken Token { get { return _token; } }

        /// <summary>
        /// Initializes teh token block builder.
        /// </summary>
        /// <param name="token"></param>
        public TokenBlockBuilder(IToken token)
        {
            Ensure.ArgumentNotNull(token, "token");
            _token = token;
        }

        /// <summary>
        /// Defines a terminator for the current token.
        /// </summary>
        /// <param name="terminator"></param>
        /// <returns></returns>
        public ITokenBlock<TModel, TChild> EndsWith(string terminator)
        {
            Ensure.ArgumentNotNullOrEmpty(terminator, "terminator");

            _token.Terminator = terminator;
            return this;
        }
    }
}
