using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Data.Entity;

namespace SF.Data.Services
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
				public bool callOnSaved { get; set; }
				public Action func { get; set; }
				public Func<Task> asyncFunc { get; set; }
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
				_postActions.Add(new ActionItem { func = action, callOnSaved = CallOnSaved });
			}
			public void AddPostAction(
			   Func<Task> action,
			   bool CallOnSaved = true
			   )
			{
				if (_postActions == null)
					_postActions = new List<ActionItem>();
				_postActions.Add(new ActionItem { asyncFunc = action, callOnSaved = CallOnSaved });
			}
			public async Task ExecutePostActions(bool ChangedSaved)
			{
				if (_postActions == null)
					return;
				foreach (var action in _postActions)
					if (ChangedSaved || !action.callOnSaved)
					{
						action.func?.Invoke();
						if (action.asyncFunc != null)
							await action.asyncFunc();
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
		protected virtual async Task OnCreate(ModifyContext ctx)
		{
			ctx.Model = new TModel();
			await OnNewModel(ctx);
			await OnUpdateModel(ctx);
			DataSet.Add(ctx.Model);
		}

        
		public virtual async Task<TKey> Create(TEditable obj)
		{
            ModifyContext ctx = null;
            var saved=await DataSet.RetryForConcurrencyException(async () =>
            {
                ctx = new ModifyContext(this, DataSet, ModifyAction.Create);
                ctx.Editable = obj;
                await OnCreate(ctx);
                return await SaveChangesAsync();
            }, RetryForConcurrencyExceptionCount);

            ctx.Id = ctx.Model.Id;
            await ctx.ExecutePostActions(saved);

            return ctx.Model.Id;
		}
		#endregion


		#region delete

		public virtual async Task Delete(TKey Id)
		{
            ModifyContext ctx = null;
            var saved = await DataSet.RetryForConcurrencyException(async () =>
            {
                ctx = new ModifyContext(this, DataSet, ModifyAction.Delete);
                ctx.Id = Id;

                await OnDelete(ctx);

                return await SaveChangesAsync();
            }, RetryForConcurrencyExceptionCount);
            await ctx.ExecutePostActions(saved);
		}
		protected virtual async Task OnDelete(ModifyContext ctx)
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

		protected virtual async Task OnUpdate(ModifyContext ctx)
		{
			ctx.Model = await OnLoadModelForUpdate(ctx);
			await OnUpdateModel(ctx);
			DataSet.Update(ctx.Model);
		}
		public virtual async Task Update(TEditable obj)
		{
            ModifyContext ctx = null;
            var saved = await DataSet.RetryForConcurrencyException(async () =>
            {
                ctx = new ModifyContext(this, DataSet, ModifyAction.Update);
                ctx.Editable = obj;
                ctx.Id = obj.Id;
                await OnUpdate(ctx);
                return await SaveChangesAsync();
            }, RetryForConcurrencyExceptionCount);
            await ctx.ExecutePostActions(saved);
                
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

		public virtual Task<TEditable> LoadForUpdate(TKey Id)
		{
			return MapModelToEditable(DataSet.AsQueryable(false).Where(m => m.Id.Equals(Id)));
		}
	}
    
   
}