#region Apache License Version 2.0
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

using SF.Sys.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Linq
{
	public static class EnumerableEx
	{
		public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
		{
			foreach (var i in enumerable)
				return false;
			return true;
		}
		public static T At<T>(this IEnumerable<T> enumerable,int Index)
		{
			if (Index < 0)
				throw new IndexOutOfRangeException();
			foreach(var e in enumerable)
			{
				if (Index == 0)
					return e;
				Index--;
			}
			throw new IndexOutOfRangeException();
		}
		public static Queue<T> ToQueue<T>(this IEnumerable<T> enumerable)
			=> new Queue<T>(enumerable);

		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable)
			=> new HashSet<T>(enumerable);

		public static T GetByIndex<T>(this IEnumerable<T> enumerable, int Index)
		{
			return enumerable.Skip(Index).First();
		}
		public static T FindPeak<T>(this IEnumerable<T> enumerable, Func<T,T,int> Comparer) 
		{
			return enumerable.Aggregate((x, y) => Comparer(x, y) >= 0 ? x : y);
		} 
		public static int IndexOf<T>(this IEnumerable<T> enumerable, Func<T, bool> Predicate)
		{
			var idx = 0;
			foreach (var i in enumerable)
			{
				if (Predicate(i))
					return idx;
				idx++;
			}
			return -1;
		}
		public static int IndexOf<T>(this IEnumerable<T> enumerable, T Item) where T : IEquatable<T>
		{
			var idx = 0;
			foreach (var i in enumerable)
			{
				if (i.Equals(Item))
					return idx;
				idx++;
			}
			return -1;
		}
		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> Action)
		{
			foreach (var it in enumerable)
				Action(it);
		}
		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> Action)
		{
			var idx = 0;
			foreach (var it in enumerable)
				Action(it, idx++);
		}
		static Task<T> Async<T>(Func<Task<T>> func)
		{
			return func();
		}
		public static async Task<IEnumerable<T>> AsyncSelect<I, T>(
			this IEnumerable<I> items,
			Func<I, Task<T>> Selector
			)
		{
			return await Task.WhenAll(
				items.Select(pair => Selector(pair)).ToArray()
				);
		}
		public static IEnumerable<T> WithFirst<T>(this IEnumerable<T> items,params T[] firsts)
		{
			foreach(var f in firsts)
				yield return f;
			foreach (var i in items)
				yield return i;
		}
		public static IEnumerable<T> WithLast<T>(this IEnumerable<T> items, params T[] lasts)
		{
			foreach (var i in items)
				yield return i;
			foreach(var l in lasts)
				yield return l;
		}

		public static bool AllEquals<T>(this IEnumerable<T> items, IEnumerable<T> other) where T : class
			=> items.Zip(other, (l, r) => l==r).All(r => r);

		public static IEnumerable<T> From<T>(T value)
		{
			yield return value;
		}
		public static IEnumerable<T> From<T>(params T[] values)
		{
			return values;
		}
		public static IEnumerable<X> Merge<X, K>(
		   this IEnumerable<X> xs,
		   IEnumerable<X> ys,
		   Func<X, K> KeySelector,
		   Func<X, X, X> Merger
		   ) where K : IEquatable<K>
			=> Merge(xs,KeySelector, ys, KeySelector, Merger);

		public static IEnumerable<Z> Merge<X,Y,Z,K>(
			this IEnumerable<X> xs,
			Func<X,K> XKeySelector,
			IEnumerable<Y> ys,
			Func<Y, K> YKeySelector,
			Func<X,Y,Z> Merger
			) where K : IEquatable<K>
		{
			xs = xs ?? Enumerable.Empty<X>();
			ys = ys ?? Enumerable.Empty<Y>();
			var xdic = xs.ToDictionary(XKeySelector);
			var ydic = ys.ToDictionary(YKeySelector);
			return xs.Select(
				xi =>
					ydic.TryGetValue(XKeySelector(xi), out var yi)?
					Merger(xi, yi):
					Merger(xi,default(Y)
					)
				).Concat(
					ys
					.Where(yi=>!xdic.ContainsKey(YKeySelector(yi)))
					.Select(yi=>Merger(default(X),yi))
				);
		}
		public static string Join(this IEnumerable<string> Enumerable, string separator = "")
		{
			return string.Join(separator, Enumerable.Where(s=>!s.IsNullOrEmpty()));
		}
		public static string Join<T>(this IEnumerable<T> Enumerable, string separator = "")
		{
			return string.Join(separator, Enumerable);
		}
		public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> Enumerable)
		{
			var re = new Dictionary<K, V>();
			foreach (var p in Enumerable)
				re[p.Key] = p.Value;
			return re;
		}
		public static SortedDictionary<K, V> ToSortedDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> Enumerable)
		{
			var re = new SortedDictionary<K, V>();
			foreach (var p in Enumerable)
				re[p.Key] = p.Value;
			return re;
		}
		public static IEnumerable<string> Normalize(this IEnumerable<string> s)
		{
			return s
				.Select(i => i == null ? null : i.Trim())
				.Where(i => !string.IsNullOrEmpty(i));
		}
	}
}
