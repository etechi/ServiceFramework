using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Data.Entity;

namespace SF.Data.Services
{
	public abstract class QuerableEntitySource<TKey, TPublic, TQueryArgument, TModel> :
		QuerableEntitySource<TKey, TPublic, TPublic, TQueryArgument, TModel>
		where TPublic : class, IObjectWithId<TKey>
		where TKey : IEquatable<TKey>
		where TModel : class, IObjectWithId<TKey>
		where TQueryArgument : class, IQueryArgument<TKey>
	{
		public QuerableEntitySource(IDataContext Context) : base(Context)
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
		public QuerableEntitySource(IDataContext Context) : base(Context)
		{
		}

		abstract protected IContextQueryable<TModel> OnBuildQuery(IContextQueryable<TModel> Query,TQueryArgument Arg, Paging paging);
		abstract protected PagingQueryBuilder<TModel> PagingQueryBuilder { get; }
		public async Task<QueryResult<TPublic>> Query(TQueryArgument Arg, Paging paging)
		{
			var q=OnBuildQuery(Context.Set<TModel>().AsQueryable(true), Arg, paging);
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