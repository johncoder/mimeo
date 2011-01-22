using System.Collections.Generic;
using Mimeo.Design;
using Mimeo.Templating;

namespace Mimeo.Parsing
{
    public interface IInputParser
    {
        IStencil Parse(IToken token, string template);
    }
}