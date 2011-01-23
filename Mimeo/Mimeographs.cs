using System;
using System.Collections.Generic;

namespace Mimeo
{
    public class Mimeographs : Dictionary<Type, Mimeo>
    {
        public void Add<TModel>(Mimeo<TModel> mimeo)
        {
            Add(typeof(TModel), mimeo);
        }

        public string Render<TModel>(string name, TModel model)
        {
            return this[typeof(TModel)].Render(name, model);
        }

        public void CreateStencil<TModel>(string name, string template)
        {
            this[typeof(TModel)].CreateStencil(name, template);
        }
    }
}