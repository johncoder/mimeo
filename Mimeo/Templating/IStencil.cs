using System.Collections.Generic;

namespace Mimeo.Templating
{
	public interface IStencil<TModel>
	{
		IEnumerable<Space> Spaces { get; }
	}
}
