using System;
using System.Threading.Tasks;

namespace SF.Entities.AutoEntityProvider
{

	public class AutoEntityManager<TKey,TDetail, TSummary,TEditable, TQueryArgument> :
		IEntityLoadable<TKey, TDetail>,
		IEntityBatchLoadable<TKey, TDetail>,
		IEntityQueryable<TSummary, TQueryArgument>,
		IEntityManagerCapabilities,
		IEntityEditableLoader<TKey, TEditable>,
		IEntityCreator<TKey, TEditable>,
		IEntityUpdator<TEditable>,
		IEntityRemover<TKey>,
		IEntityAllRemover
		where TDetail : class
		where TSummary : class
		where TEditable : class
		where TQueryArgument : class
	{
		protected IDataSetAutoEntityProvider<TKey,TDetail, TSummary, TEditable, TQueryArgument> AutoEntityProvider { get; }
		protected IDataSetEntityManager EntityManager => AutoEntityProvider.EntityManager;
		public AutoEntityManager(
			IDataSetAutoEntityProvider<TKey, TDetail, TSummary, TEditable, TQueryArgument> AutoEntityProvider
			)
		{
			this.AutoEntityProvider = AutoEntityProvider;
		}

		public EntityManagerCapability Capabilities => AutoEntityProvider.Capabilities;

		public Task<TKey> CreateAsync(TEditable Entity)
		{
			return AutoEntityProvider.CreateAsync( Entity);
		}

		public Task<TDetail> GetAsync(TKey Id)
		{
			return AutoEntityProvider.GetAsync(Id);
		}

		public Task<TDetail[]> GetAsync(TKey[] Ids)
		{
			return AutoEntityProvider.GetAsync(Ids);
		}

		public Task<TEditable> LoadForEdit(TKey Id)
		{
			return AutoEntityProvider.LoadForEdit(Id);
		}

		public Task<QueryResult<TSummary>> QueryAsync(TQueryArgument Arg, Paging paging)
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

		public Task UpdateAsync(TEditable Entity)
		{
			return AutoEntityProvider.UpdateAsync(Entity);
		}
	}


}
