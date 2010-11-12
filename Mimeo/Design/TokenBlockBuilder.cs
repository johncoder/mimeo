using System;
using Mimeo.Design.Syntax;

namespace Mimeo.Design
{
    public class TokenBlockBuilder<TModel, TChild> : ITokenBegin<TModel, TChild>, ITokenBlock<TModel, TChild>
    {
        private readonly ITokenRoot<TModel> _parent;

        public TokenBlockBuilder(ITokenRoot<TModel> root)
        {
            _parent = root;
        }

        public ITokenBlock<TModel, TChild> AsBlock(Action<ITokenRoot<TChild>> context)
        {
            return this;
        }

        public ITokenBlock<TModel, TChild> AsBlock(ITokenRoot<TChild> context)
        {
            return this;
        }

        public ITokenBlock<TModel, TChild> EndsWith(string terminator)
        {
            return this;
        }
    }
}
