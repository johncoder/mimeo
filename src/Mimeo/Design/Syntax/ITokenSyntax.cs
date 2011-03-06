
namespace Mimeo.Design.Syntax
{
    /// <summary>
    /// Token syntax.
    /// </summary>
    public interface ITokenSyntax : IFluentSyntax
    {
        /// <summary>
        /// The root of the token graph.
        /// </summary>
        IToken Token { get; }
    }
}
