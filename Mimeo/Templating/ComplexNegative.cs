using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mimeo.Design;
using Mimeo.Internal;

namespace Mimeo.Templating
{
    public class ComplexNegative<TModel, TChild> : Space
    {
        private readonly ICollection<Space> _spaces;
        private readonly IToken<TModel, TChild> _token;

        public ComplexNegative(IToken<TModel, TChild> token, IEnumerable<Space> spaces)
        {
            _token = token;
            _spaces = spaces.ToList();
        }

        public override void GetContents(object model, StringBuilder stringBuilder)
        {
            Ensure.ArgumentNotNull(model, "model");

            foreach (var item in _token.Items((TModel)model))
            {
                foreach (var space in _spaces)
                {
                    space.GetContents(item, stringBuilder);
                }
            }
        }
    }
}
