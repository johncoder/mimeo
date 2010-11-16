using System;
using System.Collections.Generic;
using System.Text;

namespace Mimeo.Design
{
    public class BlockToken<TModel, TChild> : Token<TModel>, IToken<TModel, TChild>
    {
        public Func<TModel, IEnumerable<TChild>> Items { get; set; }

        //public override string GetValue(TModel model)
        //{
        //    var sb = new StringBuilder();

        //    foreach (var item in Items(model))
        //        foreach (var child in Children)
        //            sb.Append(child.GetValue(item));

        //    return sb.ToString();
        //}
    }
}