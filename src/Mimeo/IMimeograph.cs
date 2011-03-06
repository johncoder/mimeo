using Mimeo.Design.Syntax;
using Mimeo.Parsing;
using Mimeo.Templating;

namespace Mimeo
{
    /// <summary>
    /// An interface that defines a mimeograph object.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IMimeograph<TModel>
    {
        /// <summary>
        /// The object used to parse a template into a stencil.
        /// </summary>
        IInputParser Parser { get; set; }

        /// <summary>
        /// The root token used to parse and render a template.
        /// </summary>
        ITokenRoot<TModel> Builder { get; set; }

        /// <summary>
        /// Creates a stencil using the input parser, stored by the name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        IStencil CreateStencil(string name, string template);

        /// <summary>
        /// Renders an object using a specified stencil by name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        string Render(string name, TModel model);
    }
}