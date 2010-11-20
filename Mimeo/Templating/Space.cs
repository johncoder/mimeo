using System.Text;

namespace Mimeo.Templating
{
	public abstract class Space
	{
		public abstract void GetContents(object model, StringBuilder stringBuilder);
	}

	public abstract class Space<TModel> : Space
	{
		public override void GetContents(object model, StringBuilder stringBuilder)
		{
			GetContents((TModel)model, stringBuilder);
		}
		public abstract void GetContents(TModel model, StringBuilder stringBuilder);
	}
}
