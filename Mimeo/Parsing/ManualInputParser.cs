﻿using System.Text;
using Mimeo.Design;
using Mimeo.Internal;
using Mimeo.Templating;

namespace Mimeo.Parsing
{
    public class ManualInputParser : IInputParser
    {
        private string _template;
        private int _currentPosition;

        IStencil IInputParser.Parse(IToken token, string template)
        {
            _template = template;
            return Parse(token);
        }

        private static void AddSpace(IStencil current, IStencil parent, Space space)
        {
            if (parent != null)
                parent.Add(space);
            else
                current.Add(space);
        }

        protected IStencil Parse(IToken token, int start = 0, IStencil newStencil = null, string terminator = null)
        {
            _currentPosition = start;
            var stencil = new Stencil();
            var stringBuilder = new StringBuilder();
            var lookForTerminator = !string.IsNullOrEmpty(terminator);
            var terminated = false;
            
            while (_currentPosition < _template.Length - 1)
            {
                if (lookForTerminator && CurrentIsToken(terminator))
                {
                    terminated = true;
                    break;
                }

                var result = ProcessChildTokens(token, newStencil ?? stencil, stringBuilder);
                switch (result.Type)
                {
                    case TokenType.Simple:
                        AddSpace(stencil, newStencil, result.Token.CreateSpace());
                        continue;
                    case TokenType.Complex:
                        var s = result.Token.CreateSpace();
                        var ns = new Stencil();

                        Parse(result.Token, _currentPosition, ns, result.Token.Terminator);

                        foreach (var space in ns)
                            s.Add(space);

                        AddSpace(stencil, newStencil, s);
                        continue;
                    default:
                        continue;
                }
            }

            if (newStencil == null && _template.Length - 1 == _currentPosition)
            {
                stringBuilder.Append(_template[_currentPosition]);
                _currentPosition++;
            }
            
            if (_template.Length - _currentPosition >= 0 && stringBuilder.Length > 0)
                AddSpace(stencil, newStencil, new Positive(stringBuilder.ToString()));

            if (terminated)
                _currentPosition += terminator.Length;

            return stencil;
        }

        private ProcessTokenResult ProcessChildTokens(IToken token, IStencil stencil, StringBuilder stringBuilder)
        {
            foreach (var child in token.Children)
            {
                if (string.IsNullOrEmpty(child.Identifier))
                {
                    continue;
                }

                if (CurrentIsToken(child.Identifier))
                {
                    if (_currentPosition > 0 && stringBuilder.Length > 0)
                    {
                        stencil.Add(new Positive(stringBuilder.ToString()));
                        stringBuilder.Clear();
                    }

                    _currentPosition += child.Identifier.Length;

                    if (string.IsNullOrEmpty(child.Terminator))
                    {
                        var result = new ProcessTokenResult { Position = _currentPosition, Type = TokenType.Simple, Token = child };
                        return result;
                    }

                    var complex = new ProcessTokenResult { Position = _currentPosition, Type = TokenType.Complex, Token = child };
                    return complex;
                }
                
            }

            char c = _template[_currentPosition];
            stringBuilder.Append(c);
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

        public bool CurrentIsToken(string identifier)
        {
            Ensure.ArgumentNotNullOrEmpty(identifier, "identifier");

            int step = 0;
            int end = _currentPosition + identifier.Length;

            if (end > _template.Length)
                return false;

            for (int i = _currentPosition; i < end; i++)
            {
                if (_template[i] != identifier[step])
                    return false;
                step++;
            }

            return true;
        }
    }
}
