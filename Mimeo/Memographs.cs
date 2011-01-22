using System;
using System.Collections.Generic;

namespace Mimeo
{
    public class Memographs : Dictionary<Type, Mimeo>
    {
        public void Add<TModel>(Mimeo<TModel> mimeo)
        {
            Add(typeof(TModel), mimeo);
        }

        public string Render<TModel>(string name, TModel model)
        {
            return this[typeof(TModel)].Render(name, model);
        }
    }
}