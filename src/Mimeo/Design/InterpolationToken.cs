using Mimeo.Templating;

namespace Mimeo.Design
{
    /// <summary>
    /// A token whose value is interpolated at render time.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class InterpolationToken<TModel> : Token<TModel>
    {
        /// <summary>
        /// Creates an interpolating space, which executes the injection delegate defined on the interpolating token.
        /// </summary>
        /// <returns></returns>
        public override Space CreateSpace()
        {
            var space = new InterpolatingSpace();
            space.Initialize(Interpolation.CreateValueDelegate<dynamic>());

            return space;
        }
    }
}