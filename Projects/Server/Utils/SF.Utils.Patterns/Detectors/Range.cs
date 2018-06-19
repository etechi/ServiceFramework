using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Utils.Patterns
{
	 struct Range
	{
		public static readonly Range Empty = default(Range);
		public int Begin;
		public int Length;
		public Range(int begin, int length)
		{
			this.Begin = begin;
			this.Length = length;
		}
		public int End { get { return Begin + Length; } }
	}
	 static class RangeExtension
	{
		public static bool IsEmpty(this Range r)
		{
			return r.Length == 0;
		}
		public static Range Union(this Range r, Range d)
		{
			if (d.IsEmpty())
				return r;
			if (r.IsEmpty())
				return d;
			var b = Math.Min(r.Begin, d.Begin);
			var e = Math.Max(r.End, d.End);
			return new Range(b, e - b);
		}
		public static Range Union(this Range r, int point)
		{
			if (r.IsEmpty())
				return new Range(point, 1);
			else if (point < r.Begin)
				return new Range(point, r.End - point);
			else if (point >= r.End)
				return new Range(r.Begin, point - r.Begin + 1);
			else
				return r;
		}
		public static bool Contains(this Range r, Range d)
		{
			if (r.IsEmpty())
				return false;
			if (d.IsEmpty())
				return true;
			return r.Begin <= d.Begin && r.End >= d.End;
		}

	}
}
