using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data;
using Xunit;
using SF.Applications;
using SF.Core.Hosting;
using SF.Core.ServiceManagement;
using SF.Core.ServiceFeatures;
using System.Threading.Tasks;
using System.Linq;

namespace SF.Entities.Smart
{
	public interface IDataSetSmartEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument, TModel>
		where TEntityDetail : class, IEntityWithId<TKey>
		where TEntitySummary : class, IEntityWithId<TKey>
		where TEntityEditable : class, IEntityWithId<TKey>
		where TKey : IEquatable<TKey>
		where TQueryArgument : IQueryArgument<TKey>
		where TModel : class
	{
		EntityManagerCapability Capabilities { get; }
		Task<TEntityDetail> GetAsync(IDataSetEntityManager<TModel> EntityManager, TKey Id);
		Task<TEntityDetail[]> GetAsync(IDataSetEntityManager<TModel> EntityManager, TKey[] Ids);
		Task<TKey> CreateAsync(IDataSetEntityManager<TModel> EntityManager, TEntityEditable Entity);
		Task<TEntityEditable> LoadForEdit(IDataSetEntityManager<TModel> EntityManager, TKey Id);
		Task<QueryResult<TEntitySummary>> QueryAsync(IDataSetEntityManager<TModel> EntityManager, TQueryArgument Arg, Paging paging);
		Task<QueryResult<TKey>> QueryIdentsAsync(IDataSetEntityManager<TModel> EntityManager, TQueryArgument Arg, Paging paging);
		Task RemoveAllAsync(IDataSetEntityManager<TModel> EntityManager);
		Task RemoveAsync(IDataSetEntityManager<TModel> EntityManager, TKey Key);
		Task UpdateAsync(IDataSetEntityManager<TModel> EntityManager, TEntityEditable Entity);
	}


	

