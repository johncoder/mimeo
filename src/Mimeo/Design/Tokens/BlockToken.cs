using System;
using System.Collections.Generic;
using Mimeo.Templating;

namespace Mimeo.Design.Tokens
{
    /// <summary>
    /// A token which represents a block of tokens.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TChild"></typeparam>
    public class BlockToken<TModel, TChild> : Token<TModel>, IBlockToken<TModel, TChild>
    {
        /// <summary>
        /// A function that retrieves an enumeration of items off of TModel.
        /// </summary>
        public Func<TModel, IEnumerable<object>> Items { get; set; }

        /// <summary>
        /// A function that retrieves an item off of TModel.
        /// </summary>
        public Func<TModel, TChild> Child { get; set; }

        /// <summary>
        /// Gets the child object off of TModel.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override object GetChild(object obj)
        {
            if (Child == null) return null;

            return Child((TModel)obj);
        }

        /// <summary>
        /// Initializes the block token.
        /// </summary>
        public BlockToken()
        {
            Items = m => new List<object>();
        }

        /// <summary>
        /// Creates a Space used in rendering an object.
        /// </summary>
        /// <returns></returns>
        public override Space CreateSpace()
        {
            return CreateSpace(new List<Space>());
        }

        /// <summary>
        /// Creates a space used in rendering an object.
        /// </summary>
        /// <param name="spaces"></param>
        /// <returns></returns>
        public override Space CreateSpace(IEnumerable<Space> spaces)
        {
            return new ComplexNegative<TModel, TChild>(this, spaces);
        }
    }
}