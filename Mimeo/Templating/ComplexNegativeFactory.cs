using System;
using System.Linq.Expressions;

namespace Mimeo.Templating
{
	public class ComplexNegativeFactory
	{
		public Negative<TModel> Create<TModel>(Type childType, Expression<Func<TModel, object>> getCollection, object spaces)
		{
			var newtype2 = getCollection.Body.Type;
			var type = typeof(ComplexNegative<,>);
			var newtype = type.MakeGenericType(typeof(TModel), childType);
			return (Negative<TModel>)Activator.CreateInstance(newtype, new[] { getCollection, spaces });
		}
	}
}
