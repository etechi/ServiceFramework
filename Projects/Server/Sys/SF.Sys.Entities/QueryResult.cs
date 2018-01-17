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

namespace SF.Sys.Entities
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
		IContextQueryable<TDataModel> Filter(IContextQueryable<TDataModel> Query, IEntityServiceContext Context, TQueryArgument Arg);
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
	public interface IPropertySelector
	{
		string Key { get; }
		IPropertySelector GetChildSelector(string Name);
	}
	public static class PropertySelector 
	{
		public class AllSelector : IPropertySelector
		{
			public string Key => "*";

			public IPropertySelector GetChildSelector(string Name)
			{
				return this;
			}
		}
		public static IPropertySelector All { get; } = new AllSelector();
		static System.Collections.Concurrent.ConcurrentDictionary<string, IPropertySelector> Cache { get; } = new System.Collections.Concurrent.ConcurrentDictionary<string, IPropertySelector>();

		class PropSelector : Dictionary<string, IPropertySelector>, IPropertySelector
		{
			public string Key { get; set; }

			public IPropertySelector GetChildSelector(string Name)
			{
				return this.TryGetValue(Name, out var c) ? c : null;
			}
		}

		public static IPropertySelector Get(string[] Properties)
		{
			if (Properties == null || Properties.Length == 0)
				return All;

			var key =string.Join(
				"\n", 
				Properties
				.Select(p => p.Trim())
				.Where(p => !p.IsNullOrEmpty())
				.OrderBy(p => p)
				)
				.UTF8Bytes()
				.MD5()
				.Base64();
			if (Cache.TryGetValue(key, out var selector))
				return selector;

			var re = new PropSelector();
			foreach (var p in Properties)
			{
				var ps = re;
				var keys = p.Split('.');
				for (var i = 0; i < keys.Length - 1; i++)
				{
					var k = keys[i];
					if (!ps.TryGetValue(k, out var ops) || ops == All)
					{
						var nps = new PropSelector();
						ps[k] = nps;
						ps = nps;
					}
				}
				ps[keys[keys.Length - 1]] = All;
			}
			re.Key = key;

			return Cache.GetOrAdd(key,re);
		}
	}
	public interface IQueryResultBuildHelper<E, R>
	{
		Task<QueryResult<R>> Query(IContextQueryable<E> queryable, IPagingQueryBuilder<E> PagingQueryBuilder,Paging Paging);
		Task<R> QuerySingleOrDefault(IContextQueryable<E> queryable,int Level);
		Expression BuildEntityMapper(Expression src,int Level, IPropertySelector PropSelector);
	}
	public interface IQueryResultBuildHelper<E, T, R> : IQueryResultBuildHelper<E, R>
	{
		Expression<Func<E, T>> GetEntityMapper(IPropertySelector PropSelector,int Level);
		Func<T[], Task<R[]>> GetResultMapper(IPropertySelector PropSelector,int Level);
	}
	public enum QueryMode
	{
		Detail,
		Summary,
		Edit
	}
	public interface IQueryResultBuildHelperCache
	{
		IQueryResultBuildHelper<E, R> GetHelper<E, R>(QueryMode QueryMode);
	}
}
