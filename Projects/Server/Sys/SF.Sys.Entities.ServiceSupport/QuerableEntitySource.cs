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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SF.Sys.Data;
using SF.Sys.Linq;

namespace SF.Sys.Entities
{
	public abstract class AutoQueryableEntitySource<TKey, TEntityDetail, TEntitySummary, TQueryArgument, TModel> :
			  AutoEntitySource<TKey, TEntityDetail, TModel>,
			  IEntitySource<TKey, TEntitySummary, TEntityDetail, TQueryArgument>
			  where TEntityDetail : class
			   where TEntitySummary : class
			   where TQueryArgument : class, IPagingArgument
			  where TModel : class
	{
		public AutoQueryableEntitySource(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}

		protected virtual IPagingQueryBuilder<TModel> PagingQueryBuilder => null;
		protected virtual IContextQueryable<TModel> OnBuildQuery(IContextQueryable<TModel> Query, TQueryArgument Arg)
		{
			return Query;
		}
		public virtual Task<QueryResult<TKey>> QueryIdentsAsync(TQueryArgument Arg)
		{
			return ServiceContext.AutoQueryIdentsAsync<TKey, TQueryArgument, TModel>(Arg, OnBuildQuery, PagingQueryBuilder);
		}
		//protected abstract Task<TEntitySummary[]> OnPrepareSummaries(TSummaryTemp[] Internals);
		protected virtual Expression<Func<IGrouping<int, TModel>, ISummaryWithCount>> GetSummaryExpression()
		{
			return null;
		}
		protected virtual Func<IContextQueryable<TModel>,Task<ISummaryWithCount>> GetSummaryBuilder()
		{
			var e = GetSummaryExpression();
			return e == null ? 
				(Func<IContextQueryable<TModel>, Task<ISummaryWithCount>>)null : 
				q => q.GroupBy(r => 1).Select(e).SingleOrDefaultAsync();
		}
		public virtual Task<QueryResult<TEntitySummary>> QueryAsync(TQueryArgument Arg)
		{
			var summary = GetSummaryBuilder();
			return ServiceContext.AutoQueryAsync<TEntitySummary, TQueryArgument, TModel>(
				Arg,
				OnBuildQuery,
				PagingQueryBuilder,
				GetSummaryBuilder()
				);
		}
		public IContextQueryable<TModel> GetQueryable(SF.Sys.Data.IDataContext ctx, TQueryArgument Arg)
		{
			var q = ctx.Queryable<TModel>();
			if (Arg!=null)
				q = ServiceContext.QueryFilterCache.GetFilter<TModel, TQueryArgument>()
					.Filter(q, ServiceContext, Arg);
			q = OnBuildQuery(q, Arg);
			return q;
		}
	}
	public abstract class QuerableEntitySource<TKey,TEntityDetail, TDetailTemp, TEntitySummary, TSummaryTemp, TQueryArgument, TModel> :
		   EntitySource<TKey, TEntityDetail, TDetailTemp, TModel>,
		   IEntitySource<TKey, TEntitySummary, TEntityDetail, TQueryArgument>
		   where TEntityDetail : class
			where TEntitySummary : class
			where TQueryArgument:class,IPagingArgument
		   where TModel : class
	{
		public QuerableEntitySource(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}

		abstract protected IContextQueryable<TModel> OnBuildQuery(IContextQueryable<TModel> Query, TQueryArgument Arg);
		abstract protected PagingQueryBuilder<TModel> PagingQueryBuilder { get; }

		public Task<QueryResult<TKey>> QueryIdentsAsync(TQueryArgument Arg)
		{
			return ServiceContext.QueryIdentsAsync<TKey, TQueryArgument, TModel>(Arg, OnBuildQuery, PagingQueryBuilder);
		}
		protected virtual IContextQueryable<TSummaryTemp> OnMapModelToSummary(IContextQueryable<TModel> Query)
		{
			return Query.Select(Poco.MapExpression<TModel, TSummaryTemp>());
		}
		protected abstract Task<TEntitySummary[]> OnPrepareSummaries(TSummaryTemp[] Internals);

		public Task<QueryResult<TEntitySummary>> QueryAsync(TQueryArgument Arg)
		{
			return ServiceContext.QueryAsync<TSummaryTemp, TEntitySummary, TQueryArgument, TModel>(
				Arg,
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
			where TQueryArgument:class,IPagingArgument
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
		where TQueryArgument : class,IPagingArgument
	{
		public QuerableEntitySource(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}
		protected override async Task<TPublic[]> OnPrepareDetails(TPublic[] Internals)
		{
			await ServiceContext.EntityPropertyFiller.Fill(ServiceInstanceDescriptor.InstanceId, Internals);
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
		protected override IContextQueryable<TInternal> OnBuildQuery(IContextQueryable<TInternal> Query, QueryArgument<TKey> Arg)
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
		protected override IContextQueryable<TInternal> OnBuildQuery(IContextQueryable<TInternal> Query, ObjectQueryArgument<TKey> Arg)
		{
			return Query.Filter(Arg.Name, m => m.Name)
				.Filter(Arg.LogicState, m => m.LogicState);
		}
	}
	public abstract class ConstantQueryableEntitySource<TKey, TInternal, TQueryArgument> :
		 ConstantQueryableEntitySource<TKey, TInternal, TInternal, TQueryArgument, TInternal>
		where TInternal:class
		where TQueryArgument : IPagingArgument
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
		where TQueryArgument:IPagingArgument
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
		where TQueryArgument:IPagingArgument
	{
		public ConstantQueryableEntitySource(IEntityServiceContext EntityManager, IReadOnlyDictionary<TKey, TModel> Models) : base(EntityManager, Models)
		{
		}

		abstract protected IContextQueryable<TModel> OnBuildQuery(IContextQueryable<TModel> Query, TQueryArgument Arg);
		abstract protected PagingQueryBuilder<TModel> PagingQueryBuilder { get; }

		public Task<QueryResult<TKey>> QueryIdentsAsync(TQueryArgument Arg)
		{
			var q = Models.Values.AsContextQueryable();
			q = Entity<TModel>.QueryIdentFilter(q, Arg);
			q = OnBuildQuery(q, Arg);
			return q
				.ToQueryResultAsync(
				iq=>iq.Select(Entity<TModel>.KeySelector<TKey>()),
				rs=>rs,
				PagingQueryBuilder,
				Arg.Paging
				);
		}
		public Task<QueryResult<TInternal>> QueryAsync(TQueryArgument Arg)
		{
			var q = Models.Values.AsContextQueryable();
			q = Entity<TModel>.QueryIdentFilter(q, Arg);
			q = OnBuildQuery(q, Arg);
			return q
				.ToQueryResultAsync(
				OnMapModelToInternal,
				OnPrepareInternals,
				PagingQueryBuilder,
				Arg.Paging
				);
		}
	}
}