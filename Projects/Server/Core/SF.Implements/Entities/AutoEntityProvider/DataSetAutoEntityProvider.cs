using System;
using System.Threading.Tasks;
using SF.Core.ServiceManagement;

namespace SF.Entities.AutoEntityProvider
{
	class DataSetAutoEntityProvider<TKey,TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>:
		IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>
	{
	

		public IDataSetEntityManager EntityManager { get; }
		IDataSetEntityManager IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>.EntityManager => EntityManager;
		IDataSetAutoEntityProviderSetting<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument> Setting { get; }

		public DataSetAutoEntityProvider(IDataSetEntityManager EntityManager, IDataSetAutoEntityProviderSetting<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument> Setting)
		{
			this.EntityManager = EntityManager;
			this.Setting = Setting;

		}
		public EntityManagerCapability Capabilities { get; } = EntityManagerCapability.All;


		public Task<TKey> CreateAsync( TEntityEditable Entity)
		{
			return Setting.CreateAsync(EntityManager, Entity);
		}

		public Task<TEntityDetail> GetAsync(TKey Id)
		{
			return Setting.GetAsync(EntityManager, Id);
		}

		public Task<TEntityDetail[]> GetAsync(TKey[] Ids)
		{
			return Setting.GetAsync(EntityManager, Ids);
		}

		public Task<TEntityEditable> LoadForEdit(TKey Id)
		{
			return Setting.LoadForEdit(EntityManager, Id);
		}

		public Task<QueryResult<TEntitySummary>> QueryAsync(TQueryArgument Arg, Paging paging)
		{
			return Setting.QueryAsync(EntityManager, Arg, paging);
		}

		public Task<QueryResult<TKey>> QueryIdentsAsync(TQueryArgument Arg, Paging paging)
		{
			return Setting.QueryIdentsAsync(EntityManager, Arg, paging); 
		}

		public Task RemoveAllAsync()
		{
			return Setting.RemoveAllAsync(EntityManager);
		}

		public Task RemoveAsync(TKey Key)
		{
			return Setting.RemoveAsync(EntityManager, Key);
		}

		public Task UpdateAsync(TEntityEditable Entity)
		{
			return Setting.UpdateAsync(EntityManager, Entity);
		}
	}


}
