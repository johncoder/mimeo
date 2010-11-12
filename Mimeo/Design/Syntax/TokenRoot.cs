using System;
using System.Collections.Generic;

namespace Mimeo.Design.Syntax
{
    public class TokenRoot<TModel> : ITokenRoot<TModel>
    {
        public ISimpleToken<TModel> Tokenize(Func<TModel, string> replacement, string identifier)
        {
            return new SimpleToken<TModel>();
        }

        public ITokenBegin<TModel, TChild> Tokenize<TChild>(Func<TModel, IEnumerable<TChild>> children, string identifier)
        {
            throw new NotImplementedException();
        }
    }
}
