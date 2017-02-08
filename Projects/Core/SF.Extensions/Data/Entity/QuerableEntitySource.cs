﻿using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Data.Storage;

namespace SF.Data.Entity
{
	public abstract class QuerableEntitySource<TKey, TPublic, TQueryArgument, TModel> :
		QuerableEntitySource<TKey, TPublic, TPublic, TQueryArgument, TModel>
		where TPublic : class, IObjectWithId<TKey>
		where TKey : IEquatable<TKey>
		where TModel : class, IObjectWithId<TKey>
		where TQueryArgument : class, IQueryArgument<TKey>
	{
		public QuerableEntitySource(IDataSet<TModel> DataSet) : base(DataSet)
		{
		}
	}
	public abstract class QuerableEntitySource<TKey, TPublic, TTemp, TQueryArgument, TModel> :
		EntitySource<TKey, TPublic, TTemp, TModel>,
		IEntitySource<TKey, TPublic, TQueryArgument>
		where TPublic : class, IObjectWithId<TKey>
		where TKey : IEquatable<TKey>
		where TModel : class, IObjectWithId<TKey>
		where TQueryArgument: class, IQueryArgument<TKey>
	{
		public QuerableEntitySource(IDataSet<TModel> DataSet) : base(DataSet)
		{
		}

		abstract protected IContextQueryable<TModel> OnBuildQuery(IContextQueryable<TModel> Query,TQueryArgument Arg, Paging paging);
		abstract protected PagingQueryBuilder<TModel> PagingQueryBuilder { get; }
		public async Task<QueryResult<TPublic>> QueryAsync(TQueryArgument Arg, Paging paging)
		{
			var q=OnBuildQuery(DataSet.AsQueryable(true), Arg, paging);
			var re=await q.ToQueryResultAsync(
				MapModelToPublic,
				OnPreparePublics,
				PagingQueryBuilder,
				paging
				);
			return re;
		}
	}
    
   
}