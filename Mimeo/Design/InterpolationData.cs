using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Mimeo.Design
{
    public class InterpolationData
    {
        public string Start { get; set; }
        public string End { get; set; }
        public string ArgumentPattern { get; set; }
        public string ArgumentInput { get; set; }
        public Func<dynamic, string> Injection { get; set; }

        public int TotalLength
        {
            get
            {
                if (Start == null || ArgumentInput == null || End == null)
                    return 0;
                return Start.Length + ArgumentInput.Length + End.Length;
            }
        }

        public string GetIdentifier()
        {
            return Start + End;
        }

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