using System.Collections.Generic;
using System.Text;

namespace Mimeo.Templating
{
    /// <summary>
    /// An interface defining a stencil, whose purpose is to write contents of all spaces to the string builder.
    /// </summary>
	public interface IStencil : IList<Space>
	{
        /// <summary>
        /// Appends the contents of each space to the string builder.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="stringBuilder"></param>
	    void Render(object model, StringBuilder stringBuilder);
	}
}
