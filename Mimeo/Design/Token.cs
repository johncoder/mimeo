using System;

namespace Mimeo.Design
{
	public class Token<TModel> : IToken<TModel>
    {
		public string Identifier { get; set; }
        public Func<TModel, string> Resolve { get; set; }

        public Token()
        {
        }

        public string GetValue(object obj)
        {
            return GetValue((TModel)obj);
        }

        public string GetValue(TModel model)
        {
            if (Resolve == null)
                throw new NullReferenceException("Resolve delegate not yet set.");

            return Resolve(model);
        }
	}
}
