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
		public EntityManager(IDataSet<TModel> DataSet) : base(DataSet)
		{
		}
		protected override Task<TPublic[]> OnPreparePublics(TPublic[] Items)
		{
			return Task.FromResult(Items);
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
			public object UserData { get; set; }
			public object ExtraArgument { get; set; }
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

        [TransactionScope("创建对象")]
		public virtual async Task<TKey> CreateAsync(TEditable obj)
		{
			return await UseTransaction(async () =>
			{
				var ctx = await InternalCreateAsync(obj, null);
				return ctx.Model.Id;
			});
		}
		protected virtual async Task<ModifyContext> InternalCreateAsync(TEditable obj,object ExtraArgument)
		{
			ModifyContext ctx = null;
			var saved = await DataSet.RetryForConcurrencyExceptionAsync(async () =>
			{
				ctx = new ModifyContext(this, DataSet, ModifyAction.Create)
				{
					Editable = obj,
					ExtraArgument= ExtraArgument
				};
				await OnCreateAsync(ctx);
				return await SaveChangesAsync();
			}, RetryForConcurrencyExceptionCount);

			ctx.Id = ctx.Model.Id;
			await ctx.ExecutePostActionsAsync(saved);
			return ctx;
		}
		#endregion


		#region delete

		[TransactionScope("删除对象")]
		public virtual async Task RemoveAsync(TKey Id)
		{
			await UseTransaction(async () =>
			{
				await InternalRemoveAsync(Id);
				return 0;
			});
		}
		protected virtual async Task<ModifyContext> InternalRemoveAsync(TKey Id)
		{
			ModifyContext ctx = null;
			var saved = await DataSet.RetryForConcurrencyExceptionAsync(async () =>
			{
				ctx = new ModifyContext(this, DataSet, ModifyAction.Delete)
				{
					Id = Id
				};
				await OnRemoveAsync(ctx);

				return await SaveChangesAsync();
			}, RetryForConcurrencyExceptionCount);
			await ctx.ExecutePostActionsAsync(saved);
			return ctx;
		}
		protected virtual async Task OnRemoveAsync(ModifyContext ctx)
		{
			ctx.Model = await OnLoadModelForUpdate(ctx);
			await OnRemoveModel(ctx);
		}

		protected virtual Task OnRemoveModel(ModifyContext ctx)
		{
			DataSet.Remove(ctx.Model);
			return Task.CompletedTask;
		}

		public virtual async Task RemoveAllAsync()
		{
			await this.QueryAndRemoveAsync<EntityManager<TKey, TPublic, TTemp, TQueryArgument, TEditable, TModel>, TKey,TQueryArgument>(
				DataSet.Context.TransactionScopeManager
				);
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
		[TransactionScope("更新对象")]
		public virtual async Task UpdateAsync(TEditable obj)
		{
			await UseTransaction(async () =>
			{
				await InternalUpdateAsync(obj);
				return 0;
			});
        }
		protected virtual async Task<ModifyContext> InternalUpdateAsync(TEditable obj)
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
			return ctx;
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

		protected virtual Task<TEditable> OnMapModelToEditable(IContextQueryable<TModel> Query)
		{
			return Query.Select(EntityMapper.Map<TModel, TEditable>()).SingleOrDefaultAsync();
		}

		public virtual async Task<TEditable> LoadForEdit(TKey Id)
		{
			return await UseTransaction(async () =>
			{
				return await OnMapModelToEditable(DataSet.AsQueryable(false).Where(m => m.Id.Equals(Id)));
			});
		}
	}
    
   
}