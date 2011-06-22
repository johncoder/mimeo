using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mimeo.Design;
using Mimeo.Design.Tokens;
using Mimeo.Internal;

namespace Mimeo.Templating
{
    /// <summary>
    /// A space representing a collection of child spaces to be rendered.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TChild"></typeparam>
    public class ComplexNegative<TModel, TChild> : Space
    {
        private readonly ICollection<Space> _spaces;
        private readonly IBlockToken<TModel> _token;

        /// <summary>
        /// Child spaces to be rendered.
        /// </summary>
        public ICollection<Space> Spaces { get { return _spaces; } }

        /// <summary>
        /// The token that represents the current space.
        /// </summary>
        public IBlockToken<TModel> Token { get { return _token; } }

        /// <summary>
        /// Initializes the complex negative space.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="spaces"></param>
        public ComplexNegative(IBlockToken<TModel> token, IEnumerable<Space> spaces)
        {
            Ensure.ArgumentNotNull(token, "token");
            Ensure.ArgumentNotNull(spaces, "spaces");

            _token = token;
            _spaces = spaces.ToList();
        }

        /// <summary>
        /// Gets the contents of the space using the model.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="stringBuilder"></param>
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
                        stringBuilder.Append(_token.Evaluate(obj));
                        continue;
                    }
                    else if (_token.Children.Any(c => c.CanHandle(obj)))
                    {
                        foreach (var child in _token.Children)
                            if (child.CanHandle(obj))
                            {
                                stringBuilder.Append(child.Evaluate(obj));
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
                        stringBuilder.Append(_token.Evaluate(item));
                        continue;
                    }
                    else if (_token.Children.Any(c => c.CanHandle(item)))
                    {
                        foreach (var child in _token.Children)
                            if (child.CanHandle(item))
                            {
                                stringBuilder.Append(child.Evaluate(item));
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

        /// <summary>
        /// Adds a space to the collection of spaces.
        /// </summary>
        /// <param name="obj"></param>
        public override void Add(object obj)
        {
            Ensure.ArgumentNotNull(obj, "obj");
            Spaces.Add(obj as Space);
        }

        /// <summary>
        /// Determines whether or not the space can handle the object.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override bool CanHandle(object model)
        {
            return _token.CanHandle(model);
        }

        /// <summary>
        /// Determines whether or not the space should handle the object.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override bool ShouldHandle(object model)
        {
            return _token.ShouldHandle(model);
        }
    }
}
