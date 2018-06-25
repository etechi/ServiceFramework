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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections;

namespace System.Linq
{
	//public interface IQueryableContext
	//{
	//	IContextQueryable<T> CreateQueryable<T>(IQueryable<T> Query);
	//	IOrderedContextQueryable<T> CreateOrderedQueryable<T>(IOrderedQueryable<T> Query);
	//}
	//public interface IQueryable : IQueryable
	//{
	//	//IQueryable Queryable { get; }
	//	//IQueryableContext Context { get; }
	//}


	//public interface IQueryable<out T> : IQueryable, IQueryable<T>
	//{
	//	//new IQueryable<T> Queryable { get; }
	//}
	//public interface IOrderedContextQueryable<out T> : IContextQueryable<T>
	//{
	//	new IOrderedQueryable<T> Queryable { get; }
	//}
	//public class NullContextQueryable<T> : IContextQueryable<T>, IQueryableContext
	//{
	//	public NullContextQueryable(IQueryable<T> Queryable)
	//	{
	//		this.Queryable = Queryable;
	//	}
	//	public IQueryableContext Context
	//	{
	//		get
	//		{
	//			return this;
	//		}
	//	}

	//	public IQueryable<T> Queryable
	//	{
	//		get;
	//	}

	//	IQueryable IContextQueryable.Queryable
	//	{
	//		get
	//		{
	//			return Queryable;
	//		}
	//	}

	//	public IEnumerator<T> GetEnumerator()
	//	{
	//		return Queryable.GetEnumerator();
	//	}

	//	IOrderedContextQueryable<T1> IQueryableContext.CreateOrderedQueryable<T1>(IOrderedQueryable<T1> Query)
	//	{
	//		return new OrderedNullContextQueryable<T1>(Query);
	//	}

	//	IContextQueryable<T1> IQueryableContext.CreateQueryable<T1>(IQueryable<T1> Query)
	//	{
	//		return new NullContextQueryable<T1>(Query);
	//	}

	//	IEnumerator IEnumerable.GetEnumerator()
	//	{
	//		return Queryable.GetEnumerator();
	//	}
	//}
	//public class OrderedNullContextQueryable<T> : NullContextQueryable<T>, IOrderedContextQueryable<T>
	//{
	//	public OrderedNullContextQueryable(IQueryable<T> Queryable) : base(Queryable)
	//	{
	//	}

	//	IOrderedQueryable<T> IOrderedContextQueryable<T>.Queryable
	//	{
	//		get
	//		{
	//			return (IOrderedQueryable<T>)Queryable;
	//		}
	//	}
	//}
	//public static class ContextQueryable
	//{

	//	public static IContextQueryable<O> New<I, O>(this IContextQueryable<I> Queryable, IQueryable<O> NewQueryable)
	//	{
	//		return Queryable.Context.CreateQueryable(NewQueryable);
	//	}
	//	public static IOrderedContextQueryable<O> NewOrdered<I, O>(this IContextQueryable<I> Queryable, IOrderedQueryable<O> NewQueryable)
	//	{
	//		return Queryable.Context.CreateOrderedQueryable(NewQueryable);
	//	}
	//	//public static IContextQueryable< I> New< I>(this IContextQueryable< I> Queryable, IQueryable<I> NewQueryable)
	//	//{
	//	//	return new ContextQueryable< I>(Queryable.Context, NewQueryable);
	//	//}

	//	public static IContextQueryable<T> AsQueryable<T>(this IEnumerable<T> Enumerable, IQueryableContext Context)
	//		=> Context.CreateQueryable(Enumerable.AsQueryable<T>());

	//	static IEnumerable<K> TryResolve<K>(this IEnumerable<K> e)
	//	{
	//		var cq = e as IContextQueryable<K>;
	//		return cq?.Queryable;

	//	}

	//	public static TSource Aggregate<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, TSource, TSource>> func)
	//		=> source.Queryable.Aggregate(func);
	//	public static TAccumulate Aggregate<TSource, TAccumulate>(this IContextQueryable<TSource> source, TAccumulate seed, Expression<Func<TAccumulate, TSource, TAccumulate>> func)
	//		=> source.Queryable.Aggregate(seed, func);
	//	public static TResult Aggregate<TSource, TAccumulate, TResult>(this IContextQueryable<TSource> source, TAccumulate seed, Expression<Func<TAccumulate, TSource, TAccumulate>> func, Expression<Func<TAccumulate, TResult>> selector)
	//		=> source.Queryable.Aggregate(seed, func, selector);
	//	public static bool All<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
	//		=> source.Queryable.All(predicate);

