using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mimeo.Design;
using Mimeo.Design.Syntax;
using Mimeo.Parsing;
using Mimeo.Templating;

namespace Mimeo
{
    /// <summary>
    /// A base mimeograph implementation.
    /// </summary>
    public abstract class Mimeograph
    {
        /// <summary>
        /// Renders an object using a stencil by name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract string Render(string name, object obj);

        /// <summary>
        /// Creates a stencil with a name using the template.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public abstract IStencil CreateStencil(string name, Stream template);

        /// <summary>
        /// Creates a stencil with a name using the template.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public abstract IStencil CreateStencil(string name, string template);

        /// <summary>
        /// A dictionary of stencils that have been previously created.
        /// </summary>
        public IDictionary<string, IStencil> Stencils { get; set; }

        protected Mimeograph()
        {
            Stencils = new Dictionary<string, IStencil>();
        }
    }

    /// <summary>
    /// A strongly typed mimeograph implementation.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class Mimeograph<TModel> : Mimeograph, IMimeograph<TModel>
    {
        /// <summary>
        /// The token builder used to manage template parsing and rendering.
        /// </summary>
        public ITokenRoot<TModel> Builder { get; set; }

        /// <summary>
        /// The input parser used to create a stencil.
        /// </summary>
        public IInputParser Parser { get; set; }

        /// <summary>
        /// Initializes the mimeograph. Uses ManualInputParser.
        /// </summary>
        public Mimeograph() : this(b => {}, new ManualInputParser()) { }

        /// <summary>
        /// Initializes the mimeograph. Uses ManualInputParser.
        /// </summary>
        /// <param name="configureBuilder"></param>
        public Mimeograph(Action<ITokenRoot<TModel>> configureBuilder) : this(configureBuilder, new ManualInputParser()) { }

        /// <summary>
        /// Initializes the mimeograph. The token builder should be preconfigured. Uses ManualInputParser.
        /// </summary>
        /// <param name="tokenBuilder"></param>
        public Mimeograph(TokenBuilder<TModel> tokenBuilder) : this(tokenBuilder, new ManualInputParser()) { }

        /// <summary>
        /// Initializes the mimeograph.
        /// </summary>
        /// <param name="configureBuilder"></param>
        /// <param name="parser"></param>
        public Mimeograph(Action<ITokenRoot<TModel>> configureBuilder, IInputParser parser) : this(new TokenBuilder<TModel>(), parser)
        {
            configureBuilder(Builder);
        }

        /// <summary>
        /// Initializes mimeograph, providing a token builder that is already configured.
        /// </summary>
        /// <param name="tokenBuilder"></param>
        /// <param name="parser"></param>
        public Mimeograph(TokenBuilder<TModel> tokenBuilder, IInputParser parser) : this(parser)
        {
            Builder = tokenBuilder;
        }

        private Mimeograph(IInputParser parser)
        {
            Parser = parser;
        }

        /// <summary>
        /// Renders an object with a specific stencil.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override string Render(string name, object obj)
        {
            return Render(name, (TModel)obj);
        }

        public override IStencil CreateStencil(string name, Stream template)
        {
            if (Stencils.ContainsKey(name))
                return Stencils[name];

            if (Builder == null)
                throw new NullReferenceException("Cannot parse template without builder. Configure a builder before creating stencils");

            var stencil = Parser.Parse(Builder.Token, template);

            Stencils.Add(name, stencil);

            return stencil;
        }

        public override IStencil CreateStencil(string name, string template)
        {
            using (var ms = new MemoryStream(Encoding.Default.GetBytes(template)))
            {
                return CreateStencil(name, ms);
            }
        }

        /// <summary>
        /// Renders an object with a specific stencil.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public string Render(string name, TModel model)
        {
            var stringBuilder = new StringBuilder();
            Stencils[name].Render(model, stringBuilder);
            return stringBuilder.ToString();
        }
    }
}
