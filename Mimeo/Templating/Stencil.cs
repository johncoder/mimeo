using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Mimeo.Templating
{
	public class Stencil<TModel> : IStencil<TModel>
	{
		private readonly List<Space> _spaces;
		private readonly ComplexNegativeFactory _complexNegativeFactory;
		
		public IEnumerable<Space> Spaces
		{
			get { return _spaces; }
		}

        //public Stencil(string template, IEnumerable<Range> ranges)
        //{
        //    _spaces = new List<Space>();
        //    _complexNegativeFactory = new ComplexNegativeFactory();

        //    Generate(template, ranges);
        //}

        //private void Generate(string template, IEnumerable<Range> ranges)
        //{
        //    int position = 0;

        //    // group ranges by their parent block
        //    var groups = from range in ranges.OrderBy(r => r.Start)
        //                 where range.Terminator == null
        //                    && range.IsTerminator
        //                    //&& range.ParentStart == 0
        //                 group range by range.ParentStart into grp
        //                 select grp;

        //    position = ProcessRanges<TModel>(template, ranges, position, groups, _spaces);

        //    var leftover = template.Length - position;

        //    if (leftover > 0)
        //        _spaces.Add(new Positive<TModel>(string.Intern(template.Substring(position, leftover))));
        //}

        //public int ProcessRanges<TModelContext>(string template, IEnumerable<Range> ranges, int position, IEnumerable<IGrouping<int, Range>> groups, ICollection<Space> spaces)
        //{
        //    foreach (var range in ranges.OrderBy(r => r.Start))
        //    {
        //        var distance = range.Start - position;

        //        if (distance > 0)
        //        {
        //            var space = new Positive<TModelContext>(string.Intern(template.Substring(position, distance)));
        //            spaces.Add(space);
        //            position += distance;
        //        }
				
        //        if (range.Terminator != null)
        //        {
        //            var rangeBlock = groups.SingleOrDefault(p => p.Key == range.Start);
        //            var newSpaces = new List<Space>();

        //            position += range.Length;
					
        //            var method = GetMethodToInvoke(range.Type);
        //            position = (int)method.Invoke(this, new object[] { template, rangeBlock, position, groups, newSpaces });

        //            var complexNegative = _complexNegativeFactory.Create<TModelContext>(range.Type, (Expression<Func<TModelContext, object>>)range.Contents, newSpaces);
        //            spaces.Add(complexNegative);
        //        }
        //        else if (!range.IsTerminator)
        //        {
        //            var newspace = new SimpleNegative<TModelContext>((Expression<Func<TModelContext, object>>)range.Contents);
        //            spaces.Add(newspace);
        //        }

        //        position = Math.Max(range.Finish, position);
        //    }
        //    return position;
        //}

        //private MethodInfo GetMethodToInvoke(Type type)
        //{
        //    var methodInfo = GetType().GetMethod("ProcessRanges").MakeGenericMethod(type);
        //    return methodInfo;
        //}

        //private void OrganizeRanges(IEnumerable<Range> rangesToProcess)
        //{
        //    var ranges = rangesToProcess.OrderBy(p => p.Start);

        //    foreach (var range in ranges)
        //    {

        //    }
        //}
	}
}
