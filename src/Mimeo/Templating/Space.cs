using System.Text;

namespace Mimeo.Templating
{
    /// <summary>
    /// A space, representing a value to be appended to a string builder.
    /// </summary>
	public abstract class Space
	{
        /// <summary>
        /// Determines whether or not the space can handle the object.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
	    public abstract bool CanHandle(object model);

        /// <summary>
        /// Determines whether or not the space should handle the object.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
	    public abstract bool ShouldHandle(object model);

        /// <summary>
        /// Appends the contents of the space to the string builder.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="stringBuilder"></param>
		public abstract void GetContents(object model, StringBuilder stringBuilder);

        /// <summary>
        /// Adds child spaces.
        /// </summary>
        /// <param name="obj"></param>
	    public virtual void Add(object obj) { }
	}
}
