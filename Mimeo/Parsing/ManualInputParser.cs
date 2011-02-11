using System;
using System.Text;
using Mimeo.Design;
using Mimeo.Internal;
using Mimeo.Templating;

namespace Mimeo.Parsing
{
    public class ManualInputParser : IInputParser
    {
        public string Template { get; set; }
        public int CurrentPosition { get; set; }

        IStencil IInputParser.Parse(IToken token, string template)
        {
            Template = template;
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
            CurrentPosition = start;
            
            var stencil = new Stencil();
            var stringBuilder = new StringBuilder();
            var lookForTerminator = !string.IsNullOrEmpty(terminator);
            var terminated = false;
            
            while (CurrentPosition < Template.Length - 1)
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

                        Parse(result.Token, CurrentPosition, ns, result.Token.Terminator);

                        foreach (var space in ns)
                            s.Add(space);

                        AddSpace(stencil, newStencil, s);
                        continue;
                    case TokenType.Interpolation:
                        var interpolatingSpace = result.Token.CreateSpace();
                        AddSpace(stencil, newStencil, interpolatingSpace);
                        continue;
                    default:
                        continue;
                }
            }

            if (newStencil == null && Template.Length - 1 == CurrentPosition)
            {
                stringBuilder.Append(Template[CurrentPosition]);
                CurrentPosition++;
            }
            
            if (Template.Length - CurrentPosition >= 0 && stringBuilder.Length > 0)
                AddSpace(stencil, newStencil, new Positive(stringBuilder.ToString()));

            if (terminated)
                CurrentPosition += terminator.Length;

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

                if (child.Interpolation != null && CurrentIsToken(child.Interpolation))
                {
                    if (CurrentPosition > 0 && stringBuilder.Length > 0)
                    {
                        stencil.Add(new Positive(stringBuilder.ToString()));
                        stringBuilder.Clear();
                    }

                    CurrentPosition += child.Interpolation.TotalLength;

                    var result = new ProcessTokenResult { Position = CurrentPosition, Type = TokenType.Interpolation, Token = child };
                    return result;
                }
                else if (CurrentIsToken(child.Identifier))
                {
                    if (CurrentPosition > 0 && stringBuilder.Length > 0)
                    {
                        stencil.Add(new Positive(stringBuilder.ToString()));
                        stringBuilder.Clear();
                    }

                    CurrentPosition += child.Identifier.Length;

                    if (string.IsNullOrEmpty(child.Terminator))
                    {
                        var result = new ProcessTokenResult { Position = CurrentPosition, Type = TokenType.Simple, Token = child };
                        return result;
                    }

                    var complex = new ProcessTokenResult { Position = CurrentPosition, Type = TokenType.Complex, Token = child };
                    return complex;
                }
                
            }

            char c = Template[CurrentPosition];
            stringBuilder.Append(c);
            CurrentPosition++;

            var notAToken = new ProcessTokenResult {Type = TokenType.NotAToken, Position = CurrentPosition};
            return notAToken;
        }

        public bool CurrentIsToken(InterpolationData interpolationData)
        {
            int step = 0;
            int end = CurrentPosition + interpolationData.Start.Length;

            if (end > Template.Length)
                return false;

            int tokenEndPosition = 0;
            Func<int, bool> isCurrentStartOfEndToken = start =>
            {
                int endPosition = 0;
                for (int j = start; j < Template.Length && endPosition < interpolationData.End.Length; j++)
                {
                    if (Template[j] == interpolationData.End[endPosition])
                    {
                        endPosition++;
                        continue;
                    }

                    return false;
                }
                tokenEndPosition = start + endPosition;
                return true;
            };

            // completing this loop means that the current position
            // is the beginning of an interpolation token.
            for (int i = CurrentPosition; i <= end && step < interpolationData.Start.Length; i++)
            {
                if (Template[i] != interpolationData.Start[step])
                    return false;
                step++;
            }

            for (int i = end; i < Template.Length; i++)
            {

                if (Template[i] == interpolationData.End[0])
                {
                    int endPosition = 0;
                    if (isCurrentStartOfEndToken(i))
                    {
                        interpolationData.ArgumentInput = Template.Substring(end, i - end);
                        break;
                    }
                    continue;
                }
            }
            //CurrentPosition = tokenEndPosition;
            return true;
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
            Interpolation,
            NotAToken
        }

        public bool CurrentIsToken(string identifier)
        {
            Ensure.ArgumentNotNullOrEmpty(identifier, "identifier");

            int step = 0;
            int end = CurrentPosition + identifier.Length;

            if (end > Template.Length)
                return false;

            for (int i = CurrentPosition; i < end; i++)
            {
                if (Template[i] != identifier[step])
                    return false;
                step++;
            }

            return true;
        }
    }
}
