using System;
using System.Text;

namespace Mimeo.Templating
{
    /// <summary>
    /// A space whose value is injected based on interpolated arguments.
    /// </summary>
    public class InterpolatingSpace : Space
    {
        private Func<object, string> _inject;

        /// <summary>
        /// Initializes the space
        /// </summary>
        /// <param name="inject"></param>
        public void Initialize(Func<dynamic, string> inject)
        {
            _inject = inject;
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

        /// <summary>
        /// Uses the model to inject a value into the string builder.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="stringBuilder"></param>
        public override void GetContents(object model, StringBuilder stringBuilder)
        {
            stringBuilder.Append(_inject(model));
        }
    }
}
