using System;
using System.Collections.Generic;

namespace Mimeo.Design.Syntax
{
    // Idea:  Include an Ignore token.  BlockToken... _builder.Ignore("{Ignore}").To("{/Ignore}");
    // Idea:  Add a Condition overload for each token type Func<TModel, bool> condition

    /// <summary>
    /// The root of building a token graph, used in parsing and rendering.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface ITokenRoot<TModel>
    {
        /// <summary>
        /// Gets the current token
        /// </summary>
        IToken Token { get; }

        /// <summary>
        /// Describes a token which contains an argument to be extracted at render time.
        /// </summary>
        /// <param name="start">The beginning characters of the token, leading up to the argument value.</param>
        /// <param name="argumentPattern">A regular expression describing the allowable value of the argument.</param>
        /// <param name="end">The ending characters of the token, starting after the argument pattern.</param>
        /// <param name="inject">The extracted argument is passed to this delegate.</param>
        /// <returns></returns>
        ISimpleToken<TModel> Interpolate(string start, string argumentPattern, string end, Func<dynamic, string> inject);

        /// <summary>
        /// Describes a token which contains an argument to be extracted at render time.
        /// </summary>
        /// <param name="interpolationData">An object containing all interpolation data.</param>
        /// <returns></returns>
        ISimpleToken<TModel> Interpolate(InterpolationData interpolationData);

        /// <summary>
        /// Describes a simple property to token replacement.
        /// </summary>
        /// <param name="replacement">A function that receives the templated object, and the return value replaces the token identifier.</param>
        /// <param name="identifier">A token to replace.</param>
        /// <returns></returns>
        ISimpleToken<TModel> Tokenize(Func<TModel, string> replacement, string identifier);

        /// <summary>
        /// Describes a conditional token, to be replaced if and only if the defined condition is satisfied.
        /// </summary>
        /// <typeparam name="TChild">A return type used in a member of the templated object.</typeparam>
        /// <param name="replacement">A function that receives the templated object, and the return value replaces the token identifier.</param>
        /// <param name="identifier">A token to replace.</param>
        /// <param name="condition">A condition that must be met for this token to be replaced. Not satisfying this condtion results in replacing the token with an empty string.</param>
        /// <param name="context">Build another expression for mapping child tokens to be replaced in this token block.</param>
        /// <returns></returns>
        IConditionalToken<TModel, TChild> TokenizeIf<TChild>(Func<TModel, TChild> replacement, string identifier,
                                                           Func<TModel, bool> condition, Action<ITokenRoot<TChild>> context);

        /// <summary>
        /// Describes a conditional token, to be replaced if and only if the defined condition is satisfied.
        /// </summary>
        /// <typeparam name="TChild">A return type used in a member of the templated object.</typeparam>
        /// <param name="replacement">A function that receives the templated object, and the return value replaces the token identifier.</param>
        /// <param name="identifier">A token to replace.</param>
        /// <param name="condition">A condition that must be met for this token to be replaced. Not satisfying this condtion results in replacing the token with an empty string.</param>
        /// <param name="builder">A token root containing tokens to be replaced within this token block.</param>
        /// <returns></returns>
        IConditionalToken<TModel, TChild> TokenizeIf<TChild>(Func<TModel, TChild> replacement, string identifier,
                                                           Func<TModel, bool> condition, ITokenRoot<TChild> builder);

        /// <summary>
        /// Describes a block token, which is replaced if the resulting children are not null and contain items.
        /// </summary>
        /// <typeparam name="TChild">A return type used in a member of the templated object.</typeparam>
        /// <param name="children">A function that retrieves an enumeration of items from the templated object.</param>
        /// <param name="identifier">A string representing the opening token of this block.</param>
        /// <param name="context">Build another expression for mapping child tokens to be replaced in this token block.</param>
        /// <returns></returns>
        ITokenBlock<TModel, TChild> Block<TChild>(Func<TModel, IEnumerable<TChild>> children, string identifier, Action<ITokenRoot<TChild>> context);

        /// <summary>
        /// Describes a block token, which is replaced if the resulting children are not null and contain items.
        /// </summary>
        /// <typeparam name="TChild">A return type used in a member of the templated object.</typeparam>
        /// <param name="children">A function that retrieves an enumeration of items from the templated object.</param>
        /// <param name="identifier">A string representing the opening token of this block.</param>
        /// <param name="builder">A token root containing tokens to be replaced within this token block.</param>
        /// <returns></returns>
        ITokenBlock<TModel, TChild> Block<TChild>(Func<TModel, IEnumerable<TChild>> children, string identifier, ITokenRoot<TChild> builder);
    }

    /// <summary>
    /// A token which represents a single simple replacement.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface ISimpleToken<TModel>
    {
        /// <summary>
        /// Feature not implemented yet. Has no affect on returned value.
        /// </summary>
        /// <param name="shouldEncode"></param>
        /// <returns></returns>
        ISimpleToken<TModel> Encode(bool shouldEncode);
    }

    /// <summary>
    /// A token that is replaced only by satisfying a condition.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TChild"></typeparam>
    public interface IConditionalToken<TModel, TChild> : ITokenBlock<TModel, TChild>
    {
        
    }

    /// <summary>
    /// The end of a token that represents a token block.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TChild"></typeparam>
    public interface ITokenBlock<TModel, TChild>
    {
        /// <summary>
        /// Completes the block by defining a terminating identifier.
        /// </summary>
        /// <param name="terminator">A string representing the ending or closing of a token block.</param>
        /// <returns></returns>
        ITokenBlock<TModel, TChild> EndsWith(string terminator);
    }
}
