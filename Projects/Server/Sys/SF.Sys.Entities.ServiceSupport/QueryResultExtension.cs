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


using SF.Sys.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Entities
{



	public static class QueryResultExtension
	{

		
		public static QueryResult<E> ToQueryResult<E>(
			this IQueryable<E> query,
			Paging paging,
			bool? totalRequired = null
			)
			=> query.ToQueryResult(
				q => q,
				r => r,
				null,
				paging,
				totalRequired
				);

		public static QueryResult<R> ToQueryResult<E, T, R>(
			this IQueryable<E> query,
			Func<IQueryable<E>, IQueryable<T>> mapper,
			Func<T, R> resultMapper,
			IPagingQueryBuilder<E> pagingBuilder,
			Paging paging,
			bool? totalRequired = null
			)
		{
			int? total = null;
			if (totalRequired ?? paging?.TotalRequired ?? false)
				total = query.Count();

			if (paging != null && paging.Count == 0)
				return new QueryResult<R> { Total = total, Items = Array.Empty<R>() };

			query = query.ApplyPaging(pagingBuilder, paging);

			var mappedQuery = mapper(query);
			var result = mappedQuery.ToArray();
			var mappedResult = result.Select(resultMapper).ToArray();
			return new QueryResult<R>
			{
				Total = total,
				Items = mappedResult
			};
		}
		public static Task<QueryResult<E>> ToQueryResultAsync<E>(
			this IQueryable<E> query,
			Paging paging,
			bool? totalRequired = null
			)
			where E : class
			=> query.ToQueryResultAsync<E, E>(
				q => q,
				null,
				paging,
				totalRequired
				);
		public static IQueryable<T> ApplyPaging<T>(
			this IQueryable<T> query,
			IPagingQueryBuilder<T> pagingBuilder,
			Paging paging
			) {
			if (pagingBuilder != null)
				return pagingBuilder.Build(query, paging);
			if (paging == null)
				paging = Paging.Default;
			return query.Skip(paging.Offset).Take(paging.Count);
		}
		public static async Task<QueryResult<T>> ToQueryResultAsync<E, T>(
            this IQueryable<E> query,
            Func<IQueryable<E>, IQueryable<T>> mapper,
			IPagingQueryBuilder<E> pagingBuilder,
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

            query = query.ApplyPaging(pagingBuilder,paging);
            var mappedQuery = mapper(query);
            var result = await mappedQuery.ToArrayAsync();
            return new QueryResult<T>
            {
                Total = total,
                Items = result
            };
        }

		

		public static async Task<QueryResult<R>> ToQueryResultAsync<E, T, R>(
			this IQueryable<E> query,
			Func<IQueryable<E>, IQueryable<T>> mapper,
			Func<T, R> resultMapper,
			IPagingQueryBuilder<E> pagingBuilder,
			Paging paging,
			Func<IQueryable<E>,Task<ISummaryWithCount>> Summary=null
			)
			where E : class
		{
            int? total = null;
            ISummaryWithCount swc = null;
            if(Summary!=null && (paging?.SummaryRequired ?? false))
            {
                swc = await Summary(query);
                total = swc?.Count ?? 0;
            }
            else if (paging?.TotalRequired ?? false)
                total = await query.CountAsync();

            if (paging != null && paging.Count == 0)
                return new QueryResult<R> { Total = total, Items = Array.Empty<R>() };

			query = query.ApplyPaging(pagingBuilder, paging);

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

		//public static Task<QueryResult<R>> ToQueryResultAsync<E, T, R>(
		//	this IQueryable<E> query,
		//	IQueryResultBuildHelper<E, T, R> mapper,
		//	Paging paging
		//	)
		//	where E : class
		//	=> query.ToQueryResultAsync<E,T,R>(
		//		e => e.Select(mapper.EntityMapper),
		//		mapper.ResultMapper,
		//		mapper.PagingBuilder,
		//		paging,
		//		mapper.Summary
		//		);

		public static async Task<QueryResult<R>> ToQueryResultAsync<E, T, R>(
			this IQueryable<E> query,
			Func<IQueryable<E>, IQueryable<T>> mapper,
			Func<T[], Task<R[]>> resultMapper,
			IPagingQueryBuilder<E> pagingBuilder,
			Paging paging,
			Func<IQueryable<E>, Task<ISummaryWithCount>> Summary = null
			)
			where E : class
		{
            int? total = null;
            ISummaryWithCount swc = null;
            if (Summary != null && (paging?.SummaryRequired ?? false))
            {
				swc = await Summary(query);
                total = swc?.Count ?? 0;
            }
            else if (paging?.TotalRequired ?? false)
                total = await query.CountAsync();

            if (paging != null && paging.Count == 0)
                return new QueryResult<R> { Total = total, Items = Array.Empty<R>() };

			query = query.ApplyPaging(pagingBuilder, paging);

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
