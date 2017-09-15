using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Data;

namespace SF.Entities
{
	public abstract class QuerableEntitySource<TKey, TPublic, TQueryArgument, TModel> :
		QuerableEntitySource<TKey, TPublic, TPublic, TQueryArgument, TModel>
		where TPublic : class, IEntityWithId<TKey>
		where TKey : IEquatable<TKey>
		where TModel : class, IEntityWithId<TKey>
		where TQueryArgument : class, IQueryArgument<TKey>
	{
		public QuerableEntitySource(IDataSetEntityManager<TModel> EntityManager) : base(EntityManager)
		{
		}
		protected override async Task<TPublic[]> OnPreparePublics(TPublic[] Internals)
		{
			await EntityManager.DataEntityResolver.Fill(Internals);
			return Internals;
		}
	}
	public abstract class QuerableEntitySource<TKey, TPublic, TTemp, TQueryArgument, TModel> :
		EntitySource<TKey, TPublic, TTemp, TModel>,
		IEntitySource<TKey, TPublic, TQueryArgument>
		where TPublic : class, IEntityWithId<TKey>
		where TKey : IEquatable<TKey>
		where TModel : class, IEntityWithId<TKey>
		where TQueryArgument : class, IQueryArgument<TKey>
	{
		public QuerableEntitySource(IDataSetEntityManager<TModel> EntityManager) : base(EntityManager)
		{
		}

		abstract protected IContextQueryable<TModel> OnBuildQuery(IContextQueryable<TModel> Query, TQueryArgument Arg, Paging paging);
		abstract protected PagingQueryBuilder<TModel> PagingQueryBuilder { get; }

		public Task<QueryResult<TKey>> QueryIdentsAsync(TQueryArgument Arg, Paging paging)
		{
			return EntityManager.QueryIdentsAsync<TKey, TQueryArgument, TModel>(Arg, paging, OnBuildQuery, PagingQueryBuilder);
		}
		public Task<QueryResult<TPublic>> QueryAsync(TQueryArgument Arg, Paging paging)
		{
			return EntityManager.QueryAsync<TKey,TTemp,TPublic, TQueryArgument, TModel>(
				Arg, 
				paging,
				OnBuildQuery, 
				PagingQueryBuilder,
				OnMapModelToPublic,
				OnPreparePublics
				);
		}
	}
    
   
}