	//	public static bool Any<TSource>(this IContextQueryable<TSource> source)
	//		=> source.Queryable.Any();
	//	public static bool Any<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
	//		=> source.Queryable.Any(predicate);


	//	public static double Average<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, int>> selector)
	//		=> source.Queryable.Average(selector);

	//	public static float Average<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, float>> selector)
	//		=> source.Queryable.Average(selector);

	//	public static float? Average<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, float?>> selector)
	//		=> source.Queryable.Average(selector);

	//	public static double Average<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, long>> selector)
	//		=> source.Queryable.Average(selector);

	//	public static double? Average<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, long?>> selector)
	//		=> source.Queryable.Average(selector);

	//	public static double Average<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, double>> selector)
	//		=> source.Queryable.Average(selector);

	//	public static double? Average<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, double?>> selector)
	//		=> source.Queryable.Average(selector);

	//	public static decimal Average<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
	//		=> source.Queryable.Average(selector);

	//	public static decimal? Average<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector)
	//		=> source.Queryable.Average(selector);

	//	public static double? Average<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, int?>> selector)
	//		=> source.Queryable.Average(selector);

	//	public static IContextQueryable<TResult> Cast<TResult>(this IContextQueryable<TResult> source)
	//		=> source.Context.CreateQueryable(source.Queryable.Cast<TResult>());

	//	public static IContextQueryable<TSource> Concat<TSource>(this IContextQueryable<TSource> source1, IEnumerable<TSource> source2)
	//		=> source1.Context.CreateQueryable(source1.Queryable.Concat(source2.TryResolve()));

	//	public static bool Contains<TSource>(this IContextQueryable<TSource> source, TSource item)
	//		=> source.Queryable.Contains(item);

	//	public static bool Contains<TSource>(this IContextQueryable<TSource> source, TSource item, IEqualityComparer<TSource> comparer)
	//		=> source.Queryable.Contains(item, comparer);

	//	public static int Count<TSource>(this IContextQueryable<TSource> source)
	//		=> source.Queryable.Count();

	//	public static int Count<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
	//		=> source.Queryable.Count(predicate);

	//	public static IContextQueryable<TSource> DefaultIfEmpty<TSource>(this IContextQueryable<TSource> source)
	//		=> source.New(source.Queryable.DefaultIfEmpty());

	//	public static IContextQueryable<TSource> DefaultIfEmpty<TSource>(this IContextQueryable<TSource> source, TSource defaultValue)
	//		=> source.New(source.Queryable.DefaultIfEmpty(defaultValue));

	//	public static IContextQueryable<TSource> Distinct<TSource>(this IContextQueryable<TSource> source)
	//		=> source.New(source.Queryable.Distinct());

	//	public static IContextQueryable<TSource> Distinct<TSource>(this IContextQueryable<TSource> source, IEqualityComparer<TSource> comparer)
	//		=> source.New(source.Queryable.Distinct(comparer));

	//	public static TSource ElementAt<TSource>(this IContextQueryable<TSource> source, int index)
	//		=> source.Queryable.ElementAt(index);


	//	public static TSource ElementAtOrDefault<TSource>(this IContextQueryable<TSource> source, int index)
	//		=> source.Queryable.ElementAtOrDefault(index);

	//	public static IContextQueryable<TSource> Except<TSource>(this IContextQueryable<TSource> source1, IEnumerable<TSource> source2)
	//		=> source1.New(source1.Queryable.Except(source2.TryResolve()));

	//	public static IContextQueryable<TSource> Except<TSource>(this IContextQueryable<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer)
	//		=> source1.New(source1.Queryable.Except(source2.TryResolve(), comparer));

	//	public static TSource First<TSource>(this IContextQueryable<TSource> source)
	//		=> source.Queryable.First();

	//	public static TSource First<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
	//		=> source.Queryable.First(predicate);

	//	public static TSource FirstOrDefault<TSource>(this IContextQueryable<TSource> source)
	//		=> source.Queryable.FirstOrDefault();

	//	public static TSource FirstOrDefault<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
	//		=> source.Queryable.FirstOrDefault(predicate);

	//	public static IContextQueryable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IContextQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
	//		=> source.New(source.Queryable.GroupBy(keySelector));

