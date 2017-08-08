using System;
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
		protected override Task<TPublic[]> OnPreparePublics(TPublic[] Items)
		{
			return Task.FromResult(Items);
		}
	}
	public abstract class QuerableEntitySource<TKey, TPublic, TTemp, TQueryArgument, TModel> :
		EntitySource<TKey, TPublic, TTemp, TModel>,
		IEntitySource<TKey, TPublic, TQueryArgument>
		where TPublic : class, IObjectWithId<TKey>
		where TKey : IEquatable<TKey>
		where TModel : class, IObjectWithId<TKey>
		where TQueryArgument : class, IQueryArgument<TKey>
	{
		public QuerableEntitySource(IDataSet<TModel> DataSet) : base(DataSet)
		{
		}

		abstract protected IContextQueryable<TModel> OnBuildQuery(IContextQueryable<TModel> Query, TQueryArgument Arg, Paging paging);
		abstract protected PagingQueryBuilder<TModel> PagingQueryBuilder { get; }

		public async Task<QueryResult<TKey>> QueryIdentsAsync(TQueryArgument Arg, Paging paging)
		{
		
			var q = DataSet.AsQueryable(true);
			if (Arg.Id.HasValue)
			{
				var id = Arg.Id.Value;
				q = q.Where(e => e.Id.Equals(id));
			}
			q=OnBuildQuery(q, Arg, paging);
			var re = await q.ToQueryResultAsync(
				qs=>qs.Select(i=>i.Id),
				PagingQueryBuilder,
				paging
				);
			return re;
		}
		public async Task<QueryResult<TPublic>> QueryAsync(TQueryArgument Arg, Paging paging)
		{
			var q = DataSet.AsQueryable(true);
			if (Arg.Id.HasValue)
			{
				var id = Arg.Id.Value;
				q = q.Where(e => e.Id.Equals(id));
			}
			q=OnBuildQuery(q, Arg, paging);
			var re=await q.ToQueryResultAsync(
				OnMapModelToPublic,
				OnPreparePublics,
				PagingQueryBuilder,
				paging
				);
			return re;
		}
	}
    
   
}