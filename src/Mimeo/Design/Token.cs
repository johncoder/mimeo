using System;
using System.Collections.Generic;
using Mimeo.Internal;
using Mimeo.Templating;

namespace Mimeo.Design
{
    /// <summary>
    /// A strongly typed token.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
	public class Token<TModel> : IToken<TModel>
    {
        /// <summary>
        /// A string representing the token to be replaced.
        /// </summary>
		public string Identifier { get; set; }

        /// <summary>
        /// A string representing the terminating token.
        /// </summary>
        public string Terminator { get; set; }

        /// <summary>
        /// A function that resolves a value from the object.
        /// </summary>
        public Func<TModel, string> Resolve { get; set; }

        /// <summary>
        /// A function that determines whether or not to render the token.
        /// </summary>
        public Func<TModel, bool> Condition { get; set; }

        /// <summary>
        /// A collection of children tokens to be processed along with the current token.
        /// </summary>
        public ICollection<IToken> Children { get; set; }

        /// <summary>
        /// The parent token.
        /// </summary>
	    public IToken Parent { get; set; }

        /// <summary>
        /// Interpolation data to be used during rendering.
        /// </summary>
        public InterpolationData Interpolation { get; set; }

        /// <summary>
        /// Initializes the token.
        /// </summary>
        public Token()
	    {
	        Children = new List<IToken>();
	    }

        /// <summary>
        /// Sets the parent of the token.
        /// </summary>
        /// <param name="token"></param>
        public void SetParent(IToken token)
        {
            Ensure.ArgumentNotNull(token, "token");

            Parent = token;
        }

        /// <summary>
        /// Adds a child token.
        /// </summary>
        /// <param name="token"></param>
        public void AddChild(IToken token)
        {
            Ensure.ArgumentNotNull(token, "token");

            token.SetParent(this);
            Children.Add(token);
        }

        /// <summary>
        /// Evaluates the object to get a value during render time.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual string Evaluate(object obj)
        {
            return GetValue((TModel)obj);
        }

        /// <summary>
        /// Gets a value from the object using the Resolve delegate.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual string GetValue(TModel model)
        {
            if (Resolve == null)
                throw new NullReferenceException("Resolve delegate not yet set.");

            return Resolve(model);
        }

        /// <summary>
        /// Gets a child of the object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual object GetChild(object obj)
        {
            return null;
        }

        /// <summary>
        /// Determines whether or not the token can handle the object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
	    public bool CanHandle(object obj)
	    {
	        return obj is TModel;
	    }

        /// <summary>
        /// Determines whether or not the token should handle the object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool ShouldHandle(object obj)
        {
            return ShouldHandle((TModel)obj);
        }

        /// <summary>
        /// Determines whether or not the token should handle the object.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool ShouldHandle(TModel model)
        {
            if (Condition == null)
                return true;

            return Condition(model);
        }

        /// <summary>
        /// Creates a space used to render the object.
        /// </summary>
        /// <returns></returns>
        public virtual Space CreateSpace()
        {
            return new SimpleNegative(this);
        }

        /// <summary>
        /// Creates a space used to render the object.
        /// </summary>
        /// <param name="spaces"></param>
        /// <returns></returns>
        public virtual Space CreateSpace(IEnumerable<Space> spaces)
        {
            return new SimpleNegative(this);
        }
    }
}
