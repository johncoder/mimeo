﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mimeo.Design;
using Mimeo.Design.Tokens;
using Mimeo.Templating;

namespace Mimeo.Parsing
{
    /// <summary>
    /// Manually parses a template to produce a stencil.
    /// </summary>
    public sealed class ManualInputParser : IInputParser
    {
        private Stream TemplateStream { get; set; }
        private Stack<Result> TokenStack { get; set; }
        private Stack<Space> SpaceStack { get; set; }
        private List<IToken> _prospectiveTokens = new List<IToken>();
        private dynamic _currentSpace;

        IStencil IInputParser.Parse(IToken token, Stream template)
        {
            TokenStack = new Stack<Result>();
            SpaceStack = new Stack<Space>();
            TemplateStream = template;
            return Parse(token);
        }

        private IStencil Parse(IToken token)
        {
            var stencil = new Stencil();
            _currentSpace = stencil;
            var streamReader = new StreamReader(TemplateStream);

            var buffer = new StringBuilder();
            Reset(buffer, token);

            while (!streamReader.EndOfStream)
            {
                var currentCharacter = (char)streamReader.Read();
                buffer.Append(currentCharacter);

                var result = ProcessBuffer(buffer);

                switch(result.Type)
                {
                    case TokenType.Simple:
                    case TokenType.Interpolation:
                        AddPositiveSpace(_currentSpace, result.Payload);
                        _currentSpace.Add(result.Token.CreateSpace());
                        Reset(buffer);
                        break;
                    case TokenType.Complex:
                        AddPositiveSpace(_currentSpace, result.Payload);
                        var complexSpace = result.Token.CreateSpace();
                        _currentSpace.Add(complexSpace);
                        TokenStack.Push(result);
                        SpaceStack.Push(complexSpace);
                        _currentSpace = complexSpace;
                        Reset(buffer, result.Token);
                        break;
                    case TokenType.Terminator:
                        AddPositiveSpace(_currentSpace, result.Payload);
                        TokenStack.Pop();
                        SpaceStack.Pop();
                        _currentSpace = (object)SpaceStack.FirstOrDefault() ?? stencil;
                        Reset(buffer, TokenStack.Select(t => t.Token).FirstOrDefault() ?? token);
                        break;
                    default:
                        break;
                }
            }

            AddPositiveSpace(stencil, buffer.ToString());

            TemplateStream = null;
            TokenStack = null;
            SpaceStack = null;
            _prospectiveTokens = null;

            return stencil;
        }

        private void AddPositiveSpace(dynamic space, string payload)
        {
            if (string.IsNullOrEmpty(payload))
                return;

            space.Add(new Positive(payload));
        }

        private Result ProcessBuffer(StringBuilder buffer)
        {
            string bufferContents = null;

            // Check to see if the current character ends the terminator
            if (TokenStack.Any())
            {
                var peek = TokenStack.Peek();
                bufferContents = buffer.ToString();
                if (peek.Type == TokenType.Complex && bufferContents.EndsWith(peek.Token.Terminator))
                {
                    var terminator = new Result {Type = TokenType.Terminator};
                    terminator.Payload = buffer.ToString(0, buffer.Length - peek.Token.Terminator.Length);
                    return terminator;
                }
            }

            if (string.IsNullOrEmpty(bufferContents))
                bufferContents = buffer.ToString();

            foreach(var child in _prospectiveTokens)
            {
                var isToken = bufferContents.Last() == child.Identifier.Last()
                              && bufferContents.EndsWith(child.Identifier);
                
                var match = CheckForInterpolation(child.Interpolation, bufferContents);
                
                var isInterpolated = match != null && match.Success;

                if (isToken || isInterpolated)
                {
                    var result = new Result { Token = child };

                    if (child.Interpolation != null)
                    {
                        result.Payload = buffer.ToString(0, bufferContents.IndexOf(child.Interpolation.Start));
                        child.Interpolation.ArgumentInput = match.Value;
                        result.Type = TokenType.Interpolation;
                    }
                    else if (string.IsNullOrEmpty(child.Terminator))
                    {
                        result.Payload = buffer.ToString(0, buffer.Length - child.Identifier.Length);
                        result.Type = TokenType.Simple;
                    }
                    else
                    {
                        result.Payload = buffer.ToString(0, buffer.Length - child.Identifier.Length);
                        result.Type = TokenType.Complex;
                    }

                    return result;
                }
            }

            return new Result { Type = TokenType.NotAToken };
        }

        private static Match CheckForInterpolation(InterpolationData interpolation, string bufferContents)
        {
            if (interpolation == null)
                return null;

            if (!bufferContents.EndsWith(interpolation.End))
                return null;

            var startPosition = bufferContents.IndexOf(interpolation.Start);

            if (startPosition < 0)
                return null;

            var index = startPosition + interpolation.Start.Length;

            var length = bufferContents.IndexOf(interpolation.End);

            var input = bufferContents.Substring(index, length - index);

            var match = Regex.Match(input, interpolation.ArgumentPattern);
            
            return match;
        }

        private void Reset(StringBuilder buffer, IToken token = null)
        {
            buffer.Clear();

            if (token == null)
                return;

            if (_prospectiveTokens == null)
                _prospectiveTokens = new List<IToken>();

            _prospectiveTokens.Clear();
            _prospectiveTokens.AddRange(token.Children);
        }
        
        /// <summary>
        /// The result of parsing a token.
        /// </summary>
        public class Result
        {
            /// <summary>
            /// The type of token that this result represents.
            /// </summary>
            public TokenType Type { get; set; }

            /// <summary>
            /// The paylod of string data that represents the token.
            /// </summary>
            public string Payload { get; set; }

            /// <summary>
            /// The token matching this result.
            /// </summary>
            public IToken Token { get; set; }
        }

        /// <summary>
        /// An enumeration of TokenTypes. This indicates what the parser should do with the current string buffer.
        /// </summary>
        public enum TokenType
        {
            /// <summary>
            /// A simple token that is replaced by a value.
            /// </summary>
            Simple,
            /// <summary>
            /// A complex token that is replaced by a block of content, often consisting of other simple tokens.
            /// </summary>
            Complex,
            /// <summary>
            /// A token whose value is to be interpolated, or derived from external input. This is similar to a simple token.
            /// </summary>
            Interpolation,
            /// <summary>
            /// A result that does not represent a token.
            /// </summary>
            NotAToken,
            /// <summary>
            /// The identifier that signals the end of a complex token.
            /// </summary>
            Terminator
        }
    }
}
