
using System.IO;
using System.Xml.Serialization;

namespace Mimeo.Sample.Tests
{
    public static class Helpers
    {
        public static TModel Deserialize<TModel>(this FileInfo fileInfo) where TModel : class
        {
            XmlSerializer xs = new XmlSerializer(typeof(TModel));
            var obj = xs.Deserialize(fileInfo.OpenRead());
            return obj as TModel;
        }
    }
}
