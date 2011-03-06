using Mimeo.Design;
using Mimeo.Templating;

namespace Mimeo.Parsing
{
    /// <summary>
    /// Defines an interface for input parsers.
    /// </summary>
    public interface IInputParser
    {
        /// <summary>
        /// Parses a template, and uses a token graph to create a stencil.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        IStencil Parse(IToken token, string template);
    }
}