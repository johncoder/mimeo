using System;
using System.Collections.Generic;
using Mimeo.Templating;
using System.Collections;

namespace Mimeo.Design
{
    public class BlockToken<TModel, TChild> : Token<TModel>, IBlockToken<TModel, TChild>
    {
        public Func<TModel, IEnumerable<object>> Items { get; set; }

        public override Space CreateSpace()
        {
            return new ComplexNegative<TModel, TChild>(this, new List<Space>());
        }

        public override Space CreateSpace(IEnumerable<Space> spaces)
        {
            return new ComplexNegative<TModel, TChild>(this, spaces);
        }
    }
}