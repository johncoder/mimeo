using System.Collections.Generic;
using System.Text;

namespace Mimeo.Templating
{
	public interface IStencil : IList<Space>
	{

	    void GetContents(object model, StringBuilder stringBuilder);
	}
}
