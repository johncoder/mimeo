﻿using System;
using System.Collections.Generic;
using Mimeo.Templating;

namespace Mimeo.Design
{
    public interface IToken
    {
        string Identifier { get; set; }
        string Terminator { get; set; }
        string GetValue(object obj);
        bool CanHandle(object obj);
        bool ShouldHandle(object obj);
        ICollection<IToken> Children { get; set; }
        IToken Parent { get; set; }
        void SetParent(IToken token);
        void AddChild(IToken token);
        Space CreateSpace();
        Space CreateSpace(IEnumerable<Space> spaces);
    }

    public interface IToken<TModel> : IToken
    {
        Func<TModel, string> Resolve { get; set; }
        Func<TModel, bool> Condition { get; set; }
        string GetValue(TModel model);
        //bool CanHandle(TModel model);
    }

    public interface IToken<TModel, TChild> : IToken<TModel>
    {
        Func<TModel, IEnumerable<TChild>> Items { get; set; }
    }
}
