using System;
using System.Collections.Generic;

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

        public IValueFormatter Resolve<TModel>()
        {
            return Resolve(typeof(TModel));
        }

        public IValueFormatter Resolve(Type type)
        {
            if (ContainsKey(type))
                return this[type];

            foreach (var pair in this)
            {
                if (pair.Key.IsAssignableFrom(type))
                    return pair.Value;
            }

            return null;
        }
    }
}
