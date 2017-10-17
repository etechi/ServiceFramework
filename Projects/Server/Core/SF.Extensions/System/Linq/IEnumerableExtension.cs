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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
	public static class EnumerableEx
	{
		class SimpleContextQueryable<T> : IContextQueryable<T>, IOrderedContextQueryable<T>
		{
			public IQueryableContext Context { get; set; }

			public IQueryable<T> Queryable { get; set; }

			IOrderedQueryable<T> IOrderedContextQueryable<T>.Queryable => (IOrderedQueryable<T>)Queryable;

			IQueryable IContextQueryable.Queryable => Queryable;

			public IEnumerator<T> GetEnumerator() => Queryable.GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}
		class SimpleQueryableContext : IQueryableContext
		{
			public static SimpleQueryableContext Instance { get; } = new SimpleQueryableContext();
			public IOrderedContextQueryable<T> CreateOrderedQueryable<T>(IOrderedQueryable<T> Queryable)
			{
				return new SimpleContextQueryable<T> { Queryable = Queryable, Context = this };
			}

			public IContextQueryable<T> CreateQueryable<T>(IQueryable<T> Queryable)
			{
				return new SimpleContextQueryable<T> { Queryable = Queryable, Context = this };
			}
		}
		public static IContextQueryable<T> AsContextQueryable<T>(this IEnumerable<T> enumerable)
		{
			return new SimpleContextQueryable<T>
			{
				Queryable = enumerable.AsQueryable(),
				Context = SimpleQueryableContext.Instance
			};
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
		public static IEnumerable<T> WithFirst<T>(this IEnumerable<T> items,T first)
		{
			yield return first;
			foreach (var i in items)
				yield return i;
		}
		public static IEnumerable<T> WithLast<T>(this IEnumerable<T> items, T last)
		{
			foreach (var i in items)
				yield return i;
			yield return last;
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
			var xdic = (xs??Enumerable.Empty<X>()).ToDictionary(XKeySelector);
			var ydic = (ys??Enumerable.Empty<Y>()).ToDictionary(YKeySelector);
			return ydic.Select(p =>
				Merger(xdic.Get(p.Key), p.Value)
				).Concat(
				xdic
				.Where(x => !ydic.ContainsKey(x.Key))
				.Select(x => Merger(x.Value, default(Y)))
				);
		}
	}
}
