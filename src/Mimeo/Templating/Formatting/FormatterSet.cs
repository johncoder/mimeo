using System;
using System.Collections.Generic;
using System.Linq;

namespace Mimeo.Templating.Formatting
{
    public class FormatterSet : Dictionary<Type, IValueFormatter>
    {
        public void Add(Type type, IValueFormatter formatter)
        {
            if (ContainsKey(type))
            {
                this[type] = formatter;
                return;
            }

            base.Add(type, formatter);
        }

        public void Add<TModel>(Func<TModel, string> format)
        {
            Add(typeof(TModel), new DelegateFormatter<TModel>(format));
        }

        public void Add<TModel, TFormatter>() where TFormatter : IValueFormatter, new()
        {
            Add(typeof(TModel), new TFormatter());
        }

        public IValueFormatter Resolve<TModel>(SkipFormatterSet skip = null)
        {
            return Resolve(typeof(TModel), skip);
        }

        public IValueFormatter Resolve(Type type, SkipFormatterSet skip = null)
        {
            if (ContainsKey(type))
            {
                if (skip != null && skip.Any() && skip.Contains(type))
                    return null;

                return this[type];
            }

            foreach (var pair in this)
            {
                if (pair.Key.IsAssignableFrom(type))
                {
                    if (skip != null && skip.Any() && skip.Contains(pair.Key))
                        continue;

                    return pair.Value;
                }
            }

            return null;
        }
    }
}
