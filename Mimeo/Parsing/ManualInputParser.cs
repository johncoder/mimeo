using System.Collections.Generic;
using System.Text;
using Mimeo.Design;
using Mimeo.Internal;
using Mimeo.Templating;

namespace Mimeo.Parsing
{
    public class ManualInputParser : IInputParser
    {
        private readonly string _template;
        private int _currentPosition;
        private IToken _currentToken;

        public ManualInputParser(string template)
        {
            _template = template;
        }

        IStencil IInputParser.Parse(IToken token)
        {
            return Parse(token);
        }

        public IStencil Parse(IToken token, int start = 0)
        {
            var stencil = new Stencil();
            var stringBuilder = new StringBuilder();

            for (int i = start; i < _template.Length; i++)
            {
                var result = ProcessChildTokens(token, stencil, stringBuilder);
                i = _currentPosition;
                switch(result.Type)
                {
                    case TokenType.Simple:
                        stencil.Add(token.CreateSpace());
                        break;
                    case TokenType.Complex:
                        stencil.Add(token.CreateSpace());
                        break;
                    case TokenType.NotAToken:
                    default:
                        continue;
                }
            }

            if (_template.Length - _currentPosition > 0)
                stencil.Add(new Positive(stringBuilder.ToString()));

            return stencil;
        }

        private ProcessTokenResult ProcessChildTokens(IToken token, Stencil stencil, StringBuilder stringBuilder)
        {
            char c = _template[_currentPosition];

            foreach (var child in token.Children)
            {
                if (string.IsNullOrEmpty(child.Identifier))
                {
                    continue;
                }

                if (CurrentIsToken(_currentPosition, child.Identifier))
                {
                    if (_currentPosition > 0)
                        stencil.Add(new Positive(stringBuilder.ToString()));

                    //stringBuilder.Clear();

                    _currentToken = child;

                    //if (_currentPosition == 0)
                    //    _currentPosition += child.Identifier.Length;
                    //else
                    _currentPosition += child.Identifier.Length;

                    if (string.IsNullOrEmpty(child.Terminator))
                    {
                        //stringBuilder.Append(c);
                        var result = new ProcessTokenResult { Position = _currentPosition, Type = TokenType.Simple };
                        return result;
                    }

                    var complex = new ProcessTokenResult { Position = _currentPosition, Type = TokenType.Complex };
                    //stringBuilder.Append(c);
                    return complex;
                }
            }

            _currentPosition++;

            var notAToken = new ProcessTokenResult {Type = TokenType.NotAToken, Position = _currentPosition};
            return notAToken;
        }


        public class ProcessTokenResult
        {
            public int Position { get; set; }
            public TokenType Type { get; set; }
            public IToken Token { get; set; }
        }

        public enum TokenType
        {
            Simple,
            Complex,
            NotAToken
        }

        public bool CurrentIsToken(int start, string identifier)
        {
            Ensure.ArgumentNotNullOrEmpty(identifier, "identifier");

            int step = 0;
            int end = start + identifier.Length;

            if (end > _template.Length)
                return false;

            for (int i = start; i < end; i++)
            {
                if (_template[i] != identifier[step])
                    return false;
                step++;
            }

            return true;
        }
    }
}
