namespace Mimeo.Design.Syntax
{
    public class SimpleToken<TModel> : ISimpleToken<TModel>
    {
        public ISimpleToken<TModel> Encode(bool shouldEncode)
        {
            return this;
        }
    }
}
