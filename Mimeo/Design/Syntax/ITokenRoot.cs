using System;
using System.Collections.Generic;

namespace Mimeo.Design.Syntax
{
    // Idea:  Include an Ignore token.  BlockToken... _builder.Ignore("{Ignore}").To("{/Ignore}");
    // Idea:  Add a Condition overload for each token type Func<TModel, bool> condition

    public interface ITokenRoot<TModel> : ITokenSyntax
    {
        IToken Token { get; }
        ISimpleToken<TModel> Tokenize(Func<TModel, string> replacement, string identifier);

        IConditionalToken<TModel, TChild> Block<TChild>(Func<TModel, TChild> replacement, string identifier,
                                                           Func<TModel, bool> condition, Action<ITokenRoot<TChild>> context);
        IConditionalToken<TModel, TChild> Block<TChild>(Func<TModel, TChild> replacement, string identifier,
                                                           Func<TModel, bool> condition, ITokenRoot<TChild> builder);

        ITokenBlock<TModel, TChild> Block<TChild>(Func<TModel, IEnumerable<TChild>> children, string identifier, Action<ITokenRoot<TChild>> context);
        ITokenBlock<TModel, TChild> Block<TChild>(Func<TModel, IEnumerable<TChild>> children, string identifier, ITokenRoot<TChild> builder);
    }

    public interface ISimpleToken<TModel> : ITokenSyntax
    {
        ISimpleToken<TModel> Encode(bool shouldEncode);
    }

    public interface IConditionalToken<TModel, TChild> : ITokenBlock<TModel, TChild>
    {
        
    }

    //public interface ITokenBegin<TModel, TChild> : ITokenSyntax
    //{
    //    ITokenBlock<TModel, TChild> AsBlock(Action<ITokenRoot<TChild>> context);
    //    ITokenBlock<TModel, TChild> AsBlock(ITokenRoot<TChild> context);
    //}

    public interface ITokenBlock<TModel, TChild> : ITokenSyntax
    {
        ITokenBlock<TModel, TChild> EndsWith(string terminator);
    }
}
