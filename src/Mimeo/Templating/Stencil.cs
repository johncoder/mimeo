using System.Collections.Generic;
using System.Text;

namespace Mimeo.Templating
{
    /// <summary>
    /// Renders an object by appending the contents of each space to a string builder.
    /// </summary>
	public class Stencil : List<Space>, IStencil
	{
        /// <summary>
        /// Renders an object by appending values of spaces to a string builder.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="stringBuilder"></param>
        public void Render(object model, StringBuilder stringBuilder)
        {
            foreach (var space in this)
            {
                if (space.CanHandle(model))
                    space.GetContents(model, stringBuilder);
            }
        }
    }
}
