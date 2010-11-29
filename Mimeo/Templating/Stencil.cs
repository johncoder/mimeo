using System.Collections.Generic;
using System.Text;

namespace Mimeo.Templating
{
	public class Stencil : List<Space>, IStencil
	{
        public void GetContents(object model, StringBuilder stringBuilder)
        {
            foreach (var space in this)
            {
                if (space.CanHandle(model))
                    space.GetContents(model, stringBuilder);
            }
        }
    }
}
