using System.Text;
using Mimeo.Design;

namespace Mimeo.Templating
{
    /// <summary>
    /// A space representing a simple replacement.
    /// </summary>
	public class SimpleNegative : Space
	{
	    private readonly IToken _token;

        /// <summary>
        /// Initializes the simple negative space.
        /// </summary>
        /// <param name="token"></param>
        public SimpleNegative(IToken token)
        {
            _token = token;
        }
        
        /// <summary>
        /// Gets a value from the object context, and appends it to the string builder.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="stringBuilder"></param>
        public override void GetContents(object model, StringBuilder stringBuilder)
		{
            foreach (var child in _token.Children)
            {
                if (!child.CanHandle(model))
                    continue;
                
                stringBuilder.Append(child.Evaluate(model));
                return;
            }

            var token = _token;

            do
            {
                if (!token.CanHandle(model))
                    token = token.Parent;
            } while (token != null && !token.CanHandle(model));

            if (token != null && token.CanHandle(model))
            {
                stringBuilder.Append(_token.Evaluate(model));
                return;
            }
		}

        /// <summary>
        /// Determines whether the space can handle the object.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override bool CanHandle(object model)
        {
            return _token.CanHandle(model);
        }

        /// <summary>
        /// Determines whether or not the space should handle the object.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override bool ShouldHandle(object model)
        {
            return _token.ShouldHandle(model);
        }
    }
}
