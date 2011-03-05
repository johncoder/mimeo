using System;
using System.Collections.Generic;
using Mimeo.Templating;

namespace Mimeo.Design
{
    public class BlockToken<TModel, TChild> : Token<TModel>, IBlockToken<TModel, TChild>
    {
        public Func<TModel, IEnumerable<object>> Items { get; set; }
        public Func<TModel, TChild> Child { get; set; }

        public override object GetChild(object obj)
        {
            if (Child == null) return null;

            return Child((TModel)obj);
        }

        public BlockToken()
        {
            Items = m => new List<object>();
        }

        public override Space CreateSpace()
        {
            return CreateSpace(new List<Space>());
        }

        public override Space CreateSpace(IEnumerable<Space> spaces)
        {
            return new ComplexNegative<TModel, TChild>(this, spaces);
        }
    }
}