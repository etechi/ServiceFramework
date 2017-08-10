using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using SF;
namespace System
{
	public static class ArrayExtension
	{
		public static T[] Concat<T>(this T[] first,params T[][] tails)
		{
			var re = new T[first.Length + tails.Sum(t=>t?.Length ?? 0)];
			if (first.Length > 0)
				Array.Copy(first,0, re, 0, first.Length);
			var offset = first.Length;
			foreach (var t in tails)
				if ((t?.Length ?? 0) > 0)
				{
					Array.Copy(t, 0, re, offset, t.Length);
					offset += t.Length;
				}
			return re;
		}
		public static T[] Copy<T>(this T[] data, int offset, int length=0)
		{
			Ensure.NotNull(data, nameof(data));
			Ensure.ZeroOrPositive(offset, nameof(offset));
			if (length == 0) length = data.Length - offset;
			Ensure.Range(length, 0, data.Length - offset, nameof(length));
			var re = new T[length];
			Array.Copy(data, offset, re, 0, length);
			return re;
		}
		public static void Sort<T>(this T[] data,Comparison<T> comparison)
		{
			Array.Sort(data, comparison);
		}
		public static int[] Range(int begin, int count)
		{
			var re = new int[count];
			for (var i = 0; i < count; i++)
				re[i] = i + begin;
			return re;
		}
		public static int[] Range(int count)
		{
			return Range(0, count);
		}
        
        //查找左侧区域，比当前项小的值属于当前项区域
        public static int BinarySearchLeftRange<T>(this T[] data,T value)
        {
            var re = Array.BinarySearch(data, value);
            if (re >= 0)
                return re;
            return ~re;
        }
        
        //查找右侧区域，比当前项大的值属于当前项区域
        public static int BinarySearchRightRange<T>(this T[] data, T value)
        {
            var re = Array.BinarySearch(data, value);
            if (re >= 0)
                return re;
            return (~re) - 1;
        }
    }
}
