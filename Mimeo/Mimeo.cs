﻿using System;
using System.Collections.Generic;
using System.Text;
using Mimeo.Design;
using Mimeo.Design.Syntax;
using Mimeo.Parsing;
using Mimeo.Templating;

namespace Mimeo
{
    public abstract class Mimeo
    {
        public abstract string Render(string name, object obj);
    }

    public class Mimeo<TModel> : Mimeo, IMimeo<TModel>
    {
        private IDictionary<string, IStencil> Stencils { get; set; }

        public ITokenRoot<TModel> Builder { get; set; }
        public IInputParser Parser { get; set; }

        public Mimeo() : this(b => {}, new ManualInputParser()) { }

        public Mimeo(Action<ITokenRoot<TModel>> configureBuilder) : this(configureBuilder, new ManualInputParser()) { }

        public Mimeo(Action<ITokenRoot<TModel>> configureBuilder, IInputParser parser)
        {
            Stencils = new Dictionary<string, IStencil>();
            Parser = parser;
            Builder = new TokenBuilder<TModel>();

            configureBuilder(Builder);
        }

        public IStencil CreateStencil(string name, string template)
        {
            if (Stencils.ContainsKey(name))
                return Stencils[name];

            if (Builder == null)
                throw new NullReferenceException("Cannot parse template without builder. Configure a builder before creating stencils");

            var stencil = Parser.Parse(Builder.Token, template);

            Stencils.Add(name, stencil);

            return stencil;
        }

        public override string Render(string name, object obj)
        {
            return Render(name, (TModel)obj);
        }

        public string Render(string name, TModel model)
        {
            var stringBuilder = new StringBuilder();
            Stencils[name].GetContents(model, stringBuilder);
            return stringBuilder.ToString();
        }
    }
}