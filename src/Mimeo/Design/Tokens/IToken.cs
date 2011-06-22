using System;
using System.Collections.Generic;
using Mimeo.Templating;

namespace Mimeo.Design.Tokens
{
    /// <summary>
    /// A token.
    /// </summary>
    public interface IToken
    {
        /// <summary>
        /// A string that represents a token to be replaced.
        /// </summary>
        string Identifier { get; set; }

        /// <summary>
        /// A string that represents a terminating token.
        /// </summary>
        string Terminator { get; set; }

        /// <summary>
        /// Evaluates the object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        string Evaluate(object obj);

        /// <summary>
        /// Decides whether or not the token can handle the object in question. If not, it delegates the work to a parent token.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool CanHandle(object obj);

        /// <summary>
        /// Decides whether or not the token should handle the object in question. If not, it delegates the work to a parent token.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool ShouldHandle(object obj);

        /// <summary>
        /// A collection of children tokens. This represents a hierarchy of allowable tokens in the template.
        /// </summary>
        ICollection<IToken> Children { get; set; }

        /// <summary>
        /// The parent token.
        /// </summary>
        IToken Parent { get; set; }

        /// <summary>
        /// Overrides the parent token.
        /// </summary>
        /// <param name="token"></param>
        void SetParent(IToken token);

        /// <summary>
        /// Adds a child token.
        /// </summary>
        /// <param name="token"></param>
        void AddChild(IToken token);

        /// <summary>
        /// Creates a space used in rendering a templated object.
        /// </summary>
        /// <returns></returns>
        Space CreateSpace();

        /// <summary>
        /// Creates a space used inrendering a templated object. This space will have a collection of spaces to render.
        /// </summary>
        /// <param name="spaces"></param>
        /// <returns></returns>
        Space CreateSpace(IEnumerable<Space> spaces);

        /// <summary>
        /// Gets a child.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        object GetChild(object obj);

        /// <summary>
        /// The data used to interpolate values at render time.
        /// </summary>
        InterpolationData Interpolation { get; set; }
    }

    /// <summary>
    /// A strongly typed token.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IToken<TModel> : IToken
    {
        /// <summary>
        /// A function that resolves a value to replace the identifier token.
        /// </summary>
        Func<TModel, string> Resolve { get; set; }

        /// <summary>
        /// A condition that determines whether or not the replacement should take place.
        /// </summary>
        Func<TModel, bool> Condition { get; set; }

        /// <summary>
        /// Gets the value from the object using the Resolve method.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string GetValue(TModel model);
    }

    /// <summary>
    /// A token that contains a block of tokens.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IBlockToken<TModel> : IToken<TModel>
    {
        /// <summary>
        /// A collection on the templated object.
        /// </summary>
        Func<TModel, IEnumerable<object>> Items { get; set; }
    }

    /// <summary>
    /// A strongly typed block token that defines the child type.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TChild"></typeparam>
    public interface IBlockToken<TModel, out TChild> : IBlockToken<TModel>
    {
    }
}
