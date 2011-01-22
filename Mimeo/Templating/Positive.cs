using System.Text;

namespace Mimeo.Templating
{
    public class Positive : Space
    {
        private readonly string _value;

        public string Value { get { return _value; } }

        public Positive(string value)
        {
            _value = value;
        }

        public override void GetContents(object model, StringBuilder stringBuilder)
        {
            stringBuilder.Append(_value);
        }

        public override bool CanHandle(object model)
        {
            return true;
        }

        public override bool ShouldHandle(object model)
        {
            return true;
        }
    }
}
