﻿#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys
{
	public static class ArrayExtension
	{
		public static T[] Concat<T>(this T[] first, params T[][] tails) =>
			Concat(first, (IEnumerable<T[]>)tails);

		public static T[] Concat<T>(this T[] first,IEnumerable<T[]> tails)
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
			if (data == null) throw new ArgumentNullException(nameof(data));
			if(offset<0)throw new ArgumentOutOfRangeException(nameof(offset));

			if (length == 0) length = data.Length - offset;

			if (length<0 || length>data.Length - offset)throw new ArgumentOutOfRangeException(nameof(length));

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