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
            var obj = _token.GetChild(model);
            var objs = _token.Items((TModel) model);

            if (!objs.Any() && obj != null)
            {
                foreach (var space in _spaces)
                {
                    if (space.CanHandle(obj))
                    {
                        if (space.ShouldHandle(obj))
                        {
                            space.GetContents(obj, stringBuilder);
                            continue;
                        }
                    }
                    else if (_token.CanHandle(obj))
                    {
                        stringBuilder.Append(_token.GetValue(obj));
                        continue;
                    }
                    else if (_token.Children.Any(c => c.CanHandle(obj)))
                    {
                        foreach (var child in _token.Children)
                            if (child.CanHandle(obj))
                            {
                                stringBuilder.Append(child.GetValue(obj));
                                break;
                            }
                        continue;
                    }
                    else
                    {
                        if (space.CanHandle(obj) && space.ShouldHandle(obj))
                        {
                            space.GetContents(obj, stringBuilder);
                            continue;
                        }
                    }
                }
            }
            else
            foreach (var item in objs)
            {
                foreach (var space in _spaces)
                {
                    if (space.CanHandle(item))
                    {
                        if (space.ShouldHandle(item))
                        {
                            space.GetContents(item, stringBuilder);
                            continue;
                        }
                    }
                    else if (_token.CanHandle(item))
                    {
                        stringBuilder.Append(_token.GetValue(item));
                        continue;
                    }
                    else if (_token.Children.Any(c => c.CanHandle(item)))
                    {
                        foreach (var child in _token.Children)
                            if (child.CanHandle(item))
                            {
                                stringBuilder.Append(child.GetValue(item));
                                break;
                            }
                        continue;
                    }
                    else
                    {
                        if (space.CanHandle(item) && space.ShouldHandle(item))
                        {
                            space.GetContents(item, stringBuilder);
                            continue;
                        }
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
