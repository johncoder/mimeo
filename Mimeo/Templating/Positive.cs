using System.Text;

namespace Mimeo.Templating
{
	public class Positive<TModel> : Space<TModel>
	{
		private readonly string _chunk;
		public string Chunk { get { return _chunk; } }

		public override void GetContents(TModel model, StringBuilder stringBuilder)
		{
			stringBuilder.Append(_chunk);
		}

		public Positive(string chunk)
		{
			_chunk = chunk;
		}
	}
}
