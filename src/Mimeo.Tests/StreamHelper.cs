using System.IO;
using System.Text;

namespace Mimeo.Tests
{
    public static class StreamHelper
    {
        public static Stream ToStream(this string s)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(s));
        }
    }
}
