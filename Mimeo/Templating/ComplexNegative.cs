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

        public ICollection<Space> Spaces { get { return _spaces; } }
        public IBlockToken<TModel> Token { get { return _token; } }

        public ComplexNegative(IBlockToken<TModel> token, IEnumerable<Space> spaces)
        {
            Ensure.ArgumentNotNull(token, "token");
            Ensure.ArgumentNotNull(spaces, "spaces");

            _token = token;
            _spaces = spaces.ToList();
        }

        public override void GetContents(object model, StringBuilder stringBuilder)
        {
            Ensure.ArgumentNotNull(model, "model");
            Ensure.ArgumentNotNull(stringBuilder, "stringBuilder");

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

        public override void Add(object obj)
        {
            Ensure.ArgumentNotNull(obj, "obj");
            Spaces.Add(obj as Space);
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
