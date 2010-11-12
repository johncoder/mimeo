using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mimeo.Design.Syntax
{
    public interface ITokenRoot<TModel> : ITokenSyntax
    {
        ISimpleToken<TModel> Tokenize(Func<TModel, string> replacement, string identifier);
        ITokenBegin<TModel, TChild> Tokenize<TChild>(Func<TModel, IEnumerable<TChild>> children, string identifier);
    }

    public interface ISimpleToken<TModel> : ITokenSyntax
    {
        ISimpleToken<TModel> Encode(bool shouldEncode);
    }

    public interface ITokenBegin<TModel, TChild> : ITokenSyntax
    {
        ITokenBlock<TModel, TChild> AsBlock(Action<ITokenRoot<TChild>> context);
        ITokenBlock<TModel, TChild> AsBlock(ITokenRoot<TChild> context);
    }

    public interface ITokenBlock<TModel, TChild> : ITokenSyntax
    {
        ITokenBlock<TModel, TChild> EndsWith(string terminator);
    }
}
