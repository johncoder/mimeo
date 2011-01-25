using System;
using System.Collections.Generic;

namespace Mimeo
{
    public class Mimeographs : Dictionary<Type, Mimeograph>
    {
        public void Add<TModel>(Mimeograph<TModel> mimeo)
        {
            Add(typeof(TModel), mimeo);
        }

        public string Render(string name, object model)
        {
            return this[model.GetType()].Render(name, model);
        }

        public void CreateStencil<TModel>(string name, string template)
        {
            this[typeof(TModel)].CreateStencil(name, template);
        }
    }
}