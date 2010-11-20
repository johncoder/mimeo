using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mimeo.Internal
{
    internal static class MatchCollectionExtensions
    {
        public static IEnumerable<Match> AsEnumerable(this MatchCollection matches)
        {
            return matches.Cast<Match>();
        }
    }
}
