﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mimeo.Design.Syntax
{
    // Idea:  Include an Ignore token.  BlockToken... _builder.Ignore("{Ignore}").To("{/Ignore}");
    // Idea:  Add a Condition overload for each token type Func<TModel, bool> condition

    public interface ITokenRoot<TModel> : ITokenSyntax
    {
        ISimpleToken<TModel> Interpolate(string start, string argumentPattern, string end, Func<dynamic, string> inject);
        ISimpleToken<TModel> Interpolate(InterpolationData interpolationData);
        ISimpleToken<TModel> Tokenize(Func<TModel, string> replacement, string identifier);

        IConditionalToken<TModel, TChild> TokenizeIf<TChild>(Func<TModel, TChild> replacement, string identifier,
                                                           Func<TModel, bool> condition, Action<ITokenRoot<TChild>> context);
        IConditionalToken<TModel, TChild> TokenizeIf<TChild>(Func<TModel, TChild> replacement, string identifier,
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

    public interface ITokenBlock<TModel, TChild> : ITokenSyntax
    {
        ITokenBlock<TModel, TChild> EndsWith(string terminator);
    }
}