using System;
using System.Collections.Generic;

namespace Mimeo.Design
{
    public class BlockToken<TModel, TChild> : Token<TModel>, IToken<TModel, TChild>
    {
        public Func<TModel, IEnumerable<TChild>> Items { get; set; }
    }
}