using System;
using Mimeo.Internal;

namespace Mimeo.Templating.Formatting
{
    /// <summary>
    /// A simple formatter whose replacement is based on a delegate.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class DelegateFormatter<TModel> : IValueFormatter
    {
        private readonly Func<TModel, string> _format;

        /// <summary>
        /// Iniitializes a delegate formatter, using the specified formatter delegate.
        /// </summary>
        /// <param name="format"></param>
        public DelegateFormatter(Func<TModel, string> format)
        {
            Ensure.ArgumentNotNull(format, "format");

            _format = format;
        }

        /// <summary>
        /// Uses the delegate to format an incoming value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Format(object value)
        {
            if (value == null) return string.Empty;

            return _format((TModel)value);
        }
    }
}
