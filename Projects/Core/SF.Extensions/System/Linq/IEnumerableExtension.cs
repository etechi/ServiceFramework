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

			public IEnumerator<T> GetEnumerator()=>Queryable.GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator()=>GetEnumerator();
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
	}
}
