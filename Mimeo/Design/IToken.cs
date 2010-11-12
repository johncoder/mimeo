using System;

namespace Mimeo.Design
{
    public interface IToken
    {
        string Identifier { get; set; }
        string GetValue(object obj);
        //IStencilDesigner<TModel> Block { get; set; }
    }

    public interface IToken<TModel>
    {
        Func<TModel, string> Resolve { get; set; }
        string GetValue(TModel model);
    }
}
