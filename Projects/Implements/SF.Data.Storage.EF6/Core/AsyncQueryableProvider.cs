using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity;
namespace SF.Data.Storage.EF6
{
	public class AsyncQueryableProvider : System.Linq.IAsyncQueryableProvider,IEntityQueryableProvider
	{
		AsyncQueryableProvider() { }
		public static AsyncQueryableProvider Instance { get; } = new AsyncQueryableProvider();

		public Task<bool> AllAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, bool>> predicate)
			=> source.Queryable.AllAsync(predicate);

		public Task<bool> AllAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
			=> source.Queryable.AllAsync(predicate,cancellationToken);

		public Task<bool> AnyAsync< TSource>(IContextQueryable< TSource> source)
			=> source.Queryable.AnyAsync();

		public Task<bool> AnyAsync< TSource>(IContextQueryable< TSource> source, CancellationToken cancellationToken)
			=> source.Queryable.AnyAsync(cancellationToken);

		public Task<bool> AnyAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, bool>> predicate)
			=> source.Queryable.AnyAsync(predicate);

		public Task<bool> AnyAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
			=> source.Queryable.AnyAsync(predicate,cancellationToken);


		public Task<decimal> AverageAsync(IContextQueryable< decimal> source)
			=> source.Queryable.AverageAsync();

		public Task<double> AverageAsync(IContextQueryable< long> source)
			=> source.Queryable.AverageAsync();

		public Task<double?> AverageAsync(IContextQueryable< int?> source)
			=> source.Queryable.AverageAsync();

		public Task<float?> AverageAsync(IContextQueryable< float?> source)
			=> source.Queryable.AverageAsync();

		public Task<double> AverageAsync(IContextQueryable< int> source)
			=> source.Queryable.AverageAsync();

		public Task<decimal?> AverageAsync(IContextQueryable< decimal?> source)
			=> source.Queryable.AverageAsync();

		public Task<double> AverageAsync(IContextQueryable< double> source)
			=> source.Queryable.AverageAsync();

		public Task<double?> AverageAsync(IContextQueryable< double?> source)
			=> source.Queryable.AverageAsync();

		public Task<float> AverageAsync(IContextQueryable< float> source)
			=> source.Queryable.AverageAsync();

		public Task<double?> AverageAsync(IContextQueryable< long?> source)
			=> source.Queryable.AverageAsync();

		public Task<double> AverageAsync(IContextQueryable< int> source, CancellationToken cancellationToken)
			=> source.Queryable.AverageAsync(cancellationToken);

		public Task<double> AverageAsync(IContextQueryable< double> source, CancellationToken cancellationToken)
			=> source.Queryable.AverageAsync(cancellationToken);

		public Task<double?> AverageAsync(IContextQueryable< long?> source, CancellationToken cancellationToken)
			=> source.Queryable.AverageAsync(cancellationToken);

		public Task<double?> AverageAsync(IContextQueryable< double?> source, CancellationToken cancellationToken)
			=> source.Queryable.AverageAsync(cancellationToken);

		public Task<decimal> AverageAsync(IContextQueryable< decimal> source, CancellationToken cancellationToken)
			=> source.Queryable.AverageAsync(cancellationToken);

		public Task<double> AverageAsync(IContextQueryable< long> source, CancellationToken cancellationToken)
			=> source.Queryable.AverageAsync(cancellationToken);

		public Task<float?> AverageAsync(IContextQueryable< float?> source, CancellationToken cancellationToken)
			=> source.Queryable.AverageAsync(cancellationToken);

		public Task<float> AverageAsync(IContextQueryable< float> source, CancellationToken cancellationToken)
			=> source.Queryable.AverageAsync(cancellationToken);

		public Task<double?> AverageAsync(IContextQueryable< int?> source, CancellationToken cancellationToken)
			=> source.Queryable.AverageAsync(cancellationToken);

		public Task<decimal?> AverageAsync(IContextQueryable< decimal?> source, CancellationToken cancellationToken)
			=> source.Queryable.AverageAsync(cancellationToken);

		public Task<decimal?> AverageAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, decimal?>> selector)
			=> source.Queryable.AverageAsync(selector);

		public Task<decimal> AverageAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, decimal>> selector)
			=> source.Queryable.AverageAsync(selector);

		public Task<double> AverageAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, double>> selector)
			=> source.Queryable.AverageAsync(selector);

		public Task<double?> AverageAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, long?>> selector)
			=> source.Queryable.AverageAsync(selector);

		public Task<double?> AverageAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, double?>> selector)
			=> source.Queryable.AverageAsync(selector);

		public Task<double> AverageAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, long>> selector)
			=> source.Queryable.AverageAsync(selector);

		public Task<float?> AverageAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, float?>> selector)
			=> source.Queryable.AverageAsync(selector);

		public Task<double?> AverageAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, int?>> selector)
			=> source.Queryable.AverageAsync(selector);

		public Task<float> AverageAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, float>> selector)
			=> source.Queryable.AverageAsync(selector);

		public Task<double> AverageAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, int>> selector)
			=> source.Queryable.AverageAsync(selector);

		public Task<double> AverageAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken)
			=> source.Queryable.AverageAsync(selector);

		public Task<float> AverageAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken)
			=> source.Queryable.AverageAsync(selector);

		public Task<double?> AverageAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken)
			=> source.Queryable.AverageAsync(selector);

		public Task<float?> AverageAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken)
			=> source.Queryable.AverageAsync(selector);

		public Task<double?> AverageAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken)
			=> source.Queryable.AverageAsync(selector);

		public Task<decimal> AverageAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken)
			=> source.Queryable.AverageAsync(selector);

		public Task<double?> AverageAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken)
			=> source.Queryable.AverageAsync(selector);

		public Task<double> AverageAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken)
			=> source.Queryable.AverageAsync(selector);

		public Task<decimal?> AverageAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken)
			=> source.Queryable.AverageAsync(selector);

		public Task<double> AverageAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken)
			=> source.Queryable.AverageAsync(selector);

		public Task<bool> ContainsAsync< TSource>(IContextQueryable< TSource> source, TSource item)
			=> source.Queryable.ContainsAsync(item);

		public Task<bool> ContainsAsync< TSource>(IContextQueryable< TSource> source, TSource item, CancellationToken cancellationToken)
			=> source.Queryable.ContainsAsync(item,cancellationToken);

		public Task<int> CountAsync< TSource>(IContextQueryable< TSource> source)
			=> source.Queryable.CountAsync();

		public Task<int> CountAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, bool>> predicate)
			=> source.Queryable.CountAsync(predicate);

		public Task<int> CountAsync< TSource>(IContextQueryable< TSource> source, CancellationToken cancellationToken)
			=> source.Queryable.CountAsync(cancellationToken);

		public Task<int> CountAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
			=> source.Queryable.CountAsync(predicate,cancellationToken);

		public Task<TSource> FirstAsync< TSource>(IContextQueryable< TSource> source)
			=> source.Queryable.FirstAsync();

		public Task<TSource> FirstAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, bool>> predicate)
			=> source.Queryable.FirstAsync(predicate);

		public Task<TSource> FirstAsync< TSource>(IContextQueryable< TSource> source, CancellationToken cancellationToken)
			=> source.Queryable.FirstAsync(cancellationToken);

		public Task<TSource> FirstAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
			=> source.Queryable.FirstAsync(predicate,cancellationToken);

		public Task<TSource> FirstOrDefaultAsync< TSource>(IContextQueryable< TSource> source)
			=> source.Queryable.FirstOrDefaultAsync();

		public Task<TSource> FirstOrDefaultAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, bool>> predicate)
			=> source.Queryable.FirstAsync(predicate);

		public Task<TSource> FirstOrDefaultAsync< TSource>(IContextQueryable< TSource> source, CancellationToken cancellationToken)
			=> source.Queryable.FirstOrDefaultAsync(cancellationToken);

		public Task<TSource> FirstOrDefaultAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
			=> source.Queryable.FirstOrDefaultAsync(predicate, cancellationToken);

		public Task ForEachAsync< T>(IContextQueryable< T> source, Action<T> action)
			=> source.Queryable.ForEachAsync(action);

		public Task ForEachAsync< T>(IContextQueryable< T> source, Action<T> action, CancellationToken cancellationToken)
			=> source.Queryable.ForEachAsync(action,cancellationToken);

		public Task<long> LongCountAsync< TSource>(IContextQueryable< TSource> source)
			=> source.Queryable.LongCountAsync();

		public Task<long> LongCountAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, bool>> predicate)
			=> source.Queryable.LongCountAsync(predicate);

		public Task<long> LongCountAsync< TSource>(IContextQueryable< TSource> source, CancellationToken cancellationToken)
			=> source.Queryable.LongCountAsync(cancellationToken);

		public Task<long> LongCountAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
			=> source.Queryable.LongCountAsync(predicate, cancellationToken);

		public Task<TSource> MaxAsync< TSource>(IContextQueryable< TSource> source)
			=> source.Queryable.MaxAsync();

		public Task<TSource> MaxAsync< TSource>(IContextQueryable< TSource> source, CancellationToken cancellationToken)
			=> source.Queryable.MaxAsync(cancellationToken);

		public Task<TResult> MaxAsync< TSource, TResult>(IContextQueryable< TSource> source, Expression<Func<TSource, TResult>> selector)
			=> source.Queryable.MaxAsync(selector);

		public Task<TResult> MaxAsync< TSource, TResult>(IContextQueryable< TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken)
			=> source.Queryable.MaxAsync(selector,cancellationToken);

		public Task<TSource> MinAsync< TSource>(IContextQueryable< TSource> source)
			=> source.Queryable.MinAsync();

		public Task<TSource> MinAsync< TSource>(IContextQueryable< TSource> source, CancellationToken cancellationToken)
			=> source.Queryable.MinAsync(cancellationToken);

		public Task<TResult> MinAsync< TSource, TResult>(IContextQueryable< TSource> source, Expression<Func<TSource, TResult>> selector)
			=> source.Queryable.MinAsync(selector);

		public Task<TResult> MinAsync< TSource, TResult>(IContextQueryable< TSource> source, Expression<Func<TSource, TResult>> selector, CancellationToken cancellationToken)
			=> source.Queryable.MinAsync(selector,cancellationToken);

		public Task<TSource> SingleAsync< TSource>(IContextQueryable< TSource> source)
			=> source.Queryable.SingleAsync();

		public Task<TSource> SingleAsync< TSource>(IContextQueryable< TSource> source, CancellationToken cancellationToken)
			=> source.Queryable.SingleAsync(cancellationToken);

		public Task<TSource> SingleAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, bool>> predicate)
			=> source.Queryable.SingleAsync(predicate);

		public Task<TSource> SingleAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
			=> source.Queryable.SingleAsync(predicate,cancellationToken);

		public Task<TSource> SingleOrDefaultAsync< TSource>(IContextQueryable< TSource> source)
			=> source.Queryable.SingleOrDefaultAsync();

		public Task<TSource> SingleOrDefaultAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, bool>> predicate)
			=> source.Queryable.SingleOrDefaultAsync(predicate);

		public Task<TSource> SingleOrDefaultAsync< TSource>(IContextQueryable< TSource> source, CancellationToken cancellationToken)
			=> source.Queryable.SingleOrDefaultAsync(cancellationToken);

		public Task<TSource> SingleOrDefaultAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken)
			=> source.Queryable.SingleOrDefaultAsync(predicate, cancellationToken);

		public IContextQueryable<TSource> Skip<TSource>(IContextQueryable<TSource> source, Expression<Func<int>> countAccessor)
		{
			throw new NotSupportedException();
			//=> source.New(source.Queryable.Skip(countAccessor));
		}

		public Task<double?> SumAsync(IContextQueryable< double?> source)
			=> source.Queryable.SumAsync();

		public Task<float> SumAsync(IContextQueryable< float> source)
			=> source.Queryable.SumAsync();

		public Task<float?> SumAsync(IContextQueryable< float?> source)
			=> source.Queryable.SumAsync();

		public Task<decimal?> SumAsync(IContextQueryable< decimal?> source)
			=> source.Queryable.SumAsync();

		public Task<int?> SumAsync(IContextQueryable< int?> source)
			=> source.Queryable.SumAsync();

		public Task<decimal> SumAsync(IContextQueryable< decimal> source)
			=> source.Queryable.SumAsync();

		public Task<long> SumAsync(IContextQueryable< long> source)
			=> source.Queryable.SumAsync();

		public Task<int> SumAsync(IContextQueryable< int> source)
			=> source.Queryable.SumAsync();

		public Task<double> SumAsync(IContextQueryable< double> source)
			=> source.Queryable.SumAsync();

		public Task<long?> SumAsync(IContextQueryable< long?> source)
			=> source.Queryable.SumAsync();

		public Task<int> SumAsync(IContextQueryable< int> source, CancellationToken cancellationToken)
			=> source.Queryable.SumAsync(cancellationToken);

		public Task<float?> SumAsync(IContextQueryable< float?> source, CancellationToken cancellationToken)
			=> source.Queryable.SumAsync(cancellationToken);

		public Task<decimal?> SumAsync(IContextQueryable< decimal?> source, CancellationToken cancellationToken)
			=> source.Queryable.SumAsync(cancellationToken);

		public Task<long?> SumAsync(IContextQueryable< long?> source, CancellationToken cancellationToken)
			=> source.Queryable.SumAsync(cancellationToken);

		public Task<long> SumAsync(IContextQueryable< long> source, CancellationToken cancellationToken)
			=> source.Queryable.SumAsync(cancellationToken);

		public Task<int?> SumAsync(IContextQueryable< int?> source, CancellationToken cancellationToken)
			=> source.Queryable.SumAsync(cancellationToken);

		public Task<double> SumAsync(IContextQueryable< double> source, CancellationToken cancellationToken)
			=> source.Queryable.SumAsync(cancellationToken);

		public Task<float> SumAsync(IContextQueryable< float> source, CancellationToken cancellationToken)
			=> source.Queryable.SumAsync(cancellationToken);

		public Task<double?> SumAsync(IContextQueryable< double?> source, CancellationToken cancellationToken)
			=> source.Queryable.SumAsync(cancellationToken);

		public Task<decimal> SumAsync(IContextQueryable< decimal> source, CancellationToken cancellationToken)
			=> source.Queryable.SumAsync(cancellationToken);

		public Task<decimal> SumAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, decimal>> selector)
			=> source.Queryable.SumAsync(selector);

		public Task<double?> SumAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, double?>> selector)
			=> source.Queryable.SumAsync(selector);

		public Task<int> SumAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, int>> selector)
			=> source.Queryable.SumAsync(selector);

		public Task<long?> SumAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, long?>> selector)
			=> source.Queryable.SumAsync(selector);

		public Task<double> SumAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, double>> selector)
			=> source.Queryable.SumAsync(selector);

		public Task<decimal?> SumAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, decimal?>> selector)
			=> source.Queryable.SumAsync(selector);

		public Task<int?> SumAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, int?>> selector)
			=> source.Queryable.SumAsync(selector);

		public Task<long> SumAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, long>> selector)
			=> source.Queryable.SumAsync(selector);

		public Task<float?> SumAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, float?>> selector)
			=> source.Queryable.SumAsync(selector);

		public Task<float> SumAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, float>> selector)
			=> source.Queryable.SumAsync(selector);

		public Task<int?> SumAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, int?>> selector, CancellationToken cancellationToken)
			=> source.Queryable.SumAsync(selector,cancellationToken);

		public Task<double?> SumAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, double?>> selector, CancellationToken cancellationToken)
			=> source.Queryable.SumAsync(selector, cancellationToken);

		public Task<decimal> SumAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, decimal>> selector, CancellationToken cancellationToken)
			=> source.Queryable.SumAsync(selector, cancellationToken);

		public Task<decimal?> SumAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, decimal?>> selector, CancellationToken cancellationToken)
			=> source.Queryable.SumAsync(selector, cancellationToken);

		public Task<long?> SumAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, long?>> selector, CancellationToken cancellationToken)
			=> source.Queryable.SumAsync(selector, cancellationToken);

		public Task<long> SumAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, long>> selector, CancellationToken cancellationToken)
			=> source.Queryable.SumAsync(selector, cancellationToken);

		public Task<double> SumAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, double>> selector, CancellationToken cancellationToken)
			=> source.Queryable.SumAsync(selector, cancellationToken);

		public Task<int> SumAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, int>> selector, CancellationToken cancellationToken)
			=> source.Queryable.SumAsync(selector, cancellationToken);

		public Task<float?> SumAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, float?>> selector, CancellationToken cancellationToken)
			=> source.Queryable.SumAsync(selector, cancellationToken);

		public Task<float> SumAsync< TSource>(IContextQueryable< TSource> source, Expression<Func<TSource, float>> selector, CancellationToken cancellationToken)
			=> source.Queryable.SumAsync(selector, cancellationToken);

		public IContextQueryable<TSource> Take<TSource>(IContextQueryable<TSource> source, Expression<Func<int>> countAccessor)
		{
			throw new NotSupportedException();
			//=> source.New(source.Queryable.Take(countAccessor));
		}

		public Task<TSource[]> ToArrayAsync< TSource>(IContextQueryable< TSource> source)
			=> source.Queryable.ToArrayAsync();

		public Task<TSource[]> ToArrayAsync< TSource>(IContextQueryable< TSource> source, CancellationToken cancellationToken)
			=> source.Queryable.ToArrayAsync(cancellationToken);

		public Task<Dictionary<TKey, TSource>> ToDictionaryAsync< TSource, TKey>(IContextQueryable< TSource> source, Func<TSource, TKey> keySelector)
			=> source.Queryable.ToDictionaryAsync(keySelector);

		public Task<Dictionary<TKey, TSource>> ToDictionaryAsync< TSource, TKey>(IContextQueryable< TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
			=> source.Queryable.ToDictionaryAsync(keySelector,comparer);

		public Task<Dictionary<TKey, TSource>> ToDictionaryAsync< TSource, TKey>(IContextQueryable< TSource> source, Func<TSource, TKey> keySelector, CancellationToken cancellationToken)
			=> source.Queryable.ToDictionaryAsync(keySelector, cancellationToken);

		public Task<Dictionary<TKey, TSource>> ToDictionaryAsync< TSource, TKey>(IContextQueryable< TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
			=> source.Queryable.ToDictionaryAsync(keySelector, comparer, cancellationToken);

		public Task<Dictionary<TKey, TElement>> ToDictionaryAsync< TSource, TKey, TElement>(IContextQueryable< TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
			=> source.Queryable.ToDictionaryAsync(keySelector, elementSelector);


		public Task<Dictionary<TKey, TElement>> ToDictionaryAsync< TSource, TKey, TElement>(IContextQueryable< TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
			=> source.Queryable.ToDictionaryAsync(keySelector, elementSelector,comparer);
		public Task<Dictionary<TKey, TElement>> ToDictionaryAsync< TSource, TKey, TElement>(IContextQueryable< TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, CancellationToken cancellationToken)
			=> source.Queryable.ToDictionaryAsync(keySelector, elementSelector, cancellationToken);

		public Task<Dictionary<TKey, TElement>> ToDictionaryAsync< TSource, TKey, TElement>(IContextQueryable< TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, CancellationToken cancellationToken)
			=> source.Queryable.ToDictionaryAsync(keySelector, elementSelector,comparer, cancellationToken);

		public Task<List<TSource>> ToListAsync< TSource>(IContextQueryable< TSource> source)
			=> source.Queryable.ToListAsync();

		public Task<List<TSource>> ToListAsync< TSource>(IContextQueryable< TSource> source, CancellationToken cancellationToken)
			=> source.Queryable.ToListAsync(cancellationToken);



		public IContextQueryable<T> Include<T>(IContextQueryable<T> source, string path) where T:class
			=> source.New(source.Queryable.Include(path));

		public IContextQueryable<T> Include<T, TProperty>(IContextQueryable<T> source, Expression<Func<T, TProperty>> path) where T : class
			=>source.New(source.Queryable.Include(path));
	}
}
