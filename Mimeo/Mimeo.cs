using System;
using System.Collections.Generic;
using System.Linq;
using Mimeo.Design;
using Mimeo.Design.Syntax;
using Mimeo.Parsing;
using Mimeo.Templating;

namespace Mimeo
{
    public interface IMimeo
    {
        IInputParser Parser { get; set; }
        ITokenRoot<TModel> GetBuilder<TModel>(string name);
        IStencil Parse(string name, string template);

    }

    public class Mimeo : IMimeo
    {
        private ICollection<Tuple<string, ITokenSyntax, IStencil>> Templates { get; set; }

        public Mimeo()
        {
            Templates = new List<Tuple<string, ITokenSyntax, IStencil>>();
            Parser = new ManualInputParser();
        }

        public IInputParser Parser { get; set; }

        public ITokenRoot<TModel> GetBuilder<TModel>(string name)
        {
            var row = Templates.SingleOrDefault(tuple => tuple.Item1 == name);
            if (row == null)
            {
                row = new Tuple<string, ITokenSyntax, IStencil>(name, null, null);
                Templates.Add(row);
            }

            var builder = row.Item2 as ITokenRoot<TModel>;
            
            if (builder == null)
            {
                builder = new TokenBuilder<TModel>(row.Item2 as IToken<TModel>);
            }

            return builder;
        }

        public IStencil Parse(string name, string template)
        {
            //var row = Templates.SingleOrDefault(tuple => tuple.Item1 == name);
            //if (row == null)
            //    throw new Exception("Template named " + name + " does not exist.");

            //var stencil = row.Item3;

            //if (stencil == null)
            //    row.Item3 = Parser.Parse(row.Item2.Token, template);
            throw new NotImplementedException();
        }
    }
}
