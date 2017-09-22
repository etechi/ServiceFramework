using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Data;

namespace SF.Entities
{
	public abstract class QuerableEntitySource<TKey, TEntityDetail, TDetailTemp, TEntitySummary, TSummaryTemp, TQueryArgument, TModel> :
		   EntitySource<TKey, TEntityDetail, TDetailTemp, TModel>,
		   IEntitySource<TKey, TEntitySummary, TEntityDetail, TQueryArgument>
		   where TEntityDetail : class, IEntityWithId<TKey>
			where TEntitySummary : class, IEntityWithId<TKey>
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
		protected virtual IContextQueryable<TSummaryTemp> OnMapModelToSummary(IContextQueryable<TModel> Query)
		{
			return Query.Select(EntityMapper.Map<TModel, TSummaryTemp>());
		}
		protected abstract Task<TEntitySummary[]> OnPrepareSummaries(TSummaryTemp[] Internals);

		public Task<QueryResult<TEntitySummary>> QueryAsync(TQueryArgument Arg, Paging paging)
		{
			return EntityManager.QueryAsync<TKey, TSummaryTemp, TEntitySummary, TQueryArgument, TModel>(
				Arg,
				paging,
				OnBuildQuery,
				PagingQueryBuilder,
				OnMapModelToSummary,
				OnPrepareSummaries
				);
		}
	}
	public abstract class QuerableEntitySource<TKey, TEntityDetail, TDetailTemp, TQueryArgument, TModel> :
		   QuerableEntitySource<TKey, TEntityDetail, TDetailTemp, TEntityDetail, TDetailTemp, TQueryArgument, TModel>,
		   IEntitySource<TKey, TEntityDetail, TEntityDetail, TQueryArgument>
		   where TEntityDetail : class, IEntityWithId<TKey>
		   where TKey : IEquatable<TKey>
		   where TModel : class, IEntityWithId<TKey>
		   where TQueryArgument : class, IQueryArgument<TKey>
	{
		public QuerableEntitySource(IDataSetEntityManager<TModel> EntityManager) : base(EntityManager)
		{
		}
		protected override Task<TEntityDetail[]> OnPrepareSummaries(TDetailTemp[] Internals)
			=> OnPrepareDetails(Internals);

	}
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
		protected override async Task<TPublic[]> OnPrepareDetails(TPublic[] Internals)
		{
			await EntityManager.DataEntityResolver.Fill(ServiceInstanceDescriptor.InstanceId, Internals);
			return Internals;
		}
	}
	
	public abstract class ConstantQueryableEntitySource<TKey, TInternal> :
		 ConstantQueryableEntitySource<TKey, TInternal, TInternal, QueryArgument<TKey>, TInternal>
		 where TInternal : class, IEntityWithId<TKey>
		 where TKey : IEquatable<TKey>
	{
		public ConstantQueryableEntitySource(IEntityManager EntityManager, IReadOnlyDictionary<TKey, TInternal> Models) : base(EntityManager, Models)
		{
		}

		protected override IContextQueryable<TInternal> OnMapModelToInternal(IContextQueryable<TInternal> Query)
		{
			return Query;
		}
		protected override PagingQueryBuilder<TInternal> PagingQueryBuilder => new PagingQueryBuilder<TInternal>(
			"id", b => b.Add("id", m => m.Id)
			);
		protected override IContextQueryable<TInternal> OnBuildQuery(IContextQueryable<TInternal> Query, QueryArgument<TKey> Arg, Paging paging)
		{
			return Query;
		}
	}
	public abstract class ConstantObjectQueryableEntitySource<TKey, TInternal> :
		 ConstantQueryableEntitySource<TKey, TInternal, TInternal, ObjectQueryArgument<TKey>, TInternal>
		 where TInternal : class, IEntityWithId<TKey>,IObjectEntity
		 where TKey : IEquatable<TKey>
	{
		public ConstantObjectQueryableEntitySource(IEntityManager EntityManager, IReadOnlyDictionary<TKey, TInternal> Models) : base(EntityManager, Models)
		{
		}

		protected override IContextQueryable<TInternal> OnMapModelToInternal(IContextQueryable<TInternal> Query)
		{
			return Query;
		}
		protected override Task<TInternal[]> OnPrepareInternals(TInternal[] Internals)
		{
			return Task.FromResult(Internals);
		}
		protected override PagingQueryBuilder<TInternal> PagingQueryBuilder => new PagingQueryBuilder<TInternal>(
			"name", b => b.Add("name", m => m.Name)
			);
		protected override IContextQueryable<TInternal> OnBuildQuery(IContextQueryable<TInternal> Query, ObjectQueryArgument<TKey> Arg, Paging paging)
		{
			return Query.Filter(Arg.Name, m => m.Name)
				.Filter(Arg.LogicState, m => m.LogicState);
		}
	}
	public abstract class ConstantQueryableEntitySource<TKey, TInternal, TQueryArgument> :
		 ConstantQueryableEntitySource<TKey, TInternal, TInternal, TQueryArgument, TInternal>
		 where TInternal : class, IEntityWithId<TKey>
		 where TKey : IEquatable<TKey>
		 where TQueryArgument : class, IQueryArgument<TKey>
	{
		public ConstantQueryableEntitySource(IEntityManager EntityManager, IReadOnlyDictionary<TKey, TInternal> Models) : base(EntityManager, Models)
		{
		}

		protected override IContextQueryable<TInternal> OnMapModelToInternal(IContextQueryable<TInternal> Query)
		{
			return Query;
		}
	}
	public abstract class ConstantQueryableEntitySource<TKey, TInternal, TQueryArgument, TModel> :
		 ConstantQueryableEntitySource<TKey, TInternal, TInternal, TQueryArgument, TModel>
		 where TInternal : class, IEntityWithId<TKey>
		where TModel : class, IEntityWithId<TKey>
		 where TKey : IEquatable<TKey>
		where TQueryArgument : class, IQueryArgument<TKey>
	{
		public ConstantQueryableEntitySource(IEntityManager EntityManager, IReadOnlyDictionary<TKey, TModel> Models) : base(EntityManager, Models)
		{
		}

		protected override Task<TInternal[]> OnPrepareInternals(TInternal[] Internals)
		{
			return Task.FromResult(Internals);
		}
	}
	public abstract class ConstantQueryableEntitySource<TKey, TInternal, Temp, TQueryArgument, TModel> :
		 ConstantEntitySource<TKey, TInternal,Temp,TModel>,
		 IEntitySource<TKey, TInternal, TQueryArgument>
		 where TInternal : class, IEntityWithId<TKey>
		where TModel: class, IEntityWithId<TKey>
		 where TKey : IEquatable<TKey>
		where TQueryArgument : class, IQueryArgument<TKey>
	{
		public ConstantQueryableEntitySource(IEntityManager EntityManager, IReadOnlyDictionary<TKey, TModel> Models) : base(EntityManager, Models)
		{
		}

		abstract protected IContextQueryable<TModel> OnBuildQuery(IContextQueryable<TModel> Query, TQueryArgument Arg, Paging paging);
		abstract protected PagingQueryBuilder<TModel> PagingQueryBuilder { get; }

		public Task<QueryResult<TKey>> QueryIdentsAsync(TQueryArgument Arg, Paging paging)
		{
			var q = Models.Values.AsContextQueryable();
			if (Arg.Id.HasValue)
			{
				var id = Arg.Id.Value;
				q = q.Where(e => e.Id.Equals(id));
			}
			q = OnBuildQuery(q, Arg, paging);
			return q
				.ToQueryResultAsync(
				iq=>iq.Select(m=>m.Id),
				rs=>rs,
				PagingQueryBuilder,
				paging
				);
		}
		public Task<QueryResult<TInternal>> QueryAsync(TQueryArgument Arg, Paging paging)
		{
			var q = Models.Values.AsContextQueryable();
			if (Arg.Id.HasValue)
			{
				var id = Arg.Id.Value;
				q = q.Where(e => e.Id.Equals(id));
			}
			q = OnBuildQuery(q, Arg, paging);
			return q
				.ToQueryResultAsync(
				OnMapModelToInternal,
				OnPrepareInternals,
				PagingQueryBuilder,
				paging
				);
		}
	}
}