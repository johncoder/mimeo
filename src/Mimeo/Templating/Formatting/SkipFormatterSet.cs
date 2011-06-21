using System.Collections.Generic;
using System;

namespace Mimeo.Templating.Formatting
{
    public class SkipFormatterSet : List<Type>
    {
        public void SkipFormatter<TFormatter>()
        {
            Add(typeof(TFormatter));
        }
    }
}