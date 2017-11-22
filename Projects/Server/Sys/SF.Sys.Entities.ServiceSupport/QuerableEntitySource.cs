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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Linq;

namespace SF.Sys.Entities
{
	public abstract class AutoQuerableEntitySource<TKey, TEntityDetail, TEntitySummary, TQueryArgument, TModel> :
			  AutoEntitySource<TKey, TEntityDetail, TModel>,
			  IEntitySource<TKey, TEntitySummary, TEntityDetail, TQueryArgument>
			  where TEntityDetail : class
			   where TEntitySummary : class
			   where TQueryArgument : class
			  where TModel : class
	{
		public AutoQuerableEntitySource(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}

		protected virtual IPagingQueryBuilder<TModel> PagingQueryBuilder => null;
		protected virtual IContextQueryable<TModel> OnBuildQuery(IContextQueryable<TModel> Query, TQueryArgument Arg, Paging paging)
		{
			return Query;
		}
		public virtual Task<QueryResult<TKey>> QueryIdentsAsync(TQueryArgument Arg, Paging paging)
		{
			return ServiceContext.AutoQueryIdentsAsync<TKey, TQueryArgument, TModel>(Arg, paging, OnBuildQuery, PagingQueryBuilder);
		}
		
		public virtual Task<QueryResult<TEntitySummary>> QueryAsync(TQueryArgument Arg, Paging paging)
		{
			return ServiceContext.AutoQueryAsync<TEntitySummary, TQueryArgument, TModel>(
				Arg,
				paging,
				OnBuildQuery,
				PagingQueryBuilder
				);
		}
	}
	public abstract class QuerableEntitySource<TKey,TEntityDetail, TDetailTemp, TEntitySummary, TSummaryTemp, TQueryArgument, TModel> :
		   EntitySource<TKey, TEntityDetail, TDetailTemp, TModel>,
		   IEntitySource<TKey, TEntitySummary, TEntityDetail, TQueryArgument>
		   where TEntityDetail : class
			where TEntitySummary : class
			where TQueryArgument:class
		   where TModel : class
	{
		public QuerableEntitySource(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}

		abstract protected IContextQueryable<TModel> OnBuildQuery(IContextQueryable<TModel> Query, TQueryArgument Arg, Paging paging);
		abstract protected PagingQueryBuilder<TModel> PagingQueryBuilder { get; }

		public Task<QueryResult<TKey>> QueryIdentsAsync(TQueryArgument Arg, Paging paging)
		{
			return ServiceContext.QueryIdentsAsync<TKey, TQueryArgument, TModel>(Arg, paging, OnBuildQuery, PagingQueryBuilder);
		}
		protected virtual IContextQueryable<TSummaryTemp> OnMapModelToSummary(IContextQueryable<TModel> Query)
		{
			return Query.Select(Poco.MapExpression<TModel, TSummaryTemp>());
		}
		protected abstract Task<TEntitySummary[]> OnPrepareSummaries(TSummaryTemp[] Internals);

		public Task<QueryResult<TEntitySummary>> QueryAsync(TQueryArgument Arg, Paging paging)
		{
			return ServiceContext.QueryAsync<TSummaryTemp, TEntitySummary, TQueryArgument, TModel>(
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
		   where TEntityDetail : class
			where TQueryArgument:class
		   where TModel : class
	{
		public QuerableEntitySource(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}
		protected override IContextQueryable<TDetailTemp> OnMapModelToSummary(IContextQueryable<TModel> Query)
			=> OnMapModelToDetail(Query);

		protected override Task<TEntityDetail[]> OnPrepareSummaries(TDetailTemp[] Internals)
			=> OnPrepareDetails(Internals);

	}
	public abstract class QuerableEntitySource<TKey, TPublic, TQueryArgument, TModel> :
		QuerableEntitySource<TKey, TPublic, TPublic, TQueryArgument, TModel>
		where TPublic : class
		where TModel : class
		where TQueryArgument : class
	{
		public QuerableEntitySource(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}
		protected override async Task<TPublic[]> OnPrepareDetails(TPublic[] Internals)
		{
			await ServiceContext.DataEntityResolver.Fill(ServiceInstanceDescriptor.InstanceId, Internals);
			return Internals;
		}
	}
	
	public abstract class ConstantQueryableEntitySource<TKey, TInternal> :
		 ConstantQueryableEntitySource<TKey, TInternal, TInternal, QueryArgument<TKey>, TInternal>
		 where TInternal : class
		 where TKey : class
	{
		public ConstantQueryableEntitySource(IEntityServiceContext EntityManager, IReadOnlyDictionary<TKey, TInternal> Models) : base(EntityManager, Models)
		{
		}

		protected override IContextQueryable<TInternal> OnMapModelToInternal(IContextQueryable<TInternal> Query)
		{
			return Query;
		}
		protected override PagingQueryBuilder<TInternal> PagingQueryBuilder => null;
			//new PagingQueryBuilder<TInternal>(
			//"id", b => b.Add("id", m => m.Id)
			//);
		protected override IContextQueryable<TInternal> OnBuildQuery(IContextQueryable<TInternal> Query, QueryArgument<TKey> Arg, Paging paging)
		{
			return Query;
		}
	}
	public abstract class ConstantObjectQueryableEntitySource<TKey, TInternal> :
		 ConstantQueryableEntitySource<TKey, TInternal, TInternal, ObjectQueryArgument<TKey>, TInternal>
		 where TInternal : class, IObjectEntity
		where TKey:class
	{
		public ConstantObjectQueryableEntitySource(IEntityServiceContext EntityManager, IReadOnlyDictionary<TKey, TInternal> Models) : base(EntityManager, Models)
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
		where TInternal:class
	{
		public ConstantQueryableEntitySource(IEntityServiceContext EntityManager, IReadOnlyDictionary<TKey, TInternal> Models) : base(EntityManager, Models)
		{
		}

		protected override IContextQueryable<TInternal> OnMapModelToInternal(IContextQueryable<TInternal> Query)
		{
			return Query;
		}
	}
	public abstract class ConstantQueryableEntitySource<TKey, TInternal, TQueryArgument, TModel> :
		 ConstantQueryableEntitySource<TKey, TInternal, TInternal, TQueryArgument, TModel>
		where TModel : class
	{
		public ConstantQueryableEntitySource(IEntityServiceContext EntityManager, IReadOnlyDictionary<TKey, TModel> Models) : base(EntityManager, Models)
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
		where TModel: class
	{
		public ConstantQueryableEntitySource(IEntityServiceContext EntityManager, IReadOnlyDictionary<TKey, TModel> Models) : base(EntityManager, Models)
		{
		}

		abstract protected IContextQueryable<TModel> OnBuildQuery(IContextQueryable<TModel> Query, TQueryArgument Arg, Paging paging);
		abstract protected PagingQueryBuilder<TModel> PagingQueryBuilder { get; }

		public Task<QueryResult<TKey>> QueryIdentsAsync(TQueryArgument Arg, Paging paging)
		{
			var q = Models.Values.AsContextQueryable();
			q = Entity<TModel>.QueryIdentFilter(q, Arg);
			q = OnBuildQuery(q, Arg, paging);
			return q
				.ToQueryResultAsync(
				iq=>iq.Select(Entity<TModel>.KeySelector<TKey>()),
				rs=>rs,
				PagingQueryBuilder,
				paging
				);
		}
		public Task<QueryResult<TInternal>> QueryAsync(TQueryArgument Arg, Paging paging)
		{
			var q = Models.Values.AsContextQueryable();
			q = Entity<TModel>.QueryIdentFilter(q, Arg);
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