using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mimeo
{
    /// <summary>
    /// A dictionary of mimeograph objcets.
    /// </summary>
    public class Mimeographs : Dictionary<Type, Mimeograph>
    {
        /// <summary>
        /// Adds a mimeograph to the dictionary.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="mimeo"></param>
        public void Add<TModel>(Mimeograph<TModel> mimeo)
        {
            Add(typeof(TModel), mimeo);
        }

        /// <summary>
        /// Renders an object using a specified stencil name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public string Render<TModel>(string name, TModel model)
        {
            var modelType = typeof (TModel);

            var mimeograph = FindMimeograph(modelType);
            return mimeograph.Render(name, model);
        }

        private Mimeograph FindMimeograph(Type modelType)
        {
            if (ContainsKey(modelType))
                return this[modelType];

            foreach(var mimeograph in this)
            {
                if (mimeograph.Key.IsAssignableFrom(modelType))
                    return mimeograph.Value;
            }

            throw new KeyNotFoundException(string.Format("A mimeograph for type [{0}] has not been registered.", modelType.AssemblyQualifiedName));
        }

        /// <summary>
        /// Creates a stencil for the given type.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="name"></param>
        /// <param name="template"></param>
        public void CreateStencil<TModel>(string name, string template)
        {
            CreateStencil<TModel>(name, new MemoryStream(Encoding.UTF8.GetBytes(template)));
        }

        /// <summary>
        /// Creates a stencil for the given type.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="name"></param>
        /// <param name="template"></param>
        public void CreateStencil<TModel>(string name, Stream template)
        {
            var mimeograph = FindMimeograph(typeof(TModel));

            mimeograph.CreateStencil(name, template);
        }
    }
}