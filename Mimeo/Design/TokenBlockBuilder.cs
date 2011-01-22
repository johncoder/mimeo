using Mimeo.Design.Syntax;
using Mimeo.Internal;

namespace Mimeo.Design
{
    public class TokenBlockBuilder<TModel, TChild> : IConditionalToken<TModel, TChild>
    {
        private readonly IToken _token;

        public TokenBlockBuilder(IToken token)
        {
            Ensure.ArgumentNotNull(token, "token");
            _token = token;
        }

        //public ITokenBlock<TModel, TChild> AsBlock(Action<ITokenRoot<TChild>> context)
        //{
        //    Ensure.ArgumentNotNull(context, "context");

        //    var builder = new TokenBuilder<TChild>();
        //    context(builder);
        //    AsBlock(builder);

        //    return this;
        //}

        //public ITokenBlock<TModel, TChild> AsBlock(ITokenRoot<TChild> context)
        //{
        //    Ensure.ArgumentNotNull(context, "context");

        //    var blockToken = new BlockToken<TModel, TChild>();

        //    blockToken.Identifier = _token.Identifier;
        //    blockToken.Parent = _token.Parent;
        //    blockToken.Children = _token.Children;

        //    _token = blockToken;

        //    foreach(var token in context.Token.Children)
        //        _token.AddChild(token);

        //    return this;
        //}

        public ITokenBlock<TModel, TChild> EndsWith(string terminator)
        {
            Ensure.ArgumentNotNullOrEmpty(terminator, "terminator");

            _token.Terminator = terminator;
            return this;
        }
    }
}
