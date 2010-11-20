using System;
using System.Text;
using System.Linq.Expressions;
using Mimeo.Design;

namespace Mimeo.Templating
{
	public class SimpleNegative<TModel> : Negative<TModel>
	{
		//private readonly Func<TModel, object> _getContents;
	    private IToken _token;

		public override void GetContents(TModel model, StringBuilder stringBuilder)
		{
			//stringBuilder.Append(_getContents(model).ToString());
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
