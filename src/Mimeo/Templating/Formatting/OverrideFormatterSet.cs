using System.Collections.Generic;
using System;

namespace Mimeo.Templating.Formatting
{
    /// <summary>
    /// Contains a group of con
    /// </summary>
    public class OverrideFormatterSet : List<Type>
    {
        /// <summary>
        /// The preferred formatter. Is null when it is unspecified, or when no formatter is desired.
        /// </summary>
        public IValueFormatter Formatter { get; protected set; }

        /// <summary>
        /// Adds a formatter type to be excluded when selecting a formatter.
        /// </summary>
        /// <typeparam name="TFormatter"></typeparam>
        public void SkipFormatter<TFormatter>()
        {
            Add(typeof(TFormatter));
        }

        /// <summary>
        /// Specifies the formatter to be used.
        /// </summary>
        /// <typeparam name="TFormatter"></typeparam>
        public void UseFormatter<TFormatter>() where TFormatter : IValueFormatter, new()
        {
            UseFormatter(new TFormatter());
        }

        /// <summary>
        /// Specifies the formatter to be used.
        /// </summary>
        /// <typeparam name="TFormatter"></typeparam>
        /// <param name="formatter"></param>
        public void UseFormatter<TFormatter>(TFormatter formatter) where TFormatter : IValueFormatter
        {
            Formatter = formatter;
        }
    }
}