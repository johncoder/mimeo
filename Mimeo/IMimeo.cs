using Mimeo.Design.Syntax;
using Mimeo.Parsing;
using Mimeo.Templating;

namespace Mimeo
{
    public interface IMimeo<TModel>
    {
        IInputParser Parser { get; set; }
        ITokenRoot<TModel> Builder { get; set; }
        IStencil CreateStencil(string name, string template);
        string Render(string name, TModel model);
    }
}