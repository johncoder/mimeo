using System;
using System.Collections.Generic;
using Mimeo.Internal;
using Mimeo.Templating;

namespace Mimeo.Design
{
	public class Token<TModel> : IToken<TModel>
    {
		public string Identifier { get; set; }
        public string Terminator { get; set; }
        public Func<TModel, string> Resolve { get; set; }
        public Func<TModel, bool> Condition { get; set; }
        public ICollection<IToken> Children { get; set; }
	    public IToken Parent { get; set; }

	    public Token()
	    {
	        Children = new List<IToken>();
	    }

        public void SetParent(IToken token)
        {
            Ensure.ArgumentNotNull(token, "token");

            Parent = token;
        }

        public void AddChild(IToken token)
        {
            Ensure.ArgumentNotNull(token, "token");

            token.SetParent(this);
            Children.Add(token);
        }

        public virtual string GetValue(object obj)
        {
            return GetValue((TModel)obj);
        }

        public virtual string GetValue(TModel model)
        {
            if (Resolve == null)
                throw new NullReferenceException("Resolve delegate not yet set.");

            return Resolve(model);
        }

        public virtual object GetChild(object obj)
        {
            return null;
        }

	    public bool CanHandle(object obj)
	    {
	        return obj is TModel;
	    }

        public bool ShouldHandle(object obj)
        {
            return ShouldHandle((TModel)obj);
        }

        public bool ShouldHandle(TModel model)
        {
            if (Condition == null)
                return true;

            return Condition(model);
        }

        public virtual Space CreateSpace()
        {
            return new SimpleNegative(this);
        }

        public virtual Space CreateSpace(IEnumerable<Space> spaces)
        {
            return new SimpleNegative(this);
        }
    }
}
