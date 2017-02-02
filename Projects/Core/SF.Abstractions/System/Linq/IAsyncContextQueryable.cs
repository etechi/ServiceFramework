using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Threading;

namespace System.Linq
{
	public interface IAsyncQueryableProvider
	{
		Task<bool> AllAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate);
		Task<bool> AllAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken);
		Task<bool> AnyAsync<TSource>(IContextQueryable<TSource> source);
		Task<bool> AnyAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate);
		Task<bool> AnyAsync<TSource>(IContextQueryable<TSource> source, CancellationToken cancellationToken);
		Task<bool> AnyAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken);
		Task<double?> AverageAsync(IContextQueryable<long?> source);
		Task<float> AverageAsync(IContextQueryable<float> source);
		Task<double> AverageAsync(IContextQueryable<double> source);
		Task<double?> AverageAsync(IContextQueryable<double?> source);
		Task<decimal> AverageAsync(IContextQueryable<decimal> source);
		Task<decimal?> AverageAsync(IContextQueryable<decimal?> source);
		Task<double> AverageAsync(IContextQueryable<long> source);
		Task<double?> AverageAsync(IContextQueryable<int?> source);
		Task<float?> AverageAsync(IContextQueryable<float?> source);
		Task<double> AverageAsync(IContextQueryable<int> source);
		Task<decimal?> AverageAsync(IContextQueryable<decimal?> source, CancellationToken cancellationToken);
		Task<double?> AverageAsync(IContextQueryable<int?> source, CancellationToken cancellationToken);
		Task<float> AverageAsync(IContextQueryable<float> source, CancellationToken cancellationToken);
		Task<float?> AverageAsync(IContextQueryable<float?> source, CancellationToken cancellationToken);
		Task<double?> AverageAsync(IContextQueryable<double?> source, CancellationToken cancellationToken);
		Task<decimal> AverageAsync(IContextQueryable<decimal> source, CancellationToken cancellationToken);
		Task<double> AverageAsync(IContextQueryable<long> source, CancellationToken cancellationToken);
		Task<double?> AverageAsync(IContextQueryable<long?> source, CancellationToken cancellationToken);
		Task<double> AverageAsync(IContextQueryable<int> source, CancellationToken cancellationToken);
		Task<double> AverageAsync(IContextQueryable<double> source, CancellationToken cancellationToken);
		Task<double> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, int>> selector);
		Task<float> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, float>> selector);
		Task<double?> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, int?>> selector);
		Task<double> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, long>> selector);
		Task<float?> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, float?>> selector);
		Task<decimal?> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector);
		Task<decimal> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, decimal>> selector);
		Task<double> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, double>> selector);
		Task<double?> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, long?>> selector);
		Task<double?> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, double?>> selector);
		Task<double> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken);
		Task<decimal?> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken);
		Task<double> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken);
		Task<double?> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken);
		Task<decimal> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken);
		Task<double?> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken);
		Task<double> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken);
		Task<float?> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken);
		Task<float> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken);
		Task<double?> AverageAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken);
		Task<bool> ContainsAsync<TSource>(IContextQueryable<TSource> source, TSource item);
		Task<bool> ContainsAsync<TSource>(IContextQueryable<TSource> source, TSource item, CancellationToken cancellationToken);
		Task<int> CountAsync<TSource>(IContextQueryable<TSource> source);
		Task<int> CountAsync<TSource>(IContextQueryable<TSource> source, CancellationToken cancellationToken);
		Task<int> CountAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate);
		Task<int> CountAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken);
		Task<TSource> FirstAsync<TSource>(IContextQueryable<TSource> source);
		Task<TSource> FirstAsync<TSource>(IContextQueryable<TSource> source, CancellationToken cancellationToken);
		Task<TSource> FirstAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate);
		Task<TSource> FirstAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken);
		Task<TSource> FirstOrDefaultAsync<TSource>(IContextQueryable<TSource> source);
		Task<TSource> FirstOrDefaultAsync<TSource>(IContextQueryable<TSource> source, CancellationToken cancellationToken);
		Task<TSource> FirstOrDefaultAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate);
		Task<TSource> FirstOrDefaultAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken);
		Task ForEachAsync<T>(IContextQueryable<T> source, Action<T> action);
		Task ForEachAsync<T>(IContextQueryable<T> source, Action<T> action, CancellationToken cancellationToken);
		Task<long> LongCountAsync<TSource>(IContextQueryable<TSource> source);
		Task<long> LongCountAsync<TSource>(IContextQueryable<TSource> source, CancellationToken cancellationToken);
		Task<long> LongCountAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate);
		Task<long> LongCountAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken);
		Task<TSource> MaxAsync<TSource>(IContextQueryable<TSource> source);
		Task<TSource> MaxAsync<TSource>(IContextQueryable<TSource> source, CancellationToken cancellationToken);
		Task<TResult> MaxAsync<TSource, TResult>(IContextQueryable<TSource> source, Expression<Func<TSource, TResult>> selector);
		Task<TResult> MaxAsync<TSource, TResult>(IContextQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken);
		Task<TSource> MinAsync<TSource>(IContextQueryable<TSource> source);
		Task<TSource> MinAsync<TSource>(IContextQueryable<TSource> source, CancellationToken cancellationToken);
		Task<TResult> MinAsync<TSource, TResult>(IContextQueryable<TSource> source, Expression<Func<TSource, TResult>> selector);
		Task<TResult> MinAsync<TSource, TResult>(IContextQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken);
		Task<TSource> SingleAsync<TSource>(IContextQueryable<TSource> source);
		Task<TSource> SingleAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate);
		Task<TSource> SingleAsync<TSource>(IContextQueryable<TSource> source, CancellationToken cancellationToken);
		Task<TSource> SingleAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken);
		Task<TSource> SingleOrDefaultAsync<TSource>(IContextQueryable<TSource> source);
		Task<TSource> SingleOrDefaultAsync<TSource>(IContextQueryable<TSource> source, CancellationToken cancellationToken);
		Task<TSource> SingleOrDefaultAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate);
		Task<TSource> SingleOrDefaultAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken);
		IContextQueryable<TSource> Skip<TSource>(IContextQueryable<TSource> source, Expression<Func<int>> countAccessor);
		Task<long?> SumAsync(IContextQueryable<long?> source);
		Task<double> SumAsync(IContextQueryable<double> source);
		Task<int> SumAsync(IContextQueryable<int> source);
		Task<int?> SumAsync(IContextQueryable<int?> source);
		Task<decimal> SumAsync(IContextQueryable<decimal> source);
		Task<long> SumAsync(IContextQueryable<long> source);
		Task<double?> SumAsync(IContextQueryable<double?> source);
		Task<float> SumAsync(IContextQueryable<float> source);
		Task<decimal?> SumAsync(IContextQueryable<decimal?> source);
		Task<float?> SumAsync(IContextQueryable<float?> source);
		Task<decimal> SumAsync(IContextQueryable<decimal> source, CancellationToken cancellationToken);
		Task<double?> SumAsync(IContextQueryable<double?> source, CancellationToken cancellationToken);
		Task<double> SumAsync(IContextQueryable<double> source, CancellationToken cancellationToken);
		Task<float> SumAsync(IContextQueryable<float> source, CancellationToken cancellationToken);
		Task<long?> SumAsync(IContextQueryable<long?> source, CancellationToken cancellationToken);
		Task<long> SumAsync(IContextQueryable<long> source, CancellationToken cancellationToken);
		Task<int?> SumAsync(IContextQueryable<int?> source, CancellationToken cancellationToken);
		Task<int> SumAsync(IContextQueryable<int> source, CancellationToken cancellationToken);
		Task<float?> SumAsync(IContextQueryable<float?> source, CancellationToken cancellationToken);
		Task<decimal?> SumAsync(IContextQueryable<decimal?> source, CancellationToken cancellationToken);
		Task<float> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, float>> selector);
		Task<float?> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, float?>> selector);
		Task<int?> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, int?>> selector);
		Task<long> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, long>> selector);
		Task<int> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, int>> selector);
		Task<long?> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, long?>> selector);
		Task<double> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, double>> selector);
		Task<decimal?> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector);
		Task<decimal> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, decimal>> selector);
		Task<double?> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, double?>> selector);
		Task<float> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken);
		Task<float?> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken);
		Task<double> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken);
		Task<int> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken);
		Task<decimal> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken);
		Task<decimal?> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken);
		Task<long?> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken);
		Task<long> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken);
		Task<int?> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken);
		Task<double?> SumAsync<TSource>(IContextQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken);
		IContextQueryable<TSource> Take<TSource>(IContextQueryable<TSource> source, Expression<Func<int>> countAccessor);
		Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(IContextQueryable<TSource> source, Func<TSource, TKey> keySelector);
		Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(IContextQueryable<TSource> source, Func<TSource, TKey> keySelector, CancellationToken cancellationToken);
		Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(IContextQueryable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer);
		Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(IContextQueryable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken);
		Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(IContextQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector);
		Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(IContextQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, CancellationToken cancellationToken);
		Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(IContextQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer);
		Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(IContextQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken);
		Task<List<TSource>> ToListAsync<TSource>(IContextQueryable<TSource> source);
		Task<List<TSource>> ToListAsync<TSource>(IContextQueryable<TSource> source, CancellationToken cancellationToken);
		Task<TSource[]> ToArrayAsync<TSource>(IContextQueryable<TSource> source);
		Task<TSource[]> ToArrayAsync<TSource>(IContextQueryable<TSource> source, CancellationToken cancellationToken);
	}

	public interface IAsyncQueryableContext
	{
		IAsyncQueryableProvider AsyncQueryableProvider { get; }
	}

	public static class AsyncContextQueryable
	{
		public static IAsyncQueryableProvider AsyncQueryableProvider<T>(this IContextQueryable<T> Query)
		{
			if (Query == null)
				throw new ArgumentNullException();
			var provider = (Query.Context as IAsyncQueryableContext)?.AsyncQueryableProvider;
			if (provider == null)
				throw new NotSupportedException("不支持IAsyncQueryableProvider 接口");
			return provider;
		}
		public static Task<bool> AllAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
			=> source.AsyncQueryableProvider().AllAsync(source, predicate);

		public static Task<bool> AllAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AllAsync(source, predicate, cancellationToken);

		public static Task<bool> AnyAsync<TSource>(this IContextQueryable<TSource> source)
			=> source.AsyncQueryableProvider().AnyAsync(source);

		public static Task<bool> AnyAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
			=> source.AsyncQueryableProvider().AnyAsync(source, predicate);

		public static Task<bool> AnyAsync<TSource>(this IContextQueryable<TSource> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AnyAsync(source, cancellationToken);

		public static Task<bool> AnyAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AnyAsync(source, predicate, cancellationToken);

		public static Task<double?> AverageAsync(this IContextQueryable<long?> source)
			=> source.AsyncQueryableProvider().AverageAsync(source);

		public static Task<float> AverageAsync(this IContextQueryable<float> source)
			=> source.AsyncQueryableProvider().AverageAsync(source);
		public static Task<double> AverageAsync(this IContextQueryable<double> source)
			=> source.AsyncQueryableProvider().AverageAsync(source);
		public static Task<double?> AverageAsync(this IContextQueryable<double?> source)
			=> source.AsyncQueryableProvider().AverageAsync(source);
		public static Task<decimal> AverageAsync(this IContextQueryable<decimal> source)
			=> source.AsyncQueryableProvider().AverageAsync(source);
		public static Task<decimal?> AverageAsync(this IContextQueryable<decimal?> source)
			=> source.AsyncQueryableProvider().AverageAsync(source);
		public static Task<double> AverageAsync(this IContextQueryable<long> source)
			=> source.AsyncQueryableProvider().AverageAsync(source);
		public static Task<double?> AverageAsync(this IContextQueryable<int?> source)
			=> source.AsyncQueryableProvider().AverageAsync(source);
		public static Task<float?> AverageAsync(this IContextQueryable<float?> source)
			=> source.AsyncQueryableProvider().AverageAsync(source);
		public static Task<double> AverageAsync(this IContextQueryable<int> source)
			=> source.AsyncQueryableProvider().AverageAsync(source);
		public static Task<decimal?> AverageAsync(this IContextQueryable<decimal?> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AverageAsync(source, cancellationToken);
		public static Task<double?> AverageAsync(this IContextQueryable<int?> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AverageAsync(source, cancellationToken);
		public static Task<float> AverageAsync(this IContextQueryable<float> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AverageAsync(source, cancellationToken);
		public static Task<float?> AverageAsync(this IContextQueryable<float?> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AverageAsync(source, cancellationToken);
		public static Task<double?> AverageAsync(this IContextQueryable<double?> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AverageAsync(source, cancellationToken);
		public static Task<decimal> AverageAsync(this IContextQueryable<decimal> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AverageAsync(source, cancellationToken);
		public static Task<double> AverageAsync(this IContextQueryable<long> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AverageAsync(source, cancellationToken);
		public static Task<double?> AverageAsync(this IContextQueryable<long?> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AverageAsync(source, cancellationToken);
		public static Task<double> AverageAsync(this IContextQueryable<int> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AverageAsync(source, cancellationToken);
		public static Task<double> AverageAsync(this IContextQueryable<double> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AverageAsync(source, cancellationToken);
		public static Task<double> AverageAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, int>> selector)
			=> source.AsyncQueryableProvider().AverageAsync(source, selector);
		public static Task<float> AverageAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, float>> selector)
			=> source.AsyncQueryableProvider().AverageAsync(source, selector);
		public static Task<double?> AverageAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, int?>> selector)
			=> source.AsyncQueryableProvider().AverageAsync(source, selector);
		public static Task<double> AverageAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, long>> selector)
			=> source.AsyncQueryableProvider().AverageAsync(source, selector);
		public static Task<float?> AverageAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, float?>> selector)
			=> source.AsyncQueryableProvider().AverageAsync(source, selector);
		public static Task<decimal?> AverageAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector)
			=> source.AsyncQueryableProvider().AverageAsync(source, selector);
		public static Task<decimal> AverageAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
			=> source.AsyncQueryableProvider().AverageAsync(source, selector);
		public static Task<double> AverageAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, double>> selector)
			=> source.AsyncQueryableProvider().AverageAsync(source, selector);
		public static Task<double?> AverageAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, long?>> selector)
			=> source.AsyncQueryableProvider().AverageAsync(source, selector);
		public static Task<double?> AverageAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, double?>> selector)
			=> source.AsyncQueryableProvider().AverageAsync(source, selector);
		public static Task<double> AverageAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AverageAsync(source, selector, cancellationToken);
		public static Task<decimal?> AverageAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AverageAsync(source, selector, cancellationToken);
		public static Task<double> AverageAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AverageAsync(source, selector, cancellationToken);
		public static Task<double?> AverageAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AverageAsync(source, selector, cancellationToken);
		public static Task<decimal> AverageAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AverageAsync(source, selector, cancellationToken);
		public static Task<double?> AverageAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AverageAsync(source, selector, cancellationToken);
		public static Task<double> AverageAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AverageAsync(source, selector, cancellationToken);
		public static Task<float?> AverageAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AverageAsync(source, selector, cancellationToken);
		public static Task<float> AverageAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AverageAsync(source, selector, cancellationToken);
		public static Task<double?> AverageAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().AverageAsync(source, selector, cancellationToken);
		public static Task<bool> ContainsAsync<TSource>(this IContextQueryable<TSource> source, TSource item)
			=> source.AsyncQueryableProvider().ContainsAsync(source, item);
		public static Task<bool> ContainsAsync<TSource>(this IContextQueryable<TSource> source, TSource item, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().ContainsAsync(source, item, cancellationToken);
		public static Task<int> CountAsync<TSource>(this IContextQueryable<TSource> source)
			=> source.AsyncQueryableProvider().CountAsync(source);
		public static Task<int> CountAsync<TSource>(this IContextQueryable<TSource> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().CountAsync(source, cancellationToken);

		public static Task<int> CountAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
			=> source.AsyncQueryableProvider().CountAsync(source, predicate);

		public static Task<int> CountAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().CountAsync(source, predicate, cancellationToken);

		public static Task<TSource> FirstAsync<TSource>(this IContextQueryable<TSource> source)
			=> source.AsyncQueryableProvider().FirstAsync(source);

		public static Task<TSource> FirstAsync<TSource>(this IContextQueryable<TSource> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().FirstAsync(source, cancellationToken);
		public static Task<TSource> FirstAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
			=> source.AsyncQueryableProvider().FirstAsync(source, predicate);
		public static Task<TSource> FirstAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().FirstAsync(source, predicate, cancellationToken);

		public static Task<TSource> FirstOrDefaultAsync<TSource>(this IContextQueryable<TSource> source)
			=> source.AsyncQueryableProvider().FirstOrDefaultAsync(source);
		public static Task<TSource> FirstOrDefaultAsync<TSource>(this IContextQueryable<TSource> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().FirstOrDefaultAsync(source, cancellationToken);
		public static Task<TSource> FirstOrDefaultAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
			=> source.AsyncQueryableProvider().FirstOrDefaultAsync(source, predicate);
		public static Task<TSource> FirstOrDefaultAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().FirstOrDefaultAsync(source, predicate, cancellationToken);

		public static Task ForEachAsync<T>(this IContextQueryable<T> source, Action<T> action)
			=> source.AsyncQueryableProvider().ForEachAsync(source, action);

		public static Task ForEachAsync<T>(this IContextQueryable<T> source, Action<T> action, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().ForEachAsync(source, action, cancellationToken);

		public static Task<long> LongCountAsync<TSource>(this IContextQueryable<TSource> source)
			=> source.AsyncQueryableProvider().LongCountAsync(source);

		public static Task<long> LongCountAsync<TSource>(this IContextQueryable<TSource> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().LongCountAsync(source, cancellationToken);
		public static Task<long> LongCountAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
			=> source.AsyncQueryableProvider().LongCountAsync(source, predicate);
		public static Task<long> LongCountAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().LongCountAsync(source, predicate, cancellationToken);

		public static Task<TSource> MaxAsync<TSource>(this IContextQueryable<TSource> source)
			=> source.AsyncQueryableProvider().MaxAsync(source);
		public static Task<TSource> MaxAsync<TSource>(this IContextQueryable<TSource> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().MaxAsync(source, cancellationToken);
		public static Task<TResult> MaxAsync<TSource, TResult>(this IContextQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
			=> source.AsyncQueryableProvider().MaxAsync(source, selector);
		public static Task<TResult> MaxAsync<TSource, TResult>(this IContextQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().MaxAsync(source, selector, cancellationToken);

		public static Task<TSource> MinAsync<TSource>(this IContextQueryable<TSource> source)
			=> source.AsyncQueryableProvider().MinAsync(source);
		public static Task<TSource> MinAsync<TSource>(this IContextQueryable<TSource> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().MinAsync(source, cancellationToken);
		public static Task<TResult> MinAsync<TSource, TResult>(this IContextQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
			=> source.AsyncQueryableProvider().MinAsync(source, selector);
		public static Task<TResult> MinAsync<TSource, TResult>(this IContextQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().MinAsync(source, selector, cancellationToken);

		public static Task<TSource> SingleAsync<TSource>(this IContextQueryable<TSource> source)
			=> source.AsyncQueryableProvider().SingleAsync(source);

		public static Task<TSource> SingleAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
			=> source.AsyncQueryableProvider().SingleAsync(source, predicate);
		public static Task<TSource> SingleAsync<TSource>(this IContextQueryable<TSource> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SingleAsync(source, cancellationToken);
		public static Task<TSource> SingleAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SingleAsync(source, predicate, cancellationToken);
		public static Task<TSource> SingleOrDefaultAsync<TSource>(this IContextQueryable<TSource> source)
			=> source.AsyncQueryableProvider().SingleOrDefaultAsync(source);
		public static Task<TSource> SingleOrDefaultAsync<TSource>(this IContextQueryable<TSource> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SingleOrDefaultAsync(source, cancellationToken);
		public static Task<TSource> SingleOrDefaultAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
			=> source.AsyncQueryableProvider().SingleOrDefaultAsync(source, predicate);
		public static Task<TSource> SingleOrDefaultAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SingleOrDefaultAsync(source, predicate, cancellationToken);

		public static IContextQueryable<TSource> Skip<TSource>(this IContextQueryable<TSource> source, Expression<Func<int>> countAccessor)
			=> source.AsyncQueryableProvider().Skip(source, countAccessor);

		public static Task<long?> SumAsync(this IContextQueryable<long?> source)
			=> source.AsyncQueryableProvider().SumAsync(source);
		public static Task<double> SumAsync(this IContextQueryable<double> source)
			=> source.AsyncQueryableProvider().SumAsync(source);
		public static Task<int> SumAsync(this IContextQueryable<int> source)
			=> source.AsyncQueryableProvider().SumAsync(source);
		public static Task<int?> SumAsync(this IContextQueryable<int?> source)
			=> source.AsyncQueryableProvider().SumAsync(source);
		public static Task<decimal> SumAsync(this IContextQueryable<decimal> source)
			=> source.AsyncQueryableProvider().SumAsync(source);
		public static Task<long> SumAsync(this IContextQueryable<long> source)
			=> source.AsyncQueryableProvider().SumAsync(source);
		public static Task<double?> SumAsync(this IContextQueryable<double?> source)
			=> source.AsyncQueryableProvider().SumAsync(source);
		public static Task<float> SumAsync(this IContextQueryable<float> source)
			=> source.AsyncQueryableProvider().SumAsync(source);
		public static Task<decimal?> SumAsync(this IContextQueryable<decimal?> source)
			=> source.AsyncQueryableProvider().SumAsync(source);
		public static Task<float?> SumAsync(this IContextQueryable<float?> source)
			=> source.AsyncQueryableProvider().SumAsync(source);
		public static Task<decimal> SumAsync(this IContextQueryable<decimal> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SumAsync(source, cancellationToken);
		public static Task<double?> SumAsync(this IContextQueryable<double?> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SumAsync(source, cancellationToken);
		public static Task<double> SumAsync(this IContextQueryable<double> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SumAsync(source, cancellationToken);
		public static Task<float> SumAsync(this IContextQueryable<float> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SumAsync(source, cancellationToken);
		public static Task<long?> SumAsync(this IContextQueryable<long?> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SumAsync(source, cancellationToken);
		public static Task<long> SumAsync(this IContextQueryable<long> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SumAsync(source, cancellationToken);
		public static Task<int?> SumAsync(this IContextQueryable<int?> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SumAsync(source, cancellationToken);
		public static Task<int> SumAsync(this IContextQueryable<int> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SumAsync(source, cancellationToken);
		public static Task<float?> SumAsync(this IContextQueryable<float?> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SumAsync(source, cancellationToken);
		public static Task<decimal?> SumAsync(this IContextQueryable<decimal?> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SumAsync(source, cancellationToken);
		public static Task<float> SumAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, float>> selector)
			=> source.AsyncQueryableProvider().SumAsync(source, selector);
		public static Task<float?> SumAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, float?>> selector)
			=> source.AsyncQueryableProvider().SumAsync(source, selector);
		public static Task<int?> SumAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, int?>> selector)
			=> source.AsyncQueryableProvider().SumAsync(source, selector);
		public static Task<long> SumAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, long>> selector)
			=> source.AsyncQueryableProvider().SumAsync(source, selector);
		public static Task<int> SumAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, int>> selector)
			=> source.AsyncQueryableProvider().SumAsync(source, selector);
		public static Task<long?> SumAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, long?>> selector)
			=> source.AsyncQueryableProvider().SumAsync(source, selector);
		public static Task<double> SumAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, double>> selector)
			=> source.AsyncQueryableProvider().SumAsync(source, selector);
		public static Task<decimal?> SumAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector)
			=> source.AsyncQueryableProvider().SumAsync(source, selector);
		public static Task<decimal> SumAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
			=> source.AsyncQueryableProvider().SumAsync(source, selector);
		public static Task<double?> SumAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, double?>> selector)
			=> source.AsyncQueryableProvider().SumAsync(source, selector);
		public static Task<float> SumAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SumAsync(source, selector, cancellationToken);

		public static Task<float?> SumAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SumAsync(source, selector, cancellationToken);
		public static Task<double> SumAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SumAsync(source, selector, cancellationToken);
		public static Task<int> SumAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SumAsync(source, selector, cancellationToken);
		public static Task<decimal> SumAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SumAsync(source, selector, cancellationToken);
		public static Task<decimal?> SumAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SumAsync(source, selector, cancellationToken);
		public static Task<long?> SumAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SumAsync(source, selector, cancellationToken);
		public static Task<long> SumAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SumAsync(source, selector, cancellationToken);
		public static Task<int?> SumAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SumAsync(source, selector, cancellationToken);
		public static Task<double?> SumAsync<TSource>(this IContextQueryable<TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().SumAsync(source, selector, cancellationToken);

		public static IContextQueryable<TSource> Take<TSource>(this IContextQueryable<TSource> source, Expression<Func<int>> countAccessor)
			=> source.AsyncQueryableProvider().Take(source, countAccessor);

		public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this IContextQueryable<TSource> source, Func<TSource, TKey> keySelector)
			=> source.AsyncQueryableProvider().ToDictionaryAsync(source, keySelector);

		public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this IContextQueryable<TSource> source, Func<TSource, TKey> keySelector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().ToDictionaryAsync(source, keySelector, cancellationToken);
		public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this IContextQueryable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
			=> source.AsyncQueryableProvider().ToDictionaryAsync(source, keySelector, comparer);
		public static Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(this IContextQueryable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().ToDictionaryAsync(source, keySelector, comparer, cancellationToken);
		public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this IContextQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
			=> source.AsyncQueryableProvider().ToDictionaryAsync(source, keySelector, elementSelector);
		public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this IContextQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().ToDictionaryAsync(source, keySelector, elementSelector, cancellationToken);
		public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this IContextQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
			=> source.AsyncQueryableProvider().ToDictionaryAsync(source, keySelector, elementSelector, comparer);
		public static Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this IContextQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().ToDictionaryAsync(source, keySelector, elementSelector, comparer, cancellationToken);

		public static Task<List<TSource>> ToListAsync<TSource>(this IContextQueryable<TSource> source)
			=> source.AsyncQueryableProvider().ToListAsync(source);
		public static Task<List<TSource>> ToListAsync<TSource>(this IContextQueryable<TSource> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().ToListAsync(source, cancellationToken);

		public static Task<TSource[]> ToArrayAsync<TSource>(this IContextQueryable<TSource> source)
			=> source.AsyncQueryableProvider().ToArrayAsync(source);
		public static Task<TSource[]> ToArrayAsync<TSource>(this IContextQueryable<TSource> source, CancellationToken cancellationToken)
			=> source.AsyncQueryableProvider().ToArrayAsync(source, cancellationToken);
	}
}