	//	public static IContextQueryable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IContextQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, IEqualityComparer<TKey> comparer)
	//		=> source.New(source.Queryable.GroupBy(keySelector, comparer));

	//	public static IContextQueryable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IContextQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector)
	//		=> source.New(source.Queryable.GroupBy(keySelector, elementSelector));

	//	public static IContextQueryable<TResult> GroupBy<TSource, TKey, TResult>(this IContextQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TKey, IEnumerable<TSource>, TResult>> resultSelector)
	//		=> source.New(source.Queryable.GroupBy(keySelector, resultSelector));

	//	public static IContextQueryable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IContextQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector, IEqualityComparer<TKey> comparer)
	//		=> source.New(source.Queryable.GroupBy(keySelector, elementSelector, comparer));

	//	public static IContextQueryable<TResult> GroupBy<TSource, TKey, TResult>(this IContextQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TKey, IEnumerable<TSource>, TResult>> resultSelector, IEqualityComparer<TKey> comparer)
	//		=> source.New(source.Queryable.GroupBy(keySelector, resultSelector, comparer));

	//	public static IContextQueryable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IContextQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector, Expression<Func<TKey, IEnumerable<TElement>, TResult>> resultSelector)
	//		=> source.New(source.Queryable.GroupBy(keySelector, elementSelector, resultSelector));

	//	public static IContextQueryable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IContextQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector, Expression<Func<TKey, IEnumerable<TElement>, TResult>> resultSelector, IEqualityComparer<TKey> comparer)
	//		=> source.New(source.Queryable.GroupBy(keySelector, elementSelector, resultSelector, comparer));

	//	public static IContextQueryable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IContextQueryable<TOuter> outer, IEnumerable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, IEnumerable<TInner>, TResult>> resultSelector)
	//		=> outer.New(outer.Queryable.GroupJoin(inner.TryResolve(), outerKeySelector, innerKeySelector, resultSelector));

	//	public static IContextQueryable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IContextQueryable<TOuter> outer, IEnumerable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, IEnumerable<TInner>, TResult>> resultSelector, IEqualityComparer<TKey> comparer)
	//		=> outer.New(outer.Queryable.GroupJoin(inner.TryResolve(), outerKeySelector, innerKeySelector, resultSelector, comparer));

	//	public static IContextQueryable<TSource> Intersect<TSource>(this IContextQueryable<TSource> source1, IEnumerable<TSource> source2)
	//		=> source1.New(source1.Queryable.Intersect(source2.TryResolve()));

	//	public static IContextQueryable<TSource> Intersect<TSource>(this IContextQueryable<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer)
	//		=> source1.New(source1.Queryable.Intersect(source2.TryResolve(), comparer));

	//	public static IContextQueryable<TResult> Join<TOuter, TInner, TKey, TResult>(this IContextQueryable<TOuter> outer, IEnumerable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, TResult>> resultSelector)
	//		=> outer.New(outer.Queryable.Join(inner.TryResolve(), outerKeySelector, innerKeySelector, resultSelector));

	//	public static IContextQueryable<TResult> Join<TOuter, TInner, TKey, TResult>(this IContextQueryable<TOuter> outer, IEnumerable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, TResult>> resultSelector, IEqualityComparer<TKey> comparer)
	//		=> outer.New(outer.Queryable.Join(inner.TryResolve(), outerKeySelector, innerKeySelector, resultSelector, comparer));

	//	public static TSource Last<TSource>(this IContextQueryable<TSource> source)
	//		=> source.Queryable.Last();

	//	public static TSource Last<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
	//		=> source.Queryable.Last(predicate);

	//	public static TSource LastOrDefault<TSource>(this IContextQueryable<TSource> source)
	//		=> source.Queryable.LastOrDefault();

	//	public static TSource LastOrDefault<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
	//		=> source.Queryable.LastOrDefault(predicate);

	//	public static long LongCount<TSource>(this IContextQueryable<TSource> source)
	//		=> source.Queryable.LongCount();
	//	public static long LongCount<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
	//		=> source.Queryable.LongCount(predicate);
	//	public static TSource Max<TSource>(this IContextQueryable<TSource> source)
	//		=> source.Queryable.Max();
	//	public static TResult Max<TSource, TResult>(this IContextQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
	//		=> source.Queryable.Max(selector);
	//	public static TSource Min<TSource>(this IContextQueryable<TSource> source)
	//		=> source.Queryable.Min();
	//	public static TResult Min<TSource, TResult>(this IContextQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
	//		=> source.Queryable.Min(selector);

