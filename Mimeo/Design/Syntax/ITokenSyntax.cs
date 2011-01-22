
namespace Mimeo.Design.Syntax
{
    public interface ITokenSyntax : IFluentSyntax
    {
        IToken Token { get; }
    }
}
