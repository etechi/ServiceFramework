using System;
using System.Threading.Tasks;
using SF.Core.ServiceManagement;

namespace SF.Entities.AutoEntityProvider
{
	

	class DataSetAutoEntityProvider<TKey, TEntityDetail, TEntityDetailTemp, TEntitySummary, TEntitySummaryTemp, TEntityEditable, TQueryArgument,TDataModel>:
		IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>
		where TEntityDetail : class, IEntityWithId<TKey>
		where TEntitySummary : class, IEntityWithId<TKey>
		where TEntityEditable : class, IEntityWithId<TKey>
		where TKey : IEquatable<TKey>
		where TQueryArgument : IQueryArgument<TKey>
		where TDataModel : class,IEntityWithId<TKey>,new()
	{
	

		public IDataSetEntityManager<TDataModel> EntityManager { get; }
		IDataSetEntityManager IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>.EntityManager => EntityManager;
		DataSetAutoEntityProviderSetting<TKey, TEntityDetail, TEntityDetailTemp, TEntitySummary, TEntitySummaryTemp, TEntityEditable, TQueryArgument, TDataModel> Setting { get; }
		public DataSetAutoEntityProvider(IServiceProvider sp, DataSetAutoEntityProviderSetting<TKey, TEntityDetail, TEntityDetailTemp, TEntitySummary, TEntitySummaryTemp, TEntityEditable, TQueryArgument, TDataModel> Setting)
		{
			EntityManager = sp.Resolve<IDataSetEntityManager<TDataModel>>();
			this.Setting = Setting;

		}
		public EntityManagerCapability Capabilities { get; } = EntityManagerCapability.All;


		public Task<TKey> CreateAsync( TEntityEditable Entity)
		{
			return EntityManager.CreateAsync<TKey, TEntityEditable, TDataModel>(
				Entity,
				ctx => Setting.FuncInitModel.Value(EntityManager, ctx),
				ctx => Setting.FuncUpdateModel.Value(EntityManager, ctx),
				null
				);
		}

		public Task<TEntityDetail> GetAsync(TKey Id)
		{
			return EntityManager.GetAsync(
				Id,
				ctx=> Setting.FuncMapModelToDetailTemp.Value(EntityManager,ctx),
				items=> Setting.FuncMapDetailTempToDetail.Value(EntityManager, items)
				);
		}

		public Task<TEntityDetail[]> GetAsync( TKey[] Ids)
		{
			return EntityManager.GetAsync(
				Ids,
				ctx=> Setting.FuncMapModelToDetailTemp.Value(EntityManager,ctx),
				items=> Setting.FuncMapDetailTempToDetail.Value(EntityManager,items)
				);
		}

		public Task<TEntityEditable> LoadForEdit(TKey Id)
		{
			return EntityManager.LoadForEdit(
				Id, 
				(ctx)=> Setting.FuncLoadEditable.Value(EntityManager,ctx)
				);
		}

		public Task<QueryResult<TEntitySummary>> QueryAsync(TQueryArgument Arg, Paging paging)
		{
			return EntityManager.QueryAsync<TKey,TEntitySummaryTemp, TEntitySummary, TQueryArgument, TDataModel>(
				Arg,
				paging,
				(ctx, args, pg) => Setting.FuncBuildQuery.Value(EntityManager,ctx,args,pg),
				Setting.PagingQueryBuilder.Value,
				(ctx)=> Setting.FuncMapModelToSummaryTemp.Value(EntityManager,ctx),
				(items)=> Setting.FuncMapSummaryTempToSummary.Value(EntityManager,items)
				);
		}

		public Task<QueryResult<TKey>> QueryIdentsAsync(TQueryArgument Arg, Paging paging)
		{
			return EntityManager.QueryIdentsAsync<TKey,TQueryArgument,TDataModel>(
				Arg,
				paging,
				(ctx, args, pg) => Setting.FuncBuildQuery.Value(EntityManager, ctx, args, pg),
				Setting.PagingQueryBuilder.Value
				);
		}

		public async Task RemoveAllAsync()
		{
			await EntityManager.RemoveAllAsync<TKey,TDataModel>(
				async id=>
					await RemoveAsync(id)
				);
		}

		public async Task RemoveAsync(TKey Key)
		{
			await EntityManager.RemoveAsync<TKey, TDataModel, TEntityEditable>(
				Key,
				(ctx) => Setting.FuncRemoveModel.Value(EntityManager,ctx),
				(key,ctx)=> Setting.FuncLoadModelForModify.Value(EntityManager,key,ctx)
				);
		}

		public async Task UpdateAsync(TEntityEditable Entity)
		{
			await EntityManager.UpdateAsync<TKey,TEntityEditable,TDataModel>(
				Entity,
				ctx=> Setting.FuncUpdateModel.Value(EntityManager,ctx),
				(key,ctx)=> Setting.FuncLoadModelForModify.Value(EntityManager,key,ctx)
				);
		}
	}


}
