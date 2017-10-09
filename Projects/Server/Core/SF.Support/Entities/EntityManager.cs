using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Data;
using System.Reflection;

namespace SF.Entities
{

	public abstract class ModidifiableEntityManager<TKey, TPublic, TEditable, TModel> :
		ModidifiableEntityManager<ObjectKey<TKey>, TPublic, TPublic, QueryArgument<ObjectKey<TKey>>, TEditable, TModel>
		where TPublic : class
		where TModel : class, new()
		where TEditable : class
		where TKey:IEquatable<TKey>
	{
		public ModidifiableEntityManager(IDataSetEntityManager<TEditable,TModel> EntityManager) : base(EntityManager)
		{
		}
		protected override async Task<TPublic[]> OnPrepareDetails(TPublic[] Internals)
		{
			await EntityManager.DataEntityResolver.Fill(ServiceInstanceDescriptor.InstanceId, Internals);
			return Internals;
		}
		protected override PagingQueryBuilder<TModel> PagingQueryBuilder => new PagingQueryBuilder<TModel>(
			"id",
			b => b.Add("id",Entity<TModel>.SingleKeySelector<TKey>())
			);
		protected override IContextQueryable<TModel> OnBuildQuery(IContextQueryable<TModel> Query, QueryArgument<ObjectKey<TKey>> Arg, Paging paging)
		{
			return Query;
		}
	}

	public abstract class ModidifiableEntityManager<TKey, TPublic, TQueryArgument, TEditable, TModel> :
		ModidifiableEntityManager<TKey, TPublic, TPublic, TQueryArgument, TEditable, TModel>
		where TPublic : class
		where TModel : class, new()
		where TQueryArgument : class,new()
		where TEditable : class
	{
		public ModidifiableEntityManager(IDataSetEntityManager<TEditable,TModel> EntityManager) : base(EntityManager)
		{
		}
		protected override async Task<TPublic[]> OnPrepareDetails(TPublic[] Internals)
		{
			await EntityManager.DataEntityResolver.Fill(ServiceInstanceDescriptor?.InstanceId, Internals);
			return Internals;
		}
	}
	public abstract class ModidifiableEntityManager<TKey,TPublic, TTemp, TQueryArgument, TEditable,TModel> :
		QuerableEntitySource<TKey, TPublic, TTemp, TQueryArgument, TModel>,
		IEntityManager<TKey, TEditable>
		where TPublic : class
		where TModel : class,new()
		where TQueryArgument : class,new()
		where TEditable : class
	{
		new protected IDataSetEntityManager<TEditable, TModel> EntityManager => (IDataSetEntityManager < TEditable, TModel > )base.EntityManager;
		public interface IModifyContext : IEntityModifyContext<TEditable, TModel>
		{

		}
		class ModifyContext : EntityModifyContext<TEditable, TModel>, IModifyContext
		{

		}
        public ModidifiableEntityManager(IDataSetEntityManager<TEditable,TModel> EntityManager) :base(EntityManager)
        {
        }

		public virtual EntityManagerCapability Capabilities => EntityManagerCapability.All;
		public bool AutoSaveChanges { get; set; } = true;


		#region create

		protected virtual Task OnNewModel(IModifyContext ctx)
		{
			return Task.CompletedTask;
		}
		protected IModifyContext NewModifyContext()
			=> new ModifyContext();
		public virtual async Task<TKey> CreateAsync(TEditable obj)
		{
			var ctx = NewModifyContext();
			await InternalCreateAsync(ctx, obj, null);
			return Entity<TModel>.GetKey<TKey>(ctx.Model);
		}
		protected virtual Task InternalCreateAsync(IModifyContext Context, TEditable obj,object ExtraArgument)
		{
			return EntityManager.InternalCreateAsync<TKey,TEditable, TModel,IModifyContext>(
				Context, 
				obj, 
				OnUpdateModel, 
				OnNewModel, 
				ExtraArgument
				);
		}
		#endregion


		#region delete

		public virtual async Task RemoveAsync(TKey Id)
		{
			var ctx = NewModifyContext();
			var re =await InternalRemoveAsync(ctx,Id);
			if (!re)
				throw new ArgumentException($"找不到对象:{GetType().Comment()}:{Id}");
		}
		protected virtual Task<bool> InternalRemoveAsync(IModifyContext Context, TKey Id)
		{
			return EntityManager.InternalRemoveAsync<TKey,TEditable, TModel, IModifyContext>(
				Context, 
				Id, 
				OnRemoveModel,
				OnLoadModelForUpdate
				);
		}

		protected virtual Task OnRemoveModel(IModifyContext ctx)
		{
			EntityManager.DataSet.Remove(ctx.Model);
			return Task.CompletedTask;
		}

		public virtual async Task RemoveAllAsync()
		{
			await EntityManager.RemoveAllAsync<TKey,TEditable,TModel>(
				RemoveAsync
				);
		}

		#endregion


		#region Update
		protected abstract Task OnUpdateModel(IModifyContext ctx);

		public virtual async Task UpdateAsync(TEditable obj)
		{
			var ctx = NewModifyContext();
			var re =await InternalUpdateAsync(ctx,obj);
			if (!re)
				throw new ArgumentException($"找不到对象:{GetType().Comment()}:{Entity<TEditable>.GetIdentString(obj)}");
		}
		protected virtual async Task<bool> InternalUpdateAsync(IModifyContext Context,TEditable obj)
		{
			return await EntityManager.InternalUpdateAsync<TKey,TEditable,TModel,IModifyContext>(
				Context,
				obj, 
				OnUpdateModel, 
				OnLoadModelForUpdate
				);
		}

        #endregion

        protected virtual IContextQueryable<TModel> OnLoadChildObjectsForUpdate(TKey Id,IContextQueryable<TModel> query)
		{
			return query;
		}
		protected virtual Task<TModel> OnLoadModelForUpdate(TKey Id,IContextQueryable<TModel> ctx)
		{
			return OnLoadChildObjectsForUpdate(Id,ctx).SingleOrDefaultAsync();
		}

		protected virtual Task<TEditable> OnMapModelToEditable(IContextQueryable<TModel> Query)
		{
			return Query.Select(ADT.Poco.MapExpression<TModel, TEditable>()).SingleOrDefaultAsync();
		}

		public virtual Task<TEditable> LoadForEdit(TKey Key)
		{
			return EntityManager.LoadForEdit(Key, OnMapModelToEditable);
		}
	}
    
   
}