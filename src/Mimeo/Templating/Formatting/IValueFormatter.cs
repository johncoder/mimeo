namespace Mimeo.Templating.Formatting
{
    /// <summary>
    /// An interface defining an ability to format an object into a string.
    /// </summary>
    public interface IValueFormatter
    {
        /// <summary>
        /// Formats an incoming value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string Format(object value);
    }
}
