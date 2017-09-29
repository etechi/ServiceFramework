using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using SF.Data;
using SF.Core.Times;
using SF.Core.Logging;
using SF.Core.ServiceManagement;
using System.Collections.Generic;
using SF.ADT;

namespace SF.Entities
{
	public abstract class BaseEntityManager: IManagedServiceWithId
	{
		protected IEntityManager EntityManager { get; }
		public IServiceInstanceDescriptor ServiceInstanceDescriptor => EntityManager.ServiceInstanceDescroptor;
		public DateTime Now => EntityManager.Now;
		public ILogger Logger => EntityManager.Logger;
		public IIdentGenerator IdentGenerator => EntityManager.IdentGenerator;
		
		long IManagedServiceWithId.ServiceInstanceId => EntityManager.ServiceInstanceDescroptor.InstanceId;

		public BaseEntityManager(IEntityManager EntityManager)
		{
			this.EntityManager = EntityManager;
		}
	}

	public abstract class BaseDataSetEntityManager<TModel>:
		BaseEntityManager
		where TModel:class
	{
		protected new IReadOnlyDataSetEntityManager<TModel> EntityManager => (IReadOnlyDataSetEntityManager<TModel>)base.EntityManager;
		public IDataSet<TModel> DataSet => EntityManager.DataSet;
		public IDataContext DataContext => DataSet.Context;

		public BaseDataSetEntityManager(IReadOnlyDataSetEntityManager<TModel> EntityManager):base(EntityManager)
		{
		}
	}

	public abstract class EntitySource<TEntityDetail, TModel> :
		EntitySource<TEntityDetail, TEntityDetail, TModel>
		where TEntityDetail : class
		where TModel : class
	{
		public EntitySource(IReadOnlyDataSetEntityManager<TModel> EntityManager) : base(EntityManager)
		{
		}
		protected override async Task<TEntityDetail[]> OnPrepareDetails(TEntityDetail[] Internals)
		{
			await EntityManager.DataEntityResolver.Fill(ServiceInstanceDescriptor.InstanceId, Internals);
			return Internals;
		}
	}
	
	public abstract class EntitySource<TDetail, TDetailTemp, TModel> :
		BaseDataSetEntityManager<TModel>,
		IEntityLoadable<TDetail>,
		IEntityBatchLoadable<TDetail>
		where TDetail : class
		where TModel: class
	{
		public EntitySource(IReadOnlyDataSetEntityManager<TModel> EntityManager):base(EntityManager)
		{
		}
		protected virtual IContextQueryable<TDetailTemp> OnMapModelToDetail(IContextQueryable<TModel> Query)
		{
			return Query.Select(Poco.Map<TModel, TDetailTemp>());
		}
		protected abstract Task<TDetail[]> OnPrepareDetails(TDetailTemp[] Internals);

		public Task<TDetail[]> GetAsync(TDetail[] Ids)
		{
			return EntityManager.BatchGetAsync<TDetailTemp,TDetail,TModel>(Ids, OnMapModelToDetail,OnPrepareDetails);
		}

		public Task<TDetail> GetAsync(TDetail Id)
		{
			return EntityManager.GetAsync<TDetailTemp, TDetail, TModel>(Id, OnMapModelToDetail, OnPrepareDetails);
		}
	}
	public abstract class ConstantEntitySource<TKey, TEntityDetail> :
		ConstantEntitySource<TKey, TEntityDetail, TEntityDetail>
		where TEntityDetail : class, IEntityWithId<TKey>
		where TKey : IEquatable<TKey>
	{
		public ConstantEntitySource(IEntityManager EntityManager, IReadOnlyDictionary<TKey, TEntityDetail> Models) : base(EntityManager, Models)
		{
		}

		protected override IContextQueryable<TEntityDetail> OnMapModelToInternal(IContextQueryable<TEntityDetail> Query)
		{
			return Query;
		}
	}
	public abstract class ConstantEntitySource<TKey, TEntityDetail, TModel> :
	   ConstantEntitySource<TKey, TEntityDetail, TEntityDetail, TModel>
		where TEntityDetail : class, IEntityWithId<TKey>
		where TModel : class, IEntityWithId<TKey>
		where TKey : IEquatable<TKey>
	{
		public ConstantEntitySource(IEntityManager EntityManager, IReadOnlyDictionary<TKey, TModel> Models) : base(EntityManager, Models)
		{
		}

		protected override Task<TEntityDetail[]> OnPrepareInternals(TEntityDetail[] Internals)
		{
			return Task.FromResult(Internals);
		}
	}
	public abstract class ConstantEntitySource<TKey,TEntityDetail,TTemp,TModel> :
		BaseEntityManager,
		IEntityLoadable< TEntityDetail>,
		IEntityBatchLoadable<TEntityDetail>
		where TEntityDetail : class, IEntityWithId<TKey>
		where TModel:class,IEntityWithId<TKey>
		where TKey : IEquatable<TKey>
	{

		protected IReadOnlyDictionary<TKey, TModel> Models { get; }
		public ConstantEntitySource(IEntityManager EntityManager, IReadOnlyDictionary<TKey, TModel> Models) :base(EntityManager)
		{
			this.Models = Models;
		}
		protected virtual IContextQueryable<TTemp> OnMapModelToInternal(IContextQueryable<TModel> Query)
		{
			return Query.Select(ADT.Poco.Map<TModel, TTemp>());
		}
		protected abstract Task<TEntityDetail[]> OnPrepareInternals(TTemp[] Internals);
		public async Task<TEntityDetail> GetAsync(TEntityDetail Id)
		{
			if (Models.TryGetValue(Entity<TEntityDetail>.GetSingleKey<TKey>(Id), out var m))
			{
				var re = await OnPrepareInternals(OnMapModelToInternal(new[] { m }.AsContextQueryable()).ToArray());
				if (re == null || re.Length == 0)
					return null;
				return re[0];
			}
			else
				return null;
		}

		public async Task<TEntityDetail[]> GetAsync(TEntityDetail[] Ids)
		{
			var re = await OnPrepareInternals(
				OnMapModelToInternal(
					Ids.Select(id => Models.Get(Entity<TEntityDetail>.GetSingleKey<TKey>(id)))
					.Where(m => m != null)
					.AsContextQueryable()
					).ToArray());
			return re;
		}
	}




}