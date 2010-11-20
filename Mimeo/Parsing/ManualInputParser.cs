using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mimeo.Design;
using Mimeo.Internal;

namespace Mimeo.Parsing
{
    public class ManualInputParser : IInputParser
    {
        private readonly string _template;
        private ICollection<NewTokenMatch> _matches;
        public ICollection<TokenMatch> Matches { get; set; }

        public ManualInputParser(string template)
        {
            _template = template;
            Matches = new List<TokenMatch>();
            _matches = new List<NewTokenMatch>();
        }

        void IInputParser.Parse(IToken token)
        {
            Parse(token);
        }

        public void Parse(IToken token, int start = 0)
        {
            for (int i = 0; i < _template.Length; i++)
            {
                char c = _template[i];

                if (!string.IsNullOrEmpty(token.Identifier))
                {
                    if (CurrentIsToken(i, token.Identifier))
                    {
                        
                    }
                }
            }
        }

        private bool CurrentIsToken(int start, string identifier)
        {
            Ensure.ArgumentNotNullOrEmpty(identifier, "identifier");

            int step = 0;
            int end = start + identifier.Length;

            if (end > _template.Length)
                return false;

            for (int i = start; i < end; i++)
            {
                if (_template[i] != identifier[step])
                    return false;
                step++;
            }

            return true;
        }
    }
}
