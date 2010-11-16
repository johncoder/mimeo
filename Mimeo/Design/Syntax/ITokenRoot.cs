using System;
using System.Collections.Generic;

namespace Mimeo.Design.Syntax
{
    public interface ITokenRoot<TModel> : ITokenSyntax
    {
        IToken Token { get; }
        ISimpleToken<TModel> Tokenize(Func<TModel, string> replacement, string identifier);
        IConditionalToken<TModel, TChild> Tokenize<TChild>(Func<TModel, TChild> replacement, string identifier,
                                                           Func<TModel, bool> condition);
        ITokenBegin<TModel, TChild> Tokenize<TChild>(Func<TModel, IEnumerable<TChild>> children, string identifier);
    }

    public interface ISimpleToken<TModel> : ITokenSyntax
    {
        ISimpleToken<TModel> Encode(bool shouldEncode);
    }

    public interface IConditionalToken<TModel, TChild> : ITokenBegin<TModel, TChild>
    {
        
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
