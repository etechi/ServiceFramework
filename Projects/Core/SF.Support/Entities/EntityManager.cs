using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Data;
using System.Reflection;

namespace SF.Entities
{
	public class IdQueryArgument<TKey> : IQueryArgument<TKey>
		where TKey : IEquatable<TKey>
	{
		public Option<TKey> Id { get; set; }
	}
	public abstract class EntityManager<TKey, TPublic, TEditable, TModel> :
		EntityManager<TKey, TPublic, TPublic, IdQueryArgument<TKey>, TEditable, TModel>
		where TPublic : class, IEntityWithId<TKey>
		where TKey : IEquatable<TKey>
		where TModel : class, IEntityWithId<TKey>, new()
		where TEditable : class, IEntityWithId<TKey>
	{
		public EntityManager(IDataSetEntityManager<TModel> EntityManager) : base(EntityManager)
		{
		}
		protected override async Task<TPublic[]> OnPreparePublics(TPublic[] Internals)
		{
			await EntityManager.DataEntityResolver.Fill(Internals);
			return Internals;
		}
		protected override PagingQueryBuilder<TModel> PagingQueryBuilder => new PagingQueryBuilder<TModel>(
			"id",
			b => b.Add("id", m => m.Id)
			);
		protected override IContextQueryable<TModel> OnBuildQuery(IContextQueryable<TModel> Query, IdQueryArgument<TKey> Arg, Paging paging)
		{
			return Query;
		}
	}

	public abstract class EntityManager<TKey, TPublic, TQueryArgument, TEditable, TModel> :
		EntityManager<TKey, TPublic, TPublic, TQueryArgument, TEditable, TModel>
		where TPublic : class, IEntityWithId<TKey>
		where TKey : IEquatable<TKey>
		where TModel : class, IEntityWithId<TKey>, new()
		where TQueryArgument : class, IQueryArgument<TKey>,new()
		where TEditable : class, IEntityWithId<TKey>
	{
		public EntityManager(IDataSetEntityManager<TModel> EntityManager) : base(EntityManager)
		{
		}
		protected override async Task<TPublic[]> OnPreparePublics(TPublic[] Internals)
		{
			await EntityManager.DataEntityResolver.Fill(Internals);
			return Internals;
		}
	}
	public abstract class EntityManager<TKey, TPublic, TTemp, TQueryArgument, TEditable,TModel> :
		QuerableEntitySource<TKey, TPublic, TTemp, TQueryArgument, TModel>,
		IEntityManager<TKey, TEditable>
		where TPublic : class, IEntityWithId<TKey>
		where TKey : IEquatable<TKey>
		where TModel : class, IEntityWithId<TKey>,new()
		where TQueryArgument : class, IQueryArgument<TKey>,new()
		where TEditable : class,IEntityWithId<TKey>
	{
		public interface IModifyContext : IEntityModifyContext<TKey, TEditable, TModel>
		{

		}
		class ModifyContext : EntityModifyContext<TKey, TEditable, TModel>, IModifyContext
		{

		}
        public EntityManager(IDataSetEntityManager<TModel> EntityManager) :base(EntityManager)
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
			return ctx.Id;
		}
		protected virtual Task InternalCreateAsync(IModifyContext Context, TEditable obj,object ExtraArgument)
		{
			return EntityManager.InternalCreateAsync<TKey,TEditable,TModel,IModifyContext>(Context, obj, OnUpdateModel, OnNewModel, ExtraArgument);
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
		protected virtual Task<bool> InternalRemoveAsync(IModifyContext Context,TKey Id)
		{
			return EntityManager.InternalRemoveAsync<TKey,TModel,TEditable,IModifyContext>(
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
			await EntityManager.RemoveAllAsync<TKey,TModel>(
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
				throw new ArgumentException($"找不到对象:{GetType().Comment()}:{obj.Id}");
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
			return Query.Select(EntityMapper.Map<TModel, TEditable>()).SingleOrDefaultAsync();
		}

		public virtual Task<TEditable> LoadForEdit(TKey Id)
		{
			return EntityManager.LoadForEdit(Id, OnMapModelToEditable);
		}
	}
    
   
}