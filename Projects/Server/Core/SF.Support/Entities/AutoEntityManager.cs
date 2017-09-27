using System;
using System.Threading.Tasks;

namespace SF.Entities.AutoEntityProvider
{

	public class AutoEntityManager<TKey, TEntityDetail, TEntitySummary,TEntityEditable, TQueryArgument> :
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
	{
		protected IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument> AutoEntityProvider { get; }
		protected IDataSetEntityManager EntityManager => AutoEntityProvider.EntityManager;
		public AutoEntityManager(
			IDataSetAutoEntityProvider<TKey, TEntityDetail, TEntitySummary, TEntityEditable, TQueryArgument> AutoEntityProvider
			)
		{
			this.AutoEntityProvider = AutoEntityProvider;
		}

		public EntityManagerCapability Capabilities => AutoEntityProvider.Capabilities;

		public Task<TKey> CreateAsync(TEntityEditable Entity)
		{
			return AutoEntityProvider.CreateAsync( Entity);
		}

		public Task<TEntityDetail> GetAsync(TKey Id)
		{
			return AutoEntityProvider.GetAsync(Id);
		}

		public Task<TEntityDetail[]> GetAsync(TKey[] Ids)
		{
			return AutoEntityProvider.GetAsync(Ids);
		}

		public Task<TEntityEditable> LoadForEdit(TKey Id)
		{
			return AutoEntityProvider.LoadForEdit(Id);
		}

		public Task<QueryResult<TEntitySummary>> QueryAsync(TQueryArgument Arg, Paging paging)
		{
			return AutoEntityProvider.QueryAsync(Arg, paging);
		}

		public Task<QueryResult<TKey>> QueryIdentsAsync(TQueryArgument Arg, Paging paging)
		{
			return AutoEntityProvider.QueryIdentsAsync(Arg, paging);
		}

		public Task RemoveAllAsync()
		{
			return AutoEntityProvider.RemoveAllAsync();
		}

		public Task RemoveAsync(TKey Key)
		{
			return AutoEntityProvider.RemoveAsync(Key);
		}

		public Task UpdateAsync(TEntityEditable Entity)
		{
			return AutoEntityProvider.UpdateAsync(Entity);
		}
	}


}
