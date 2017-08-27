using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
	public static class IEnumerableExtension
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
	}
}
