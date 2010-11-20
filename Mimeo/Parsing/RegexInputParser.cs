using System;
using System.Linq;
using System.Collections.Generic;
using Mimeo.Design;
using System.Text.RegularExpressions;
using Mimeo.Internal;

namespace Mimeo.Parsing
{
    public class RegexInputParser : IInputParser
    {
        public ICollection<TokenMatch> Matches { get; set; }
        private readonly string _template;

        public RegexInputParser(string template)
        {
            Ensure.ArgumentNotNullOrEmpty(template, "template");
            _template = template;
            Matches = new List<TokenMatch>();
        }

        public void Parse(IToken token)
        {
            if (!string.IsNullOrEmpty(token.Identifier))
                AddMatches(token, t => t.Identifier);

            if (!string.IsNullOrEmpty(token.Terminator))
            {
                AddMatches(token, t => t.Terminator);
            }

            foreach (var child in token.Children)
            {
                Parse(child);
                //if (child.Children != null && child.Children.Any())
                //    foreach(var grandchild in child.Children)
                //        Parse(grandchild);
            }
        }

        private void AddMatches(IToken token, Func<IToken, string> pattern)
        {
            var matches = Regex.Matches(_template, pattern(token)).AsEnumerable()
                .Select(match => new TokenMatch {Token = token, Match = match});

            foreach(var tokenMatch in matches)
            {
                Matches.Add(tokenMatch);
            }
        }
    }
}
