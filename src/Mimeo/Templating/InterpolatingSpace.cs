using System;
using System.Text;

namespace Mimeo.Templating
{
    public class InterpolatingSpace : Space
    {
        private Func<object, string> _inject;

        public void Initialize(string input, string pattern, Func<dynamic, string> inject)
        {
            _inject = inject;
        }

        public override bool CanHandle(object model)
        {
            return true;
        }

        public override bool ShouldHandle(object model)
        {
            return true;
        }

        public override void GetContents(object model, StringBuilder stringBuilder)
        {
            stringBuilder.Append(_inject(model));
        }
    }
}
