using System.Text;

namespace Mimeo.Templating
{
    public class Positive : Space
    {
        private readonly string _value;

        public Positive(string value)
        {
            _value = value;
        }

        public override void GetContents(object model, StringBuilder stringBuilder)
        {
            stringBuilder.Append(_value);
        }
    }
}
