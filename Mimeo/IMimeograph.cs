using Mimeo.Design.Syntax;
using Mimeo.Parsing;
using Mimeo.Templating;

namespace Mimeo
{
    public interface IMimeograph<TModel>
    {
        IInputParser Parser { get; set; }
        ITokenRoot<TModel> Builder { get; set; }
        IStencil CreateStencil(string name, string template);
        string Render(string name, TModel model);
    }
}