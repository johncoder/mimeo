using System.Text;

namespace Mimeo.Templating
{
    /// <summary>
    /// A simple space which appends plain text to the string builder.
    /// </summary>
    public class Positive : Space
    {
        private readonly string _value;

        /// <summary>
        /// The value to be appended to the string builder.
        /// </summary>
        public string Value { get { return _value; } }

        /// <summary>
        /// Initializes the positive space.
        /// </summary>
        /// <param name="value"></param>
        public Positive(string value)
        {
            _value = value;
        }

        /// <summary>
        /// Appends its Value property to the string bulder.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="stringBuilder"></param>
        public override void GetContents(object model, StringBuilder stringBuilder)
        {
            stringBuilder.Append(_value);
        }

        /// <summary>
        /// Determines whether or not the space can handle the object.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override bool CanHandle(object model)
        {
            return true;
        }

        /// <summary>
        /// Determines whether or not the space should handle the object.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override bool ShouldHandle(object model)
        {
            return true;
        }
    }
}