	//	public static IContextQueryable<TResult> OfType<TResult>(this IContextQueryable<TResult> source)
	//		=> source.Context.CreateQueryable(source.Queryable.OfType<TResult>());

	//	public static IOrderedContextQueryable<TSource> OrderBy<TSource, TKey>(this IContextQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
	//		=> source.NewOrdered(source.Queryable.OrderBy(keySelector));

	//	public static IOrderedContextQueryable<TSource> OrderBy<TSource, TKey>(this IContextQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, IComparer<TKey> comparer)
	//		=> source.NewOrdered(source.Queryable.OrderBy(keySelector, comparer));

	//	public static IOrderedContextQueryable<TSource> OrderByDescending<TSource, TKey>(this IContextQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
	//			=> source.NewOrdered(source.Queryable.OrderByDescending(keySelector));

	//	public static IOrderedContextQueryable<TSource> OrderByDescending<TSource, TKey>(this IContextQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, IComparer<TKey> comparer)
	//			=> source.NewOrdered(source.Queryable.OrderByDescending(keySelector, comparer));

	//	public static IContextQueryable<TSource> Reverse<TSource>(this IContextQueryable<TSource> source)
	//			=> source.New(source.Queryable.Reverse());

	//	public static IContextQueryable<TResult> Select<TSource, TResult>(this IContextQueryable<TSource> source, Expression<Func<TSource, int, TResult>> selector)
	//		=> source.New(source.Queryable.Select(selector));

	//	public static IContextQueryable<TResult> Select<TSource, TResult>(this IContextQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
	//		=> source.New(source.Queryable.Select(selector));

	//	public static IContextQueryable<TResult> SelectMany<TSource, TResult>(this IContextQueryable<TSource> source, Expression<Func<TSource, int, IEnumerable<TResult>>> selector)
	//		=> source.New(source.Queryable.SelectMany(selector));
	//	public static IContextQueryable<TResult> SelectMany<TSource, TResult>(this IContextQueryable<TSource> source, Expression<Func<TSource, IEnumerable<TResult>>> selector)
	//		=> source.New(source.Queryable.SelectMany(selector));
	//	public static IContextQueryable<TResult> SelectMany<TSource, TCollection, TResult>(this IContextQueryable<TSource> source, Expression<Func<TSource, int, IEnumerable<TCollection>>> collectionSelector, Expression<Func<TSource, TCollection, TResult>> resultSelector)
	//		=> source.New(source.Queryable.SelectMany(collectionSelector, resultSelector));

	//	public static IContextQueryable<TResult> SelectMany<TSource, TCollection, TResult>(this IContextQueryable<TSource> source, Expression<Func<TSource, IEnumerable<TCollection>>> collectionSelector, Expression<Func<TSource, TCollection, TResult>> resultSelector)
	//		=> source.New(source.Queryable.SelectMany(collectionSelector, resultSelector));

	//	public static bool SequenceEqual<TSource>(this IContextQueryable<TSource> source1, IEnumerable<TSource> source2)
	//		=> source1.Queryable.SequenceEqual(source2.TryResolve());

	//	public static bool SequenceEqual<TSource>(this IContextQueryable<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer)
	//		=> source1.Queryable.SequenceEqual(source2.TryResolve(), comparer);

	//	public static TSource Single<TSource>(this IContextQueryable<TSource> source)
	//		=> source.Queryable.Single();

	//	public static TSource Single<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
	//		=> source.Queryable.Single(predicate);

	//	public static TSource SingleOrDefault<TSource>(this IContextQueryable<TSource> source)
	//		=> source.Queryable.SingleOrDefault();

	//	public static TSource SingleOrDefault<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
	//		=> source.Queryable.SingleOrDefault(predicate);

	//	public static IContextQueryable<TSource> Skip<TSource>(this IContextQueryable<TSource> source, int count)
	//		=> source.New(source.Queryable.Skip(count));

	//	public static IContextQueryable<TSource> SkipWhile<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, int, bool>> predicate)
	//		=> source.New(source.Queryable.SkipWhile(predicate));

	//	public static IContextQueryable<TSource> SkipWhile<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
	//		=> source.New(source.Queryable.SkipWhile(predicate));

	//	public static int Sum<CTX>(this IContextQueryable<int> source)
	//		=> source.Queryable.Sum();

