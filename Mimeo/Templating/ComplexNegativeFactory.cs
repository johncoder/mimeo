using System;
using System.Linq.Expressions;

namespace Mimeo.Templating
{
    public class ComplexNegativeFactory
    {
        public Space<TModel> Create<TModel>(Type childType, Func<TModel> getCollection, object spaces)
        {
            var type = typeof(ComplexNegative<,>);
            var newtype = type.MakeGenericType(typeof(TModel), childType);
            return (Space<TModel>)Activator.CreateInstance(newtype, new[] { getCollection, spaces });
        }
    }
}
