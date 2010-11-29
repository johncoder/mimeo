﻿using System.Collections.Generic;
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

                switch(result)
                {
                    case ProcessTokenResult.Simple:
                        break;
                    case ProcessTokenResult.Complex:
                        break;
                    case ProcessTokenResult.NotAToken:
                        continue;
                }
            }

            if (stringBuilder.Length > 0)
                stencil.Add(new Positive(stringBuilder.ToString()));

            return stencil;
        }

        private ProcessTokenResult ProcessChildTokens(IToken token, Stencil stencil, StringBuilder stringBuilder)
        {
            char c = _template[_currentPosition];

            foreach (var child in token.Children)
            {
                if (!string.IsNullOrEmpty(child.Identifier))
                {
                    if (CurrentIsToken(_currentPosition, child.Identifier))
                    {
                        stencil.Add(new Positive(stringBuilder.ToString()));
                        stringBuilder.Clear();

                        _currentToken = child;
                        _currentPosition += child.Identifier.Length - 1;

                        if (string.IsNullOrEmpty(child.Terminator))
                        {
                            return ProcessTokenResult.Simple;
                        }
                        
                        return ProcessTokenResult.Complex;
                    }

                    stringBuilder.Append(c);
                }
            }

            return ProcessTokenResult.NotAToken;
        }

        enum ProcessTokenResult
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
