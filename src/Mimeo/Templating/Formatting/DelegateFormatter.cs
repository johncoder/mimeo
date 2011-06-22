using System;
using Mimeo.Internal;

namespace Mimeo.Templating.Formatting
{
    public class DelegateFormatter<TModel> : IValueFormatter
    {
        private readonly Func<TModel, string> _format;

        public DelegateFormatter(Func<TModel, string> format)
        {
            Ensure.ArgumentNotNull(format, "format");

            _format = format;
        }

        public string Format(object value)
        {
            if (value == null) return string.Empty;

            return _format((TModel)value);
        }
    }
}
