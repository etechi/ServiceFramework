using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Data;
using System.Reflection;

namespace SF.Entities
{

	public abstract class ModidifiableEntityManager<TPublic, TEditable, TModel> :
		ModidifiableEntityManager<TPublic, TPublic, QueryArgument, TEditable, TModel>
		where TPublic : class
		where TModel : class, new()
		where TEditable : class
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
			null//b => b.Add("id", Entity<TModel>.KeyFilter.SingleKeySelector( m.Id)
			);
		protected override IContextQueryable<TModel> OnBuildQuery(IContextQueryable<TModel> Query, QueryArgument Arg, Paging paging)
		{
			return Query;
		}
	}

	public abstract class ModidifiableEntityManager<TPublic, TQueryArgument, TEditable, TModel> :
		ModidifiableEntityManager<TPublic, TPublic, TQueryArgument, TEditable, TModel>
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
			await EntityManager.DataEntityResolver.Fill(ServiceInstanceDescriptor.InstanceId, Internals);
			return Internals;
		}
	}
	public abstract class ModidifiableEntityManager<TPublic, TTemp, TQueryArgument, TEditable,TModel> :
		QuerableEntitySource<TPublic, TTemp, TQueryArgument, TModel>,
		IEntityManager<TEditable>
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
		public virtual async Task<TEditable> CreateAsync(TEditable obj)
		{
			var ctx = NewModifyContext();
			await InternalCreateAsync(ctx, obj, null);
			return ctx.Editable;
		}
		protected virtual Task InternalCreateAsync(IModifyContext Context, TEditable obj,object ExtraArgument)
		{
			return EntityManager.InternalCreateAsync<TEditable,TModel,IModifyContext>(
				Context, 
				obj, 
				OnUpdateModel, 
				OnNewModel, 
				ExtraArgument
				);
		}
		#endregion


		#region delete

		public virtual async Task<TEditable> RemoveAsync(TEditable Id)
		{
			var ctx = NewModifyContext();
			var re =await InternalRemoveAsync(ctx,Id);
			if (re==null)
				throw new ArgumentException($"找不到对象:{GetType().Comment()}:{Id}");
			return re;
		}
		protected virtual Task<TEditable> InternalRemoveAsync(IModifyContext Context, TEditable Id)
		{
			return EntityManager.InternalRemoveAsync<TModel,TEditable,IModifyContext>(
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
			await EntityManager.RemoveAllAsync<TEditable,TModel>(
				RemoveAsync
				);
		}

		#endregion


		#region Update
		protected abstract Task OnUpdateModel(IModifyContext ctx);

		public virtual async Task<TEditable> UpdateAsync(TEditable obj)
		{
			var ctx = NewModifyContext();
			var re =await InternalUpdateAsync(ctx,obj);
			if (re==null)
				throw new ArgumentException($"找不到对象:{GetType().Comment()}:{Entity<TEditable>.GetIdentString(obj)}");
			return re;
		}
		protected virtual async Task<TEditable> InternalUpdateAsync(IModifyContext Context,TEditable obj)
		{
			return await EntityManager.InternalUpdateAsync<TEditable,TModel,IModifyContext>(
				Context,
				obj, 
				OnUpdateModel, 
				OnLoadModelForUpdate
				);
		}

        #endregion

        protected virtual IContextQueryable<TModel> OnLoadChildObjectsForUpdate(TEditable Id,IContextQueryable<TModel> query)
		{
			return query;
		}
		protected virtual Task<TModel> OnLoadModelForUpdate(TEditable Id,IContextQueryable<TModel> ctx)
		{
			return OnLoadChildObjectsForUpdate(Id,ctx).SingleOrDefaultAsync();
		}

		protected virtual Task<TEditable> OnMapModelToEditable(IContextQueryable<TModel> Query)
		{
			return Query.Select(ADT.Poco.Map<TModel, TEditable>()).SingleOrDefaultAsync();
		}

		public virtual Task<TEditable> LoadForEdit(TEditable Id)
		{
			return EntityManager.LoadForEdit(Id, OnMapModelToEditable);
		}
	}
    
   
}