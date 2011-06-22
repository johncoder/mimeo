using System;
using System.Collections.Generic;
using System.Linq;

namespace Mimeo.Templating.Formatting
{
    /// <summary>
    /// A collection of formatters to be used at token configuration time. Each formatter is a
    /// candidate, based on its type, to be used in a closure to wrap token value replacement.
    /// </summary>
    public class FormatterSet : Dictionary<Type, IValueFormatter>
    {
        /// <summary>
        /// Adds a formatter for a given type. Does not throw if the key already exists (the exiting formatter will be overridden with the new one).
        /// </summary>
        /// <param name="type"></param>
        /// <param name="formatter"></param>
        public new void Add(Type type, IValueFormatter formatter)
        {
            if (ContainsKey(type))
            {
                this[type] = formatter;
                return;
            }

            base.Add(type, formatter);
        }

        /// <summary>
        /// Initializes a formatter from the provided delegate, and adds it for the generic type parameter.
        /// </summary>
        /// <typeparam name="TModel">The type for which the formatter should be added.</typeparam>
        /// <param name="format"></param>
        public void Add<TModel>(Func<TModel, string> format)
        {
            Add(typeof(TModel), new DelegateFormatter<TModel>(format));
        }

        /// <summary>
        /// Initializes and adds a formatter for a given type.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TFormatter"></typeparam>
        public void Add<TModel, TFormatter>() where TFormatter : IValueFormatter, new()
        {
            Add(typeof(TModel), new TFormatter());
        }

        /// <summary>
        /// Resolves a formatter for the specified type. The state of the provided OverrideFormatterSet
        /// has a primary affect on the chosen formatter.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="formatters"></param>
        /// <returns></returns>
        public IValueFormatter Resolve<TModel>(OverrideFormatterSet formatters = null)
        {
            return Resolve(typeof(TModel), formatters);
        }

        /// <summary>
        /// Resolves a formatter for the specified type. The state of the provided OverrideFormatterSet
        /// has a primary affect on the chosen formatter.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="formatters"></param>
        /// <returns></returns>
        public IValueFormatter Resolve(Type type, OverrideFormatterSet formatters = null)
        {
            if (formatters != null && formatters.Formatter != null)
                return formatters.Formatter;

            if (ContainsKey(type))
            {
                if (formatters != null && formatters.Any() && formatters.Contains(type))
                    return null;

                return this[type];
            }

            foreach (var pair in this)
            {
                if (pair.Key.IsAssignableFrom(type))
                {
                    if (formatters != null && formatters.Any() && formatters.Contains(pair.Key))
                        continue;

                    return pair.Value;
                }
            }

            return null;
        }
    }
}
