using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Data.Storage;

namespace SF.Data.Entity
{
	public enum ModifyAction
	{
		Create,
		Update,
		Delete
	}
	public abstract class EntityManager<TKey, TPublic, TQueryArgument, TEditable, TModel> :
		EntityManager<TKey, TPublic, TPublic, TQueryArgument, TEditable, TModel>
		where TPublic : class, IObjectWithId<TKey>
		where TKey : IEquatable<TKey>
		where TModel : class, IObjectWithId<TKey>, new()
		where TQueryArgument : class, IQueryArgument<TKey>
		where TEditable : class, IObjectWithId<TKey>
	{
		public EntityManager(IDataSet<TModel> DataSet) : base(DataSet)
		{
		}
	}
	public abstract class EntityManager<TKey, TPublic, TTemp, TQueryArgument, TEditable,TModel> :
		QuerableEntitySource<TKey, TPublic, TTemp, TQueryArgument, TModel>,
		IEntityManager<TKey, TEditable>
		where TPublic : class, IObjectWithId<TKey>
		where TKey : IEquatable<TKey>
		where TModel : class, IObjectWithId<TKey>,new()
		where TQueryArgument : class, IQueryArgument<TKey>
		where TEditable : class,IObjectWithId<TKey>
	{
		public class ModifyContext 
		{
			class ActionItem
			{
				public bool CallOnSaved { get; set; }
				public Action Func { get; set; }
				public Func<Task> AsyncFunc { get; set; }
			}

			List<ActionItem> _postActions;
			EntityManager<TKey, TPublic, TTemp, TQueryArgument, TEditable, TModel> Manager { get; }
			public ModifyAction Action { get; }
			public TKey Id { get; set; }
			public TEditable Editable { get; set; }
			public TModel Model { get; set; }
			public object OwnerId { get; set; }
			public IDataSet<TModel> ModelSet { get; }

			
			public void AddPostAction(
				Action action,
				bool CallOnSaved = true
				)
			{
				if (_postActions == null)
					_postActions = new List<ActionItem>();
				_postActions.Add(new ActionItem { Func = action, CallOnSaved = CallOnSaved });
			}
			public void AddPostAction(
			   Func<Task> action,
			   bool CallOnSaved = true
			   )
			{
				if (_postActions == null)
					_postActions = new List<ActionItem>();
				_postActions.Add(new ActionItem { AsyncFunc = action, CallOnSaved = CallOnSaved });
			}
			public async Task ExecutePostActionsAsync(bool ChangedSaved)
			{
				if (_postActions == null)
					return;
				foreach (var action in _postActions)
					if (ChangedSaved || !action.CallOnSaved)
					{
						action.Func?.Invoke();
						if (action.AsyncFunc != null)
							await action.AsyncFunc();
					}
			}
			public ModifyContext(
				EntityManager<TKey, TPublic, TTemp, TQueryArgument, TEditable, TModel> Manager,
				IDataSet<TModel> ModelSet,
				ModifyAction Action
				)
			{
				this.Manager = Manager;
				this.ModelSet = ModelSet;
				this.Action = Action;
			}
		}

        protected virtual int RetryForConcurrencyExceptionCount => 0;
        public EntityManager(IDataSet<TModel> DataSet) :base(DataSet)
        {
        }

		public virtual EntityManagerCapability Capabilities => EntityManagerCapability.All;
		public bool AutoSaveChanges { get; set; } = true;

		public async Task<bool> SaveChangesAsync()
		{
			if (AutoSaveChanges)
			{
				await OnSaveChangesAsync();
				return true;
			}
			return false;
		}
		protected virtual Task OnSaveChangesAsync()
		{
			return DataSet.Context.SaveChangesAsync();	
		}

		#region create

		protected virtual Task OnNewModel(ModifyContext ctx)
		{
			return Task.CompletedTask;
		}
		protected virtual async Task OnCreateAsync(ModifyContext ctx)
		{
			ctx.Model = new TModel();
			await OnNewModel(ctx);
			await OnUpdateModel(ctx);
			DataSet.Add(ctx.Model);
		}

        
		public virtual async Task<TKey> CreateAsync(TEditable obj)
		{
            ModifyContext ctx = null;
            var saved=await DataSet.RetryForConcurrencyExceptionAsync(async () =>
            {
				ctx = new ModifyContext(this, DataSet, ModifyAction.Create)
				{
					Editable = obj
				};
				await OnCreateAsync(ctx);
                return await SaveChangesAsync();
            }, RetryForConcurrencyExceptionCount);

            ctx.Id = ctx.Model.Id;
            await ctx.ExecutePostActionsAsync(saved);

            return ctx.Model.Id;
		}
		#endregion


		#region delete

		public virtual async Task DeleteAsync(TKey Id)
		{
            ModifyContext ctx = null;
            var saved = await DataSet.RetryForConcurrencyExceptionAsync(async () =>
            {
				ctx = new ModifyContext(this, DataSet, ModifyAction.Delete)
				{
					Id = Id
				};
				await OnDeleteAsync(ctx);

                return await SaveChangesAsync();
            }, RetryForConcurrencyExceptionCount);
            await ctx.ExecutePostActionsAsync(saved);
		}
		protected virtual async Task OnDeleteAsync(ModifyContext ctx)
		{
			ctx.Model = await OnLoadModelForUpdate(ctx);
			await OnRemoveModel(ctx);
		}

		protected virtual Task OnRemoveModel(ModifyContext ctx)
		{
			DataSet.Remove(ctx.Model);
			return Task.CompletedTask;
		}
		#endregion


		#region Update
		protected abstract Task OnUpdateModel(ModifyContext ctx);

		protected virtual async Task OnUpdateAsync(ModifyContext ctx)
		{
			ctx.Model = await OnLoadModelForUpdate(ctx);
			await OnUpdateModel(ctx);
			DataSet.Update(ctx.Model);
		}
		public virtual async Task UpdateAsync(TEditable obj)
		{
            ModifyContext ctx = null;
            var saved = await DataSet.RetryForConcurrencyExceptionAsync(async () =>
            {
				ctx = new ModifyContext(this, DataSet, ModifyAction.Update)
				{
					Editable = obj,
					Id = obj.Id
				};
				await OnUpdateAsync(ctx);
                return await SaveChangesAsync();
            }, RetryForConcurrencyExceptionCount);
            await ctx.ExecutePostActionsAsync(saved);
                
        }

        #endregion

        protected virtual IContextQueryable<TModel> OnLoadChildObjectsForUpdate(ModifyContext ctx,IContextQueryable<TModel> query)
		{
			return query;
		}
		protected virtual Task<TModel> OnLoadModelForUpdate(ModifyContext ctx)
		{
            var id = ctx.Id;
			return OnLoadChildObjectsForUpdate(ctx, DataSet.AsQueryable(false).Where(s => s.Id.Equals(id)))
				.SingleOrDefaultAsync();
		}

		protected abstract Task<TEditable> MapModelToEditable(IContextQueryable<TModel> Query);

		public virtual Task<TEditable> LoadForEdit(TKey Id)
		{
			return MapModelToEditable(DataSet.AsQueryable(false).Where(m => m.Id.Equals(Id)));
		}
	}
    
   
}