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
        public string Render(string name, object model)
        {
            return this[model.GetType()].Render(name, model);
        }

        /// <summary>
        /// Creates a stencil for an object type using a name.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="name"></param>
        /// <param name="template"></param>
        //public void CreateStencil<TModel>(string name, string template)
        //{
        //    this[typeof(TModel)].CreateStencil(name, template);
        //}

        public void CreateStencil<TModel>(string name, string template)
        {
            CreateStencil<TModel>(name, new MemoryStream(Encoding.UTF8.GetBytes(template)));
        }

        public void CreateStencil<TModel>(string name, Stream template)
        {
            this[typeof (TModel)].CreateStencil(name, template);
        }
    }
}