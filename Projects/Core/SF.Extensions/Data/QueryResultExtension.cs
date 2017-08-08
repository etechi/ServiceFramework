using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data
{
	
	public interface ISummaryWithCount : ISummary
    {
        int Count { get;  }
    }

    public static class QueryResultExtension
	{
		public static QueryResult<R> ToQueryResult<E, T, R>(
			   this IQueryable<E> query,
			   Func<IQueryable<E>, IQueryable<T>> mapper,
			   Func<T, R> resultMapper,
			   PagingQueryBuilder<E> pagingBuilder,
			   Paging paging,
			   bool? totalRequired = null
			   )
		{
			return ToQueryResult(
				new NullContextQueryable<E>(query),
				e => new NullContextQueryable<T>(mapper(e.Queryable)),
				resultMapper,
				pagingBuilder,
				paging,
				totalRequired
				);
		}
		public static QueryResult<R> ToQueryResult<E, T, R>(
			this IContextQueryable<E> query,
			Func<IContextQueryable<E>, IContextQueryable<T>> mapper,
			Func<T, R> resultMapper,
			PagingQueryBuilder<E> pagingBuilder,
			Paging paging,
			bool? totalRequired = null
			)
		{
            int? total = null;
            if (totalRequired ?? paging?.TotalRequired ?? false)
                total = query.Count();

            if (paging!=null && paging.Count == 0)
                return new QueryResult<R> { Total = total, Items = Array.Empty<R>() };

            query = pagingBuilder.Build(query, paging);
			var mappedQuery = mapper(query);
			var result = mappedQuery.ToArray();
			var mappedResult = result.Select(resultMapper).ToArray();
			return new QueryResult<R>
			{
				Total = total,
				Items = mappedResult
			};
		}
        public static async Task<QueryResult<T>> ToQueryResultAsync<E, T>(
            this IContextQueryable<E> query,
            Func<IContextQueryable<E>, IContextQueryable<T>> mapper,
            PagingQueryBuilder<E> pagingBuilder,
            Paging paging,
            bool? totalRequired = null
            )
            where E : class
        {
            int? total = null;
            if (totalRequired ?? paging?.TotalRequired ?? false)
                total = await query.CountAsync();

            if (paging != null && paging.Count == 0)
                return new QueryResult<T> { Total = total, Items = Array.Empty<T>() };

            query = pagingBuilder.Build(query, paging);
            var mappedQuery = mapper(query);
            var result = await mappedQuery.ToArrayAsync();
            return new QueryResult<T>
            {
                Total = total,
                Items = result
            };
        }
        
        public static async Task<QueryResult<R>> ToQueryResultAsync<E, T, R>(
			this IContextQueryable<E> query,
			Func<IContextQueryable<E>, IContextQueryable<T>> mapper,
			Func<T, R> resultMapper,
			PagingQueryBuilder<E> pagingBuilder,
			Paging paging,
			System.Linq.Expressions.Expression<Func<IGrouping<int,E>,ISummaryWithCount>> Summary=null
			)
			where E : class
		{
            int? total = null;
            ISummaryWithCount swc = null;
            if(Summary!=null && (paging?.SummaryRequired ?? false))
            {
                swc = await query.GroupBy(i=>1).Select(Summary).SingleOrDefaultAsync();
                total = swc?.Count ?? 0;
            }
            else if (paging?.TotalRequired ?? false)
                total = await query.CountAsync();

            if (paging != null && paging.Count == 0)
                return new QueryResult<R> { Total = total, Items = Array.Empty<R>() };

            query = pagingBuilder.Build(query, paging);
			var mappedQuery = mapper(query);
			var result = await mappedQuery.ToArrayAsync();
			var mappedResult = result.Select(resultMapper).ToArray();
			return new QueryResult<R>
			{
                Summary = swc,
                Total = total,
				Items = mappedResult
			};
		}
		public static async Task<QueryResult<R>> ToQueryResultAsync<E, T, R>(
			this IContextQueryable<E> query,
			Func<IContextQueryable<E>, IContextQueryable<T>> mapper,
			Func<T[], Task<R[]>> resultMapper,
			PagingQueryBuilder<E> pagingBuilder,
			Paging paging,
            System.Linq.Expressions.Expression<Func<IGrouping<int, E>, ISummaryWithCount>> Summary = null
            )
            where E : class
		{
            int? total = null;
            ISummaryWithCount swc = null;
            if (Summary != null && (paging?.SummaryRequired ?? false))
            {
                swc = await query.GroupBy(i => 1).Select(Summary).SingleOrDefaultAsync();
                total = swc?.Count ?? 0;
            }
            else if (paging?.TotalRequired ?? false)
                total = await query.CountAsync();

            if (paging != null && paging.Count == 0)
                return new QueryResult<R> { Total = total, Items = Array.Empty<R>() };

			query = pagingBuilder.Build(query, paging);
			var mappedQuery = mapper(query);
			var result = await mappedQuery.ToArrayAsync();
			var mappedResult = await resultMapper(result);
            return new QueryResult<R>
			{
                Summary = swc,
                Total = total,
				Items = mappedResult
			};
		}
		public static QueryResult<T> Select<R,T>(this QueryResult<R> result,Func<R,T> selector)
		{
			return new QueryResult<T>
			{
				Items = result.Items.Select(selector),
				Summary = result.Summary,
				Total = result.Total
			};
		}
	}
}
