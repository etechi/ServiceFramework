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
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Entities
{
	public interface ISummary
    {
    }
	public interface IQueryResult
	{
		 ISummary Summary { get; set; }
		 int? Total { get; set; }
		 IEnumerable Items { get; set; }
	}
	public class QueryResult<T> : IQueryResult,ISummary
	{
        public ISummary Summary { get; set; }
        public int? Total { get; set; }
        public IEnumerable<T> Items { get; set; }
        public static QueryResult<T> Empty { get; } = new QueryResult<T>
        {
            Items = Enumerable.Empty<T>()
        };
		IEnumerable IQueryResult.Items { get { return Items; } set { Items = (IEnumerable<T>)value; } }
	}

	public interface ISummaryWithCount : ISummary
	{
		int Count { get; }
	}
	public interface IPropertyQueryFilter
	{
		int Priority { get; }
	}

	public interface IPropertyQueryFilter<T> : IPropertyQueryFilter
	{
		Expression GetFilterExpression(Expression obj, T value);
	}
	public interface IPropertyQueryFilterProvider
	{
		IPropertyQueryFilter GetFilter<TDataModel,TQueryArgument>(PropertyInfo queryProp);
		int Priority { get; }
	}
	public interface IQueryFilter<TDataModel, TQueryArgument>
	{
		int Priority { get; }
		IContextQueryable<TDataModel> Filter(IContextQueryable<TDataModel> Query, IDataSetEntityManager EntityManager, TQueryArgument Arg);
	}
	public interface IQueryFilterProvider
	{
		IQueryFilter<TDataModel, TQueryArgument> GetFilter<TDataModel, TQueryArgument>();
	}
	public interface IQueryFilterCache : IQueryFilterProvider
	{
	}

	public interface IPagingQueryBuilder<T>
	{
		IContextQueryable<T> Build(IContextQueryable<T> query, Paging paging);
	}
	public interface IPagingQueryBuilderCache
	{
		IPagingQueryBuilder<T> GetBuilder<T>();
	}
	public interface IQueryResultBuildHelper<E, R>
	{
		Task<QueryResult<R>> Query(IContextQueryable<E> queryable, IPagingQueryBuilder<E> PagingQueryBuilder,Paging paging);
		Task<R> QuerySingleOrDefault(IContextQueryable<E> queryable);
	}
	public interface IQueryResultBuildHelper<E, T, R> : IQueryResultBuildHelper<E, R>
	{
		Expression<Func<E, T>> EntityMapper { get; }
		Func<T[],Task<R[]>> ResultMapper { get; }
	}
	public interface IQueryResultBuildHelperCache
	{
		IQueryResultBuildHelper<E, R> GetHelper<E, R>();
	}
}
