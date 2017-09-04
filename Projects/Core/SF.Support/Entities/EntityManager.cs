using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Data;

namespace SF.Entities
{
	
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
		
        public EntityManager(IDataSetEntityManager<TModel> EntityManager) :base(EntityManager)
        {
        }

		public virtual EntityManagerCapability Capabilities => EntityManagerCapability.All;
		public bool AutoSaveChanges { get; set; } = true;


		#region create

		protected virtual Task OnNewModel(IEntityModifyContext<TKey,TModel> ctx)
		{
			return Task.CompletedTask;
		}
		
		public virtual async Task<TKey> CreateAsync(TEditable obj)
		{
			var re=await InternalCreateAsync(obj, null);
			return re.Id;
		}
		protected virtual Task<IEntityModifyContext<TKey,TEditable, TModel>> InternalCreateAsync(TEditable obj,object ExtraArgument)
		{
			return EntityManager.InternalCreateAsync<TKey,TEditable,TModel>(obj, OnUpdateModel, OnNewModel, ExtraArgument);
		}
		#endregion


		#region delete

		public virtual async Task RemoveAsync(TKey Id)
		{
			await InternalRemoveAsync(Id);
		}
		protected virtual Task<IEntityModifyContext<TKey, TModel>> InternalRemoveAsync(TKey Id)
		{
			return EntityManager.InternalRemoveAsync(Id, OnRemoveModel,OnLoadModelForUpdate);
		}

		protected virtual Task OnRemoveModel(IEntityModifyContext<TKey, TModel> ctx)
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
		protected abstract Task OnUpdateModel(IEntityModifyContext<TKey,TEditable, TModel> ctx);

		public virtual async Task UpdateAsync(TEditable obj)
		{
			await InternalUpdateAsync(obj);
        }
		protected virtual async Task<IEntityModifyContext<TKey,TEditable,TModel>> InternalUpdateAsync(TEditable obj)
		{
			var ctx = await EntityManager.InternalUpdateAsync<TKey,TEditable,TModel>(
				obj, 
				OnUpdateModel, 
				OnLoadModelForUpdate
				);
			return ctx;
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