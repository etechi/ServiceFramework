using System;
using System.Threading.Tasks;

namespace SF.Entities.AutoEntityProvider
{

	public class AutoEntityManager<TDetail, TSummary,TEditable, TQueryArgument> :
		IEntityLoadable<TDetail>,
		IEntityBatchLoadable<TDetail>,
		IEntityQueryable<TSummary, TQueryArgument>,
		IEntityManagerCapabilities,
		IEntityEditableLoader<TEditable>,
		IEntityCreator<TEditable>,
		IEntityUpdator<TEditable>,
		IEntityRemover<TEditable>,
		IEntityAllRemover
		where TDetail : class
		where TSummary : class
		where TEditable : class
		where TQueryArgument : class
	{
		protected IDataSetAutoEntityProvider<TDetail, TSummary, TEditable, TQueryArgument> AutoEntityProvider { get; }
		protected IDataSetEntityManager EntityManager => AutoEntityProvider.EntityManager;
		public AutoEntityManager(
			IDataSetAutoEntityProvider<TDetail, TSummary, TEditable, TQueryArgument> AutoEntityProvider
			)
		{
			this.AutoEntityProvider = AutoEntityProvider;
		}

		public EntityManagerCapability Capabilities => AutoEntityProvider.Capabilities;

		public Task<TEditable> CreateAsync(TEditable Entity)
		{
			return AutoEntityProvider.CreateAsync( Entity);
		}

		public Task<TDetail> GetAsync(TDetail Id)
		{
			return AutoEntityProvider.GetAsync(Id);
		}

		public Task<TDetail[]> GetAsync(TDetail[] Ids)
		{
			return AutoEntityProvider.GetAsync(Ids);
		}

		public Task<TEditable> LoadForEdit(TEditable Id)
		{
			return AutoEntityProvider.LoadForEdit(Id);
		}

		public Task<QueryResult<TSummary>> QueryAsync(TQueryArgument Arg, Paging paging)
		{
			return AutoEntityProvider.QueryAsync(Arg, paging);
		}

		public Task<QueryResult<TSummary>> QueryIdentsAsync(TQueryArgument Arg, Paging paging)
		{
			return AutoEntityProvider.QueryIdentsAsync(Arg, paging);
		}

		public Task RemoveAllAsync()
		{
			return AutoEntityProvider.RemoveAllAsync();
		}

		public Task<TEditable> RemoveAsync(TEditable Key)
		{
			return AutoEntityProvider.RemoveAsync(Key);
		}

		public Task<TEditable> UpdateAsync(TEditable Entity)
		{
			return AutoEntityProvider.UpdateAsync(Entity);
		}
	}


}
