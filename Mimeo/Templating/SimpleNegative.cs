using System;
using System.Text;
using Mimeo.Design;

namespace Mimeo.Templating
{
	public class SimpleNegative : Space
	{
	    private readonly IToken _token;

        public SimpleNegative(IToken token)
        {
            _token = token;
        }
        
        public override void GetContents(object model, StringBuilder stringBuilder)
		{
            //var token = _token;

            //do
            //{
            //    if (!token.CanHandle(model))
            //        token = token.Parent;

            //} while (token != null && !token.CanHandle(model));

            //if (token != null && token.CanHandle(model))
            //    stringBuilder.Append(token.GetValue(model));

            foreach (var child in _token.Children)
            {
                if (!child.CanHandle(model))
                    continue;
                
                stringBuilder.Append(child.Evaluate(model));
                return;
            }

            var token = _token;

            do
            {
                if (!token.CanHandle(model))
                    token = token.Parent;
            } while (token != null && !token.CanHandle(model));

            if (token != null && token.CanHandle(model))
            {
                stringBuilder.Append(_token.Evaluate(model));
                return;
            }
		}

        public override bool CanHandle(object model)
        {
            return _token.CanHandle(model);
        }

        public override bool ShouldHandle(object model)
        {
            return _token.ShouldHandle(model);
        }
    }
}
