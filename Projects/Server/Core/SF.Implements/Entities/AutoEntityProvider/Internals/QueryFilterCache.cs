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
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using SF.Metadata;
using System.Reflection.Emit;
using SF.Core.ServiceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Entities.AutoEntityProvider.Internals
{

	public class QueryFilterCache : IQueryFilterCache
	{
		IQueryFilterProvider[] QueryFilterProviders { get; }
		
		public QueryFilterCache(IEnumerable<IQueryFilterProvider> QueryFilterProviders ){
			this.QueryFilterProviders = QueryFilterProviders.ToArray();
		}
		static MethodInfo MethodGetFilter { get; } = typeof(IQueryFilterProvider).GetMethodExt(
			nameof(IQueryFilterProvider.GetFilter),
			BindingFlags.Instance | BindingFlags.Public
			);

		class CombineQueryFilter<TDataModel, TQueryArgument> : IQueryFilter<TDataModel, TQueryArgument>
		{
			IQueryFilter<TDataModel, TQueryArgument>[] filters { get; }
			public int Priority => 0;
			public CombineQueryFilter(IQueryFilter<TDataModel, TQueryArgument>[] filters)
			{
				this.filters = filters;
			}
			public IContextQueryable<TDataModel> Filter(IContextQueryable<TDataModel> Query, IEntityServiceContext ServiceContext, TQueryArgument Arg)
			{
				foreach (var f in filters)
					Query=f.Filter(Query, ServiceContext, Arg);
				return Query;
			}
		}
		IQueryFilter<TDataModel, TQueryArgument> Build<TDataModel,TQueryArgument>()
		{
			var filters = QueryFilterProviders
				.Select(p => p.GetFilter<TDataModel, TQueryArgument>())
				.Where(f => f != null)
				.OrderBy(f => f.Priority)
				.ToArray();
			return new CombineQueryFilter<TDataModel, TQueryArgument>(filters);
		}

		System.Collections.Concurrent.ConcurrentDictionary<(Type, Type), object> Filters { get; } 
			= new System.Collections.Concurrent.ConcurrentDictionary<(Type, Type), object>();

		public IQueryFilter<TDataModel, TQueryArgument> GetFilter<TDataModel, TQueryArgument>()
		{
			var key = (typeof(TDataModel),typeof(TQueryArgument));
			if (Filters.TryGetValue(key, out var b))
				return (IQueryFilter<TDataModel,TQueryArgument>)b;
			return (IQueryFilter<TDataModel,TQueryArgument>) Filters.GetOrAdd(key, Build<TDataModel, TQueryArgument>());
		}
	}
}
