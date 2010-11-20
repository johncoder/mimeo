using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace Mimeo.Templating
{
	public class ComplexNegative<TModel, TChild> : Negative<TModel>
	{
		Func<TModel, object> _getProperty;
		IEnumerable<Space> _body;

		public IEnumerable<Space> Spaces { get { return _body; } }

		public ComplexNegative(Expression<Func<TModel, object>> property, IEnumerable<Space> body)
		{
			_body = body;
			_getProperty = property.Compile();
		}

		public override void GetContents(TModel model, StringBuilder stringBuilder)
		{
			var result = _getProperty(model);

			GetContents(stringBuilder, result as IEnumerable<object>);
		}

		private void GetContents(StringBuilder stringBuilder, IEnumerable<object> items)
		{
			foreach (var item in items)
			{
				foreach (var space in _body)
				{
					space.GetContents(item, stringBuilder);
				}
			}
		}
	}
}
