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
        private readonly IBlockToken<TModel> _token;

        public ComplexNegative(IBlockToken<TModel> token, IEnumerable<Space> spaces)
        {
            _token = token;
            _spaces = spaces.ToList();
        }

        public override void GetContents(object model, StringBuilder stringBuilder)
        {
            Ensure.ArgumentNotNull(model, "model");

            if (!CanHandle(model))
                return;

            foreach (var item in _token.Items((TModel)model))
            {
                foreach (var space in _spaces)
                {
                    if (space.CanHandle(item))
                    {
                        if (space.ShouldHandle(item))
                            space.GetContents(item, stringBuilder);
                    }
                    else
                    {
                        if (space.CanHandle(model) && space.ShouldHandle(model))
                            space.GetContents(model, stringBuilder);
                    }
                }
            }
        }

        public override bool CanHandle(object model)
        {
            return _token.CanHandle(model);
        }

        public override bool ShouldHandle(object model)
        {
            return _token.ShouldHandle(model);
        }
    }
}
