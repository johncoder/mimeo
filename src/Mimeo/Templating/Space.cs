using System.Text;

namespace Mimeo.Templating
{
	public abstract class Space
	{
	    public abstract bool CanHandle(object model);
	    public abstract bool ShouldHandle(object model);
		public abstract void GetContents(object model, StringBuilder stringBuilder);
	    public virtual void Add(object obj) { }
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
