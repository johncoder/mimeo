using System;
using System.Collections.Generic;
using Mimeo.Templating;

namespace Mimeo.Design
{
    public class BlockToken<TModel> : Token<TModel>, IBlockToken<TModel>
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