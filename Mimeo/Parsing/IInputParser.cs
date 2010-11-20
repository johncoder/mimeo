using System.Collections.Generic;
using Mimeo.Design;

namespace Mimeo.Parsing
{
    public interface IInputParser
    {
        ICollection<TokenMatch> Matches { get; set; }
        void Parse(IToken token);
    }
}