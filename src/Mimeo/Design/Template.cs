namespace Mimeo.Design
{
    /// <summary>
    /// A base template class for defining a template.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public abstract class Template<TModel> : TokenBuilder<TModel>
    {
        /// <summary>
        /// Initializes formatters and tokens.
        /// </summary>
        public void Initialize()
        {
            ConfigureFormatters();
            ConfigureTokens();
        }

        /// <summary>
        /// This method initializes all base IValueFormatters that are to be used for all token replacements.
        /// </summary>
        protected abstract void ConfigureFormatters();

        /// <summary>
        /// This method initializes all tokens that are supported by the template.
        /// </summary>
        protected abstract void ConfigureTokens();
    }
}
