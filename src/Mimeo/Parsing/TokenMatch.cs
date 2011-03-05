using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mimeo.Design;

namespace Mimeo.Parsing
{
    public class TokenMatch
    {
        public IToken Token { get; set; }
        public Match Match { get; set; }
    }

    public class NewTokenMatch
    {
        public IToken Token { get; set; }
        public int Start { get; set; }
        public int Finish { get; set; }
        public ICollection<NewTokenMatch> Children { get; set; }

        public NewTokenMatch()
        {
            Children = new List<NewTokenMatch>();
        }
    }
}
