using System.Text;
using Mimeo.Design;

namespace Mimeo.Templating
{
	public class SimpleNegative : Space
	{
	    private readonly IToken _token;

		public override void GetContents(object model, StringBuilder stringBuilder)
		{
		    var token = _token;

		    do
		    {
                if (!token.CanHandle(model))
                    token = token.Parent;

		    } while (token != null && !token.CanHandle(model));

            if (token != null && token.CanHandle(model))
		        stringBuilder.Append(token.GetValue(model));
		}

	    public SimpleNegative(IToken token)
	    {
	        _token = token;
	    }
	}
}
