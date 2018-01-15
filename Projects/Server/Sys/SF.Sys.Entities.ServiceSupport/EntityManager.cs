#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SF.Sys.Comments;

namespace SF.Sys.Entities
{

	public abstract class ModidifiableEntityManager<TKey, TPublic, TEditable, TModel> :
		ModidifiableEntityManager<ObjectKey<TKey>, TPublic, TPublic, QueryArgument<ObjectKey<TKey>>, TEditable, TModel>
		where TPublic : class
		where TModel : class, new()
		where TEditable : class
		where TKey:IEquatable<TKey>
	{
		public ModidifiableEntityManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}
		protected override async Task<TPublic[]> OnPrepareDetails(TPublic[] Internals)
		{
			await ServiceContext.EntityPropertyFiller.Fill(ServiceInstanceDescriptor.InstanceId, Internals);
			return Internals;
		}
		protected override PagingQueryBuilder<TModel> PagingQueryBuilder => new PagingQueryBuilder<TModel>(
			"id",
			b => b.Add("id",Entity<TModel>.SingleKeySelector<TKey>())
			);
		protected override IContextQueryable<TModel> OnBuildQuery(IContextQueryable<TModel> Query, QueryArgument<ObjectKey<TKey>> Arg)
		{
			return Query;
		}
	}

	public abstract class ModidifiableEntityManager<TKey, TPublic, TQueryArgument, TEditable, TModel> :
		ModidifiableEntityManager<TKey, TPublic, TPublic, TQueryArgument, TEditable, TModel>
		where TPublic : class
		where TModel : class, new()
		where TQueryArgument : class,IPagingArgument, new()
		where TEditable : class
	{
		public ModidifiableEntityManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}
		protected override async Task<TPublic[]> OnPrepareDetails(TPublic[] Internals)
		{
			await ServiceContext.EntityPropertyFiller.Fill(ServiceInstanceDescriptor?.InstanceId, Internals);
			return Internals;
		}
	}
	public abstract class ModidifiableEntityManager<TKey,TPublic, TTemp, TQueryArgument, TEditable,TModel> :
		QuerableEntitySource<TKey, TPublic, TTemp, TQueryArgument, TModel>,
		IEntityManager<TKey, TEditable>
		where TPublic : class
		where TModel : class,new()
		where TQueryArgument : class,IPagingArgument, new()
		where TEditable : class
	{
		public interface IModifyContext : IEntityModifyContext<TEditable, TModel>
		{

		}
		class ModifyContext : EntityModifyContext<TEditable, TModel>, IModifyContext
		{

		}
        public ModidifiableEntityManager(IEntityServiceContext EntityServiceContext) :base(EntityServiceContext)
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
			return ServiceContext.InternalCreateAsync<TKey,TEditable, TModel,IModifyContext>(
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
			return ServiceContext.InternalRemoveAsync<TKey,TEditable, TModel, IModifyContext>(
				Context, 
				Id, 
				OnRemoveModel,
				OnLoadModelForUpdate
				);
		}

		protected virtual Task OnRemoveModel(IModifyContext ctx)
		{
			ServiceContext.DataContext.Set<TModel>().Remove(ctx.Model);
			return Task.CompletedTask;
		}

		public virtual async Task RemoveAllAsync()
		{
			await ServiceContext.RemoveAllAsync<TKey,TEditable,TModel>(
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
				throw new ArgumentException($"找不到对象:{GetType().Comment()}:{Entity<TEditable>.GetIdentValues(obj)}");
		}
		protected virtual async Task<bool> InternalUpdateAsync(IModifyContext Context,TEditable obj)
		{
			return await ServiceContext.InternalUpdateAsync<TKey,TEditable,TModel,IModifyContext>(
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
			return Query.Select(Poco.MapExpression<TModel, TEditable>()).SingleOrDefaultAsync();
		}

		public virtual Task<TEditable> LoadForEdit(TKey Key)
		{
			return ServiceContext.LoadForEdit<TKey,TEditable,TModel>(Key, OnMapModelToEditable);
		}
	}

	public abstract class AutoModifiableEntityManager<TKey, TDetail, TSummary,  TQueryArgument, TEditable, TModel> :
		 AutoQuerableEntitySource<TKey, TDetail, TSummary, TQueryArgument, TModel>,
		 IEntityManager<TKey, TEditable>
		 where TDetail : class
		 where TModel : class, new()
		 where TQueryArgument : class,IPagingArgument, new()
		 where TEditable : class
		 where TSummary:class
	{
		public interface IModifyContext : IEntityModifyContext<TEditable, TModel>
		{

		}
		class ModifyContext : EntityModifyContext<TEditable, TModel>, IModifyContext
		{

		}
		public AutoModifiableEntityManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}

		public virtual EntityManagerCapability Capabilities => EntityManagerCapability.All;
		public bool AutoSaveChanges { get; set; } = true;

		IEntityModifySyncQueue<TEditable> _SyncQueue;

		protected void SetSyncQueue<TSyncKey>(
			SF.Sys.Threading.ISyncQueue<TSyncKey> Queue,
			Func<TEditable, TSyncKey> GetSyncKey
			)
			=> _SyncQueue = new EntityModifySyncQueue<TEditable, TSyncKey>(Queue, GetSyncKey);

		#region create

		protected virtual async Task OnNewModel(IModifyContext ctx)
		{
			await ServiceContext
				.EntityModifierCache
				.GetEntityModifier<TEditable, TModel>(DataActionType.Create)
				.Execute(ServiceContext, ctx);
		}
		protected IModifyContext NewModifyContext()
			=> new ModifyContext();
		
		public virtual async Task<TKey> CreateAsync(TEditable obj)
		{
			var ctx = NewModifyContext();
			if (_SyncQueue == null)
				await InternalCreateAsync(ctx, obj, null);
			else
				await _SyncQueue.Queue(obj, () => InternalCreateAsync(ctx, obj, null));
			return Entity<TModel>.GetKey<TKey>(ctx.Model);
		}
		protected virtual Task<TKey> InternalCreateAsync(IModifyContext Context, TEditable obj, object ExtraArgument)
		{
			return ServiceContext.InternalCreateAsync<TKey, TEditable, TModel, IModifyContext>(
				Context,
				obj,
				OnUpdateModel,
				OnNewModel,
				ExtraArgument,
				true
				);
		}
		#endregion


		#region delete

		public virtual async Task RemoveAsync(TKey Id)
		{
			var ctx = NewModifyContext();
			var re = _SyncQueue == null ?
				await InternalRemoveAsync(ctx, Id) :
				await _SyncQueue.Queue(
					await LoadForEdit(Id), 
					() => InternalRemoveAsync(ctx, Id)
					);
			if (!re)
				throw new ArgumentException($"找不到对象:{GetType().Comment()}:{Id}");
		}
		protected virtual Task<bool> InternalRemoveAsync(IModifyContext Context, TKey Id)
		{
			return ServiceContext.InternalRemoveAsync<TKey, TEditable, TModel, IModifyContext>(
				Context,
				Id,
				OnRemoveModel,
				OnLoadModelForUpdate
				);
		}

		protected virtual async Task OnRemoveModel(IModifyContext ctx)
		{
			await ServiceContext
				.EntityModifierCache
				.GetEntityModifier<TEditable, TModel>(DataActionType.Delete)
				.Execute(ServiceContext, ctx);

		}

		public virtual async Task RemoveAllAsync()
		{
			await ServiceContext.RemoveAllAsync<TKey, TEditable, TModel>(
				RemoveAsync
				);
		}

		#endregion


		#region Update
		protected virtual async Task OnUpdateModel(IModifyContext ctx)
		{
			await ServiceContext
				.EntityModifierCache
				.GetEntityModifier<TEditable, TModel>(DataActionType.Update)
				.Execute(ServiceContext, ctx);

		}

		public virtual async Task UpdateAsync(TEditable obj)
		{
			var ctx = NewModifyContext();
			var re = _SyncQueue == null ?
					await InternalUpdateAsync(ctx, obj) :
					await _SyncQueue.Queue(obj, () => 
						InternalUpdateAsync(ctx, obj)
						);
			if (!re)
				throw new ArgumentException($"找不到对象:{GetType().Comment()}:{Entity<TEditable>.GetIdentValues(obj)}");
		}
		protected virtual async Task<bool> InternalUpdateAsync(IModifyContext Context, TEditable obj)
		{
			return await ServiceContext.InternalUpdateAsync<TKey, TEditable, TModel, IModifyContext>(
				Context,
				obj,
				OnUpdateModel,
				OnLoadModelForUpdate
				);
		}

		#endregion

		#region CreateOrUpdate
		public virtual Task<(TKey Id, bool CreateNewObject)> CreateOrUpdateAsync(
			TEditable Editable,
			Expression<Func<TModel, bool>> Selector,
			Action<IModifyContext> Updator,
			Func<IModifyContext, Task> Initializer = null,
			object ExtraArgument = null
			)
			=> CreateOrUpdateAsync<int>(
			Editable,
			Selector,
			m =>
			{
				Updator(m);
				return Task.FromResult(0);
			},
			Initializer,
			ExtraArgument
			).ContinueWith(r=>(r.Result.Id,r.Result.CreateNewObject));

		public virtual Task<(T Result, TKey Id, bool CreateNewObject)> CreateOrUpdateAsync<T>(
			TEditable Editable,
			Expression<Func<TModel, bool>> Selector,
			Func<IModifyContext, T> Updator,
			Func<IModifyContext, Task> Initializer = null,
			object ExtraArgument = null
			)
			=> CreateOrUpdateAsync<T>(
			Editable,
			Selector,
			m => Task.FromResult(Updator(m)),
			Initializer,
			ExtraArgument
			);
		public virtual async Task<(T Result,TKey Id,bool CreateNewObject)> CreateOrUpdateAsync<T>(
			TEditable Editable,
			Expression<Func<TModel,bool>> Selector, 
			Func<IModifyContext, Task<T>> Updator, 
			Func<IModifyContext, Task> Initializer=null,
			object ExtraArgument=null
			)
		{
			var ctx = NewModifyContext();
			var re = _SyncQueue == null ?
					await InternalCreateOrUpdateAsync(ctx, Editable, Selector,Updator) :
					await _SyncQueue.Queue(Editable, () =>
						InternalCreateOrUpdateAsync(ctx, Editable, Selector, Updator, Initializer)
						);
			return re;
		}
		protected virtual async Task<(T Result, TKey Id, bool NewObjectCreated)> InternalCreateOrUpdateAsync<T>(
			IModifyContext Context, 
			TEditable Editable, 
			Expression<Func<TModel, bool>> Selector, 
			Func<IModifyContext,Task<T>> Updator, 
			Func<IModifyContext, Task> Initializer = null, 
			object ExtraArgument = null
			)
		{
			T result=default;
			var NewObjectCreated = false;
			var re=await ServiceContext.InternalCreateOrUpdateAsync<TKey, TEditable, TModel, IModifyContext>(
				Context,
				Editable,
				Selector,
				async ctx=>
				{
					result = await Updator(ctx);
					await OnUpdateModel(ctx);
				},
				async ctx=>
				{
					NewObjectCreated = true;
					if (Initializer != null)
						await Initializer(ctx);
					await OnNewModel(ctx);
				},
				ExtraArgument,
				true
				);
			return (result, re, NewObjectCreated);
		}

		#endregion

		protected virtual IContextQueryable<TModel> OnLoadChildObjectsForUpdate(TKey Id, IContextQueryable<TModel> query)
		{
			return query;
		}
		protected virtual Task<TModel> OnLoadModelForUpdate(TKey Id, IContextQueryable<TModel> ctx)
		{
			return OnLoadChildObjectsForUpdate(Id, ctx).SingleOrDefaultAsync();
		}

		
		public virtual Task<TEditable> LoadForEdit(TKey Key)
		{
			return ServiceContext.AutoLoadForEdit<TKey,TEditable,TModel>(Key);
		}
	}

}