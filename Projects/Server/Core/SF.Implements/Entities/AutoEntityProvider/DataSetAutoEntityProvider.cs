using System;
using System.Threading.Tasks;
using SF.Core.ServiceManagement;

namespace SF.Entities.AutoEntityProvider
{
	class DataSetAutoEntityProvider<TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>:
		IDataSetAutoEntityProvider<TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>
		where TEntityDetail : class
		where TEntitySummary : class
		where TEntityEditable : class
		where TQueryArgument : class
	{
	

		public IDataSetEntityManager EntityManager { get; }
		IDataSetEntityManager IDataSetAutoEntityProvider<TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument>.EntityManager => EntityManager;
		IDataSetAutoEntityProviderSetting<TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument> Setting { get; }

		public DataSetAutoEntityProvider(IDataSetEntityManager EntityManager, IDataSetAutoEntityProviderSetting<TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument> Setting)
		{
			this.EntityManager = EntityManager;
			this.Setting = Setting;

		}
		public EntityManagerCapability Capabilities { get; } = EntityManagerCapability.All;


		public Task<TEntityEditable> CreateAsync( TEntityEditable Entity)
		{
			return Setting.CreateAsync(EntityManager, Entity);
		}

		public Task<TEntityDetail> GetAsync(TEntityDetail Id)
		{
			return Setting.GetAsync(EntityManager, Id);
		}

		public Task<TEntityDetail[]> GetAsync(TEntityDetail[] Ids)
		{
			return Setting.GetAsync(EntityManager, Ids);
		}

		public Task<TEntityEditable> LoadForEdit(TEntityEditable Id)
		{
			return Setting.LoadForEdit(EntityManager, Id);
		}

		public Task<QueryResult<TEntitySummary>> QueryAsync(TQueryArgument Arg, Paging paging)
		{
			return Setting.QueryAsync(EntityManager, Arg, paging);
		}

		public Task<QueryResult<TEntitySummary>> QueryIdentsAsync(TQueryArgument Arg, Paging paging)
		{
			return Setting.QueryIdentsAsync(EntityManager, Arg, paging); 
		}

		public Task RemoveAllAsync()
		{
			return Setting.RemoveAllAsync(EntityManager);
		}

		public Task<TEntityEditable> RemoveAsync(TEntityEditable Key)
		{
			return Setting.RemoveAsync(EntityManager, Key);
		}

		public Task<TEntityEditable> UpdateAsync(TEntityEditable Entity)
		{
			return Setting.UpdateAsync(EntityManager, Entity);
		}
	}


}