	//	public static int? Sum<CTX>(this IContextQueryable<int?> source)
	//		=> source.Queryable.Sum();
	//	public static long Sum<CTX>(this IContextQueryable<long> source)
	//		=> source.Queryable.Sum();
	//	public static long? Sum<CTX>(this IContextQueryable<long?> source)
	//		=> source.Queryable.Sum();
	//	public static float Sum<CTX>(this IContextQueryable<float> source)
	//		=> source.Queryable.Sum();
	//	public static float? Sum<CTX>(this IContextQueryable<float?> source)
	//		=> source.Queryable.Sum();
	//	public static double Sum<CTX>(this IContextQueryable<double> source)
	//		=> source.Queryable.Sum();
	//	public static double? Sum<CTX>(this IContextQueryable<double?> source)
	//		=> source.Queryable.Sum();
	//	public static decimal Sum<CTX>(this IContextQueryable<decimal> source)
	//		=> source.Queryable.Sum();
	//	public static decimal? Sum<CTX>(this IContextQueryable<decimal?> source)
	//		=> source.Queryable.Sum();

	//	public static int? Sum<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, int?>> selector)
	//		=> source.Queryable.Sum(selector);
	//	public static long Sum<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, long>> selector)
	//		=> source.Queryable.Sum(selector);
	//	public static double Sum<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, double>> selector)
	//		=> source.Queryable.Sum(selector);
	//	public static long? Sum<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, long?>> selector)
	//		=> source.Queryable.Sum(selector);
	//	public static float Sum<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, float>> selector)
	//		=> source.Queryable.Sum(selector);
	//	public static int Sum<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, int>> selector)
	//		=> source.Queryable.Sum(selector);
	//	public static float? Sum<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, float?>> selector)
	//		=> source.Queryable.Sum(selector);
	//	public static decimal? Sum<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector)
	//		=> source.Queryable.Sum(selector);
	//	public static decimal Sum<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
	//		=> source.Queryable.Sum(selector);
	//	public static double? Sum<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, double?>> selector)
	//		=> source.Queryable.Sum(selector);

	//	public static IContextQueryable<TSource> Take<TSource>(this IContextQueryable<TSource> source, int count)
	//		=> source.New(source.Queryable.Take(count));

	//	public static IContextQueryable<TSource> TakeWhile<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
	//		=> source.New(source.Queryable.TakeWhile(predicate));

	//	public static IContextQueryable<TSource> TakeWhile<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, int, bool>> predicate)
	//		=> source.New(source.Queryable.TakeWhile(predicate));

	//	public static IOrderedContextQueryable<TSource> ThenBy<TSource, TKey>(this IOrderedContextQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
	//		=> source.NewOrdered(source.Queryable.OrderBy(keySelector));

	//	public static IOrderedContextQueryable<TSource> ThenBy<TSource, TKey>(this IOrderedContextQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, IComparer<TKey> comparer)
	//		=> source.NewOrdered(source.Queryable.OrderBy(keySelector, comparer));
	//	public static IOrderedContextQueryable<TSource> ThenByDescending<TSource, TKey>(this IOrderedContextQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
	//		=> source.NewOrdered(source.Queryable.OrderByDescending(keySelector));
	//	public static IOrderedContextQueryable<TSource> ThenByDescending<TSource, TKey>(this IOrderedContextQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, IComparer<TKey> comparer)
	//		=> source.NewOrdered(source.Queryable.OrderByDescending(keySelector, comparer));

	//	public static IContextQueryable<TSource> Union<TSource>(this IContextQueryable<TSource> source1, IEnumerable<TSource> source2)
	//		=> source1.New(source1.Queryable.Union(source2.TryResolve()));

	//	public static IContextQueryable<TSource> Union<TSource>(this IContextQueryable<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer)
	//		=> source1.New(source1.Queryable.Union(source2.TryResolve(), comparer));
	//	public static IContextQueryable<TSource> Where<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, int, bool>> predicate)
	//		=> source.New(source.Queryable.Where(predicate));
	//	public static IContextQueryable<TSource> Where<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
	//		=> source.New(source.Queryable.Where(predicate));

	//	public static IContextQueryable<TResult> Zip<TFirst, TSecond, TResult>(this IContextQueryable<TFirst> source1, IEnumerable<TSecond> source2, Expression<Func<TFirst, TSecond, TResult>> resultSelector)
	//		=> source1.New(source1.Queryable.Zip(source2.TryResolve(), resultSelector));

	//}
}
