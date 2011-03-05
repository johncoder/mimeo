using Mimeo.Templating;

namespace Mimeo.Design
{
    public class InterpolationToken<TModel> : Token<TModel>
    {
        public override Space CreateSpace()
        {
            var space = new InterpolatingSpace();
            space.Initialize(Interpolation.ArgumentInput, Interpolation.ArgumentPattern, Interpolation.CreateValueDelegate<dynamic>());

            return space;
        }
    }
}