using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using SF.Data;
using SF.Core.Times;
using SF.Core.Logging;
using SF.Core.ServiceManagement;

namespace SF.Entities
{
	public abstract class BaseEntityManager<TModel>
		where TModel:class
	{
		protected IDataSetEntityManager<TModel> EntityManager { get; }
		public IServiceInstanceDescriptor ServiceInstanceDescriptor => EntityManager.ServiceInstanceDescroptor;
		public ITimeService TimeService => EntityManager.TimeService;
		public ILogger Logger => EntityManager.Logger;
		public IDataSet<TModel> DataSet => EntityManager.DataSet;
		public IIdentGenerator IdentGenerator => EntityManager.IdentGenerator;

		public BaseEntityManager(IDataSetEntityManager<TModel> EntityManager)
		{
			this.EntityManager = EntityManager;
		}
	}
	public abstract class EntitySource<TKey, TPublic, TModel> :
		EntitySource<TKey, TPublic, TPublic, TModel>
		where TPublic : class, IEntityWithId<TKey>
		where TKey : IEquatable<TKey>
		where TModel : class, IEntityWithId<TKey>
	{
		public EntitySource(IDataSetEntityManager<TModel> EntityManager) : base(EntityManager)
		{
		}
		protected override async Task<TPublic[]> OnPreparePublics(TPublic[] Internals)
		{
			await EntityManager.DataEntityResolver.Fill(Internals);
			return Internals;
		}
	}
	
	public abstract class EntitySource<TKey, TPublic, TTemp, TModel> :
		BaseEntityManager<TModel>,
		IEntityLoadable<TKey, TPublic>,
		IEntityBatchLoadable<TKey, TPublic>
		where TPublic : class, IEntityWithId<TKey>
		where TKey : IEquatable<TKey>
		where TModel: class,IEntityWithId<TKey>
	{
		public EntitySource(IDataSetEntityManager<TModel> EntityManager):base(EntityManager)
		{
		}
		protected virtual IContextQueryable<TTemp> OnMapModelToPublic(IContextQueryable<TModel> Query)
		{
			return Query.Select(EntityMapper.Map<TModel, TTemp>());
		}
		protected abstract Task<TPublic[]> OnPreparePublics(TTemp[] Internals);

		public Task<TPublic[]> GetAsync(TKey[] Ids)
		{
			return EntityManager.GetAsync<TKey,TTemp,TPublic,TModel>(Ids, OnMapModelToPublic,OnPreparePublics);
		}

		public Task<TPublic> GetAsync(TKey Id)
		{
			return EntityManager.GetAsync<TKey, TTemp, TPublic, TModel>(Id, OnMapModelToPublic, OnPreparePublics);
		}
	}
	
    
   
}