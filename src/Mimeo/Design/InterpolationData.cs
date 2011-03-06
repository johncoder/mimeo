using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mimeo.Design
{
    /// <summary>
    /// Represents data that will be interpolated at render time.
    /// </summary>
    public class InterpolationData
    {
        /// <summary>
        /// A string that represents the beginning of the token, up to the argument pattern.
        /// </summary>
        public string Start { get; set; }

        /// <summary>
        /// A string that represents the end of the token, after the argument pattern.
        /// </summary>
        public string End { get; set; }

        /// <summary>
        /// A pattern that describes the allowable value for the argument. Named groups are captured.
        /// </summary>
        public string ArgumentPattern { get; set; }

        /// <summary>
        /// The value extracted from the token.
        /// </summary>
        public string ArgumentInput { get; set; }

        /// <summary>
        /// A function that performs an action based on the values extracted from the token.
        /// </summary>
        /// <remarks>Any captured named groups will appear as properties on the dynamic parameter.</remarks>
        public Func<dynamic, string> Injection { get; set; }

        /// <summary>
        /// The total length of the parsed token.
        /// </summary>
        public int TotalLength
        {
            get
            {
                if (Start == null || ArgumentInput == null || End == null)
                    return 0;
                return Start.Length + ArgumentInput.Length + End.Length;
            }
        }

        /// <summary>
        /// Returns the base identifier.
        /// </summary>
        /// <returns></returns>
        public string GetIdentifier()
        {
            return Start + End;
        }

        /// <summary>
        /// Parses the argument with the defined regular expression, and creates an expando object containing the captured values.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public Func<TModel, string> CreateValueDelegate<TModel>()
        {
            var regex = new Regex(ArgumentPattern);
            var match = regex.Match(ArgumentInput);

            var expando = new ExpandoObject() as IDictionary<string, object>;

            if (match.Groups.Count == 1)
                (expando as dynamic).ToString = ((Func<string>)(() => match.Value));
            else
            {
                foreach(var name in regex.GetGroupNames().Skip(1))
                {
                    expando[name] = match.Groups[name];
                }
            }

            return m => Injection(expando);
        }
    }
}