	public class DataSetSmartEntityProvider<TKey, TEntityDetail, TEntityDetailTemp, TEntitySummary, TEntitySummaryTemp, TEntityEditable, TQueryArgument, TDataModel>:
		IDataSetSmartEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument, TDataModel>
		where TEntityDetail : class, IEntityWithId<TKey>
		where TEntitySummary : class, IEntityWithId<TKey>
		where TEntityEditable : class, IEntityWithId<TKey>
		where TKey : IEquatable<TKey>
		where TQueryArgument : IQueryArgument<TKey>
		where TDataModel : class,IEntityWithId<TKey>,new()
	{
		protected virtual Lazy<Func<IDataSetEntityManager<TDataModel>, IContextQueryable<TDataModel>, IContextQueryable<TEntityDetailTemp>>> FuncMapModelToDetailTemp { get; }
		protected virtual Lazy<Func<IDataSetEntityManager<TDataModel>, TEntityDetailTemp[], Task<TEntityDetail[]>>> FuncMapDetailTempToDetail { get; }

		protected virtual Lazy<Func<IDataSetEntityManager<TDataModel>, IContextQueryable<TDataModel>, IContextQueryable<TEntitySummaryTemp>>> FuncMapModelToSummaryTemp { get; }
		protected virtual Lazy<Func<IDataSetEntityManager<TDataModel>, TEntitySummaryTemp[], Task<TEntitySummary[]>>> FuncMapSummaryTempToSummary { get; }

		protected virtual Lazy<Func<IDataSetEntityManager<TDataModel>, IContextQueryable<TDataModel>, TQueryArgument, Paging, IContextQueryable<TDataModel>>> FuncBuildQuery { get; }

		protected virtual Lazy<Func<IDataSetEntityManager<TDataModel>, IContextQueryable<TDataModel>, Task<TEntityEditable>>> FuncLoadEditable { get; }
		protected virtual Lazy<Func<IDataSetEntityManager<TDataModel>, TKey, IContextQueryable<TDataModel>, Task<TDataModel>>> FuncLoadModelForModify { get; }
		protected virtual Lazy<Func<IDataSetEntityManager<TDataModel>, IEntityModifyContext<TKey, TEntityEditable, TDataModel>, Task>> FuncInitModel { get; }
		protected virtual Lazy<Func<IDataSetEntityManager<TDataModel>, IEntityModifyContext<TKey, TEntityEditable, TDataModel>, Task>> FuncUpdateModel { get; }
		protected virtual Lazy<Func<IDataSetEntityManager<TDataModel>, IEntityModifyContext<TKey, TDataModel>, Task>> FuncRemoveModel { get; }

		protected virtual Lazy<PagingQueryBuilder<TDataModel>> PagingQueryBuilder { get; }

		public DataSetSmartEntityProvider()
		{
			FuncMapModelToDetailTemp = new Lazy<Func<IDataSetEntityManager<TDataModel>, IContextQueryable<TDataModel>, IContextQueryable<TEntityDetailTemp>>>(() =>
			{
				//var ArgModel = Expression.Parameter(typeof(TDataModel));
				//var expr = Expression.Lambda<Func<TDataModel, TEntityDetailTemp>>(
				//		Expression.MemberInit(
				//		Expression.New(typeof(TEntityDetailTemp)),
				//		from dstProp in typeof(TEntityDetailTemp).AllPublicInstanceProperties()
				//		let srcProp = typeof(TDataModel).GetProperty(dstProp.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
				//		where srcProp != null
				//		select Expression.Bind(dstProp, Expression.Property(ArgModel, srcProp))
				//		),
				//		ArgModel
				//	);
				var expr = EntityMapper.Map<TDataModel, TEntityDetailTemp>();
				return (em, ctx) => ctx.Select(expr);
			});
			FuncMapDetailTempToDetail = new Lazy<Func<IDataSetEntityManager<TDataModel>, TEntityDetailTemp[], Task<TEntityDetail[]>>>(() =>
			{
				return (em, tmps) =>
				Task.FromResult(
					tmps.Select(tmp => EntityMapper.Map<TEntityDetailTemp, TEntityDetail>(tmp)).ToArray()
					);
			});
			FuncMapModelToSummaryTemp= new Lazy<Func<IDataSetEntityManager<TDataModel>, IContextQueryable<TDataModel>, IContextQueryable<TEntitySummaryTemp>>>(() =>
			{
				var expr = EntityMapper.Map<TDataModel, TEntitySummaryTemp>();
				return (em, ctx) => ctx.Select(expr);
			});
			FuncMapSummaryTempToSummary = new Lazy<Func<IDataSetEntityManager<TDataModel>, TEntitySummaryTemp[], Task<TEntitySummary[]>>>(() =>
			{
				return (em, tmps) =>
				Task.FromResult(
					tmps.Select(tmp => EntityMapper.Map<TEntitySummaryTemp, TEntitySummary>(tmp)).ToArray()
					);
			});

			FuncBuildQuery = new Lazy<Func<IDataSetEntityManager<TDataModel>, IContextQueryable<TDataModel>, TQueryArgument, Paging, IContextQueryable<TDataModel>>>(() =>
			  {
				  return (em, q, arg, pg) => q;
			  });

			FuncLoadEditable = new Lazy<Func<IDataSetEntityManager<TDataModel>, IContextQueryable<TDataModel>, Task<TEntityEditable>>>(() =>
			  {
				  var expr = EntityMapper.Map<TDataModel, TEntityEditable>();
				  return (em, q) =>
					q.Select(expr).SingleOrDefaultAsync();
			  });

			FuncLoadModelForModify = new Lazy<Func<IDataSetEntityManager<TDataModel>, TKey, IContextQueryable<TDataModel>, Task<TDataModel>>>(() =>
			{
				return (em, id, q) => q.SingleOrDefaultAsync();
			});

			FuncInitModel = new Lazy<Func<IDataSetEntityManager<TDataModel>, IEntityModifyContext<TKey, TEntityEditable, TDataModel>, Task>>(() =>
			{
				return (em,ctx) => Task.CompletedTask;
			});

			FuncUpdateModel = new Lazy<Func<IDataSetEntityManager<TDataModel>, IEntityModifyContext<TKey, TEntityEditable, TDataModel>, Task>>(() =>
			{
				var ArgModel = Expression.Parameter(typeof(TDataModel));
				var ArgEditable = Expression.Parameter(typeof(TEntityEditable));
				var update=Expression.Lambda<Action<TDataModel, TEntityEditable>>(
					Expression.Block(
						from dstProp in typeof(TDataModel).AllPublicInstanceProperties()
						where dstProp.CanWrite
						let srcProp=typeof(TEntityEditable).GetProperty(dstProp.Name,BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
						where srcProp!=null
						select Expression.Call(
							ArgModel,
							dstProp.SetMethod,
							Expression.Property(ArgEditable,srcProp)
							)
					),
					ArgModel,
					ArgEditable
					).Compile();

				return (em, ctx) =>
				{
					update(ctx.Model, ctx.Editable);
					return Task.CompletedTask;
				};
			});

			FuncRemoveModel = new Lazy<Func<IDataSetEntityManager<TDataModel>, IEntityModifyContext<TKey, TDataModel>, Task>>(() =>
			{
				return (em, ctx) => Task.CompletedTask;
			});

			Capabilities = EntityManagerCapability.All;

			PagingQueryBuilder = new Lazy<PagingQueryBuilder<TDataModel>>(() =>
				  new PagingQueryBuilder<TDataModel>("id", b => b.Add("id", m => m.Id))
			);
		}
		public EntityManagerCapability Capabilities { get; }


		public Task<TKey> CreateAsync(IDataSetEntityManager<TDataModel> EntityManager, TEntityEditable Entity)
		{
			return EntityManager.CreateAsync<TKey, TEntityEditable, TDataModel>(
				Entity,
				ctx => FuncInitModel.Value(EntityManager, ctx),
				ctx => FuncUpdateModel.Value(EntityManager, ctx),
				null
				);
		}

		public Task<TEntityDetail> GetAsync(IDataSetEntityManager<TDataModel> EntityManager, TKey Id)
		{
			return EntityManager.GetAsync(
				Id,
				ctx=>FuncMapModelToDetailTemp.Value(EntityManager,ctx),
				items=>FuncMapDetailTempToDetail.Value(EntityManager, items)
				);
		}

		public Task<TEntityDetail[]> GetAsync(IDataSetEntityManager<TDataModel> EntityManager, TKey[] Ids)
		{
			return EntityManager.GetAsync(
				Ids,
				ctx=>FuncMapModelToDetailTemp.Value(EntityManager,ctx),
				items=>FuncMapDetailTempToDetail.Value(EntityManager,items)
				);
		}

		public Task<TEntityEditable> LoadForEdit(IDataSetEntityManager<TDataModel> EntityManager, TKey Id)
		{
			return EntityManager.LoadForEdit(
				Id, 
				(ctx)=>FuncLoadEditable.Value(EntityManager,ctx)
				);
		}

		public Task<QueryResult<TEntitySummary>> QueryAsync(IDataSetEntityManager<TDataModel> EntityManager, TQueryArgument Arg, Paging paging)
		{
			return EntityManager.QueryAsync<TKey,TEntitySummaryTemp, TEntitySummary, TQueryArgument, TDataModel>(
				Arg,
				paging,
				(ctx, args, pg) =>FuncBuildQuery.Value(EntityManager,ctx,args,pg),
				PagingQueryBuilder.Value,
				(ctx)=>FuncMapModelToSummaryTemp.Value(EntityManager,ctx),
				(items)=>FuncMapSummaryTempToSummary.Value(EntityManager,items)
				);
		}

		public Task<QueryResult<TKey>> QueryIdentsAsync(IDataSetEntityManager<TDataModel> EntityManager, TQueryArgument Arg, Paging paging)
		{
			return EntityManager.QueryIdentsAsync<TKey,TQueryArgument,TDataModel>(
				Arg,
				paging,
				(ctx, args, pg) => FuncBuildQuery.Value(EntityManager, ctx, args, pg),
				PagingQueryBuilder.Value
				);
		}

		public async Task RemoveAllAsync(IDataSetEntityManager<TDataModel> EntityManager)
		{
			await EntityManager.RemoveAllAsync<TKey,TDataModel>(
				async id=>
					await RemoveAsync(EntityManager,id)
				);
		}

		public async Task RemoveAsync(IDataSetEntityManager<TDataModel> EntityManager, TKey Key)
		{
			await EntityManager.RemoveAsync<TKey, TDataModel, TEntityEditable>(
				Key,
				(ctx) => FuncRemoveModel.Value(EntityManager,ctx),
				(key,ctx)=> FuncLoadModelForModify.Value(EntityManager,key,ctx)
				);
		}

		public async Task UpdateAsync(IDataSetEntityManager<TDataModel> EntityManager, TEntityEditable Entity)
		{
			await EntityManager.UpdateAsync<TKey,TEntityEditable,TDataModel>(
				Entity,
				ctx=>FuncUpdateModel.Value(EntityManager,ctx),
				(key,ctx)=>FuncLoadModelForModify.Value(EntityManager,key,ctx)
				);
		}
	}

	public class EntityManager<TKey, TEntityDetail, TEntitySummary,TEntityEditable, TQueryArgument,TModel> :
		IEntityLoadable<TKey, TEntityDetail>,
		IEntityBatchLoadable<TKey, TEntityDetail>,
		IEntityQueryable<TKey, TEntitySummary, TQueryArgument>,
		IEntityManagerCapabilities,
		IEntityEditableLoader<TKey, TEntityEditable>,
		IEntityCreator<TKey, TEntityEditable>,
		IEntityUpdator<TKey, TEntityEditable>,
		IEntityRemover<TKey>,
		IEntityAllRemover
		where TEntityDetail : class,IEntityWithId<TKey>
		where TEntitySummary : class, IEntityWithId<TKey>
		where TEntityEditable : class, IEntityWithId<TKey>
		where TKey : IEquatable<TKey>
		where TQueryArgument : IQueryArgument<TKey>
		where TModel:class
	{
		IDataSetEntityManager<TModel> DataSetEntityManager { get; }
		IDataSetSmartEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument, TModel> SmartEntityProvider { get; }
		public EntityManager(
			IDataSetEntityManager<TModel> DataSetEntityManager, 
			IDataSetSmartEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument, TModel> SmartEntityProvider
			)
		{
			this.DataSetEntityManager = DataSetEntityManager;
			this.SmartEntityProvider = SmartEntityProvider;
		}

		public EntityManagerCapability Capabilities => SmartEntityProvider.Capabilities;

		public Task<TKey> CreateAsync(TEntityEditable Entity)
		{
			return SmartEntityProvider.CreateAsync(DataSetEntityManager, Entity);
		}

		public Task<TEntityDetail> GetAsync(TKey Id)
		{
			return SmartEntityProvider.GetAsync(DataSetEntityManager,Id);
		}

		public Task<TEntityDetail[]> GetAsync(TKey[] Ids)
		{
			return SmartEntityProvider.GetAsync(DataSetEntityManager, Ids);
		}

		public Task<TEntityEditable> LoadForEdit(TKey Id)
		{
			return SmartEntityProvider.LoadForEdit(DataSetEntityManager, Id);
		}

		public Task<QueryResult<TEntitySummary>> QueryAsync(TQueryArgument Arg, Paging paging)
		{
			return SmartEntityProvider.QueryAsync(DataSetEntityManager, Arg, paging);
		}

		public Task<QueryResult<TKey>> QueryIdentsAsync(TQueryArgument Arg, Paging paging)
		{
			return SmartEntityProvider.QueryIdentsAsync(DataSetEntityManager, Arg, paging);
		}

		public Task RemoveAllAsync()
		{
			return SmartEntityProvider.RemoveAllAsync(DataSetEntityManager);
		}

		public Task RemoveAsync(TKey Key)
		{
			return SmartEntityProvider.RemoveAsync(DataSetEntityManager,Key);
		}

		public Task UpdateAsync(TEntityEditable Entity)
		{
			return SmartEntityProvider.UpdateAsync(DataSetEntityManager,Entity);
		}
	}


}
