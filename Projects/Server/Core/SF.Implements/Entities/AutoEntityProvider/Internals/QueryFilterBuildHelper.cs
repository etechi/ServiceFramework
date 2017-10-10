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

	public class QueryFilterBuildHelper
	{
		IQueryFilterProvider[] QueryFilterProviders { get; }
		
		public QueryFilterBuildHelper(IQueryFilterProvider[] QueryFilterProviders ){
			this.QueryFilterProviders = QueryFilterProviders;
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
			public IContextQueryable<TDataModel> Filter(IContextQueryable<TDataModel> Query, TQueryArgument Arg)
			{
				foreach (var f in filters)
					Query=f.Filter(Query, Arg);
				return Query;
			}
		}
		public IQueryFilter<TDataModel, TQueryArgument> Build<TDataModel,TQueryArgument>()
		{
			var filters = QueryFilterProviders
				.Select(p => p.GetFilter<TDataModel, TQueryArgument>())
				.Where(f => f != null)
				.OrderBy(f => f.Priority)
				.ToArray();
			return new CombineQueryFilter<TDataModel, TQueryArgument>(filters);
		}
		
	}
}
