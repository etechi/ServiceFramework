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

using System.Threading.Tasks;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Common;
using SF.Sys.Data;
using SF.Sys.Comments;
using SF.Sys.Auth.Permissions;
using SF.Sys.Linq;
using SF.Sys.Events;

namespace SF.Sys.Entities
{
	public class EntityModifyContext< TModel> : IEntityModifyContext<TModel>
			   where TModel : class
	{
		
		public TModel Model { get; set; }
		public ModifyAction Action { get; set; }
		public object OwnerId { get; set; }
		public object UserData { get; set; }
		public object ExtraArgument { get; set; }
	}

	public class EntityModifyContext<TEditable, TModel> :
		EntityModifyContext<TModel>,
		IEntityModifyContext<TEditable, TModel>
		where TModel : class
	{

		public TEditable Editable { get; set; }
	}
	public static class DataSetEntityStorage
	{
		public static Task<T> UseTransaction<T>(
			this IEntityServiceContext Storage,
			string TransMessage,
			Func<DbTransaction, Task<T>> Action
			)
		{
			return Storage.DataContext.UseTransaction(TransMessage, Action);
		}

		#region GetAsync

		public static void PermissionValidate(this IEntityServiceContext EntityManager,string Operation)
		{
			//EntityManager.AccessToken.Operator.PermissionValidate(EntityManager.EntityMetadata.Ident, Operation);
		}
		public static async Task<TReadOnlyEntity> GetAsync<TKey, TReadOnlyEntity, TModel>
		(
			this IEntityServiceContext Storage,
			TKey Id,
			Func<IContextQueryable<TModel>, Task<TReadOnlyEntity >> MapModelToReadOnly
		)
			where TModel : class
		{
			if (Id.IsDefault())
				return default(TReadOnlyEntity);

			Storage.PermissionValidate(Operations.Read);

			return await Storage.UseTransaction(
				$"载入实体{typeof(TModel).Comment().Title}:{Id}",
				async (trans) =>
				{
					var re = await MapModelToReadOnly(
						Storage.DataContext.Set<TModel>().AsQueryable(true).Where(Entity<TModel>.ObjectFilter(Id))
						);
					return re;
				});
		}
		public static async Task<TReadOnlyEntity> AutoGetAsync<TKey, TReadOnlyEntity, TModel>
		(
			this IEntityServiceContext Storage,
			TKey Id
		)
			where TModel : class
		{
			return await Storage.GetAsync<TKey,TReadOnlyEntity,TModel>(
				Id,
				async q =>
				{
					return await Storage.QueryResultBuildHelperCache
						.GetHelper<TModel, TReadOnlyEntity>(QueryMode.Detail)
						.QuerySingleOrDefault(q);
				}
				);
		}

		public static async Task<TReadOnlyEntity> GetAsync<TKey,TTempReadOnlyEntity, TReadOnlyEntity, TModel>
		(
			this IEntityServiceContext Storage,
			TKey Id,
			Func<IContextQueryable<TModel>, IContextQueryable<TTempReadOnlyEntity>> MapModelToReadOnly,
			Func<TTempReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
		)
			where TModel : class
		{
			return await Storage.GetAsync<TKey, TReadOnlyEntity, TModel>(
				Id,
				async q =>
				{
					var re = await MapModelToReadOnly(q).SingleOrDefaultAsync();
					if (re == null)
						return default(TReadOnlyEntity);
					var res = await PrepareReadOnly(new[] { re });
					return res[0];
				}
				);
		}

		public static Task<TReadOnlyEntity> GetAsync<TKey, TReadOnlyEntity, TModel>
		(
			this IEntityServiceContext Storage,
			TKey Id,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly,
			Func<TReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
		)
			where TModel : class
			=> GetAsync<TKey, TReadOnlyEntity, TReadOnlyEntity, TModel>(
				Storage,
				Id,
				MapModelToReadOnly,
				PrepareReadOnly
				);

		public static Task<TReadOnlyEntity> GetAsync<TKey, TReadOnlyEntity, TModel>
		(
			this IEntityServiceContext Storage,
			TKey Id,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly
		)
			where TModel : class
			=> GetAsync<TKey,TReadOnlyEntity, TReadOnlyEntity, TModel>(
				Storage,
				Id,
				MapModelToReadOnly,
				re => Task.FromResult(re)
				);
		#endregion

		#region Batch Get
		public static async Task<TReadOnlyEntity[]> BatchGetAsync<TKey, TReadOnlyEntity, TModel>
		(
			this IEntityServiceContext Storage,
			TKey[] Ids,
			Func<IContextQueryable<TModel>, Task<TReadOnlyEntity[]>> Query
		)
			where TModel : class
		{
			if (Ids == null || Ids.Length == 0)
				return Array.Empty<TReadOnlyEntity>();
			Storage.PermissionValidate(Operations.Read);
			return await Storage.UseTransaction(
				$"批量载入实体：{typeof(TModel).Comment().Title}",
				async (trans) =>
				{
					return await Query(
						Storage.DataContext.Set<TModel>().AsQueryable(true).Where(Entity<TModel>.MultipleObjectFilter(Ids))
						);
				});
		}
		public static async Task<TReadOnlyEntity[]> AutoBatchGetAsync<TKey, TReadOnlyEntity, TModel>
		(
			this IEntityServiceContext Storage,
			TKey[] Ids
		)
			where TModel : class
		{
			return await Storage.BatchGetAsync<TKey, TReadOnlyEntity, TModel>(
				Ids,
				async q => {
					return (await Storage.QueryResultBuildHelperCache.GetHelper<TModel, TReadOnlyEntity>(QueryMode.Detail).Query(
						q,
						Storage.PagingQueryBuilderCache.GetBuilder<TModel>(),
						Paging.All
						)).Items.ToArray();
				});
		}
		public static async Task<TReadOnlyEntity[]> BatchGetAsync<TKey, TTempReadOnlyEntity, TReadOnlyEntity, TModel>
		(
			this IEntityServiceContext Storage,
			TKey[] Ids,
			Func<IContextQueryable<TModel>, IContextQueryable<TTempReadOnlyEntity>> MapModelToReadOnly,
			Func<TTempReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
		)
			where TModel : class
		{
			return await Storage.BatchGetAsync<TKey, TReadOnlyEntity, TModel>(
				Ids,
				async q => { 
					var re = await MapModelToReadOnly(q).ToArrayAsync();
					if (re == null)
						return null;
					var res = await PrepareReadOnly(re);
					return res;
			});
		}

		public static Task<TReadOnlyEntity[]> BatchGetAsync<TKey,TReadOnlyEntity, TModel>
		(
			this IEntityServiceContext Storage,
			TKey[] Ids,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly,
			Func<TReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
		)
			where TModel : class
			=> BatchGetAsync<TKey, TReadOnlyEntity, TReadOnlyEntity, TModel>(
				Storage,
				Ids,
				MapModelToReadOnly,
				PrepareReadOnly
				);

		public static Task<TReadOnlyEntity[]> BatchGetAsync<TKey,TReadOnlyEntity, TModel>
		(
			this IEntityServiceContext Storage,
			TKey[] Ids,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly
		)
			where TModel : class
			where TReadOnlyEntity : class
			=> BatchGetAsync<TKey,TReadOnlyEntity, TReadOnlyEntity, TModel>(
				Storage,
				Ids,
				MapModelToReadOnly,
				re => Task.FromResult(re)
				);
		#endregion


		#region Query Idents

		public static async Task<QueryResult<TKey>> QueryIdentsAsync<TKey, TQueryArgument, TModel>(
			this IEntityServiceContext Storage,
			TQueryArgument Arg,
			Func<IContextQueryable<TModel>, TQueryArgument, IContextQueryable<TModel>> BuildQuery,
			IPagingQueryBuilder<TModel> PagingQueryBuilder
			)
			where TModel : class
			where TQueryArgument:IPagingArgument
		{
			Storage.PermissionValidate(Operations.Read);
			return await Storage.UseTransaction(
				$"查询实体主键：{typeof(TModel).Comment().Title}",
				async (trans) =>
				{
					var q = Storage.DataContext.Set<TModel>().AsQueryable(true);
					q = Entity<TModel>.QueryIdentFilter(q, Arg);
					q = BuildQuery(q, Arg);
					var re = await q.ToQueryResultAsync(
						qs => qs.Select(Entity<TModel>.KeySelector<TKey>()),
						PagingQueryBuilder,
						Arg.Paging
						);
					return re;
				});

		}
		public static async Task<QueryResult<TKey>> AutoQueryIdentsAsync<TKey, TQueryArgument, TModel>(
			   this IEntityServiceContext Storage,
			TQueryArgument Arg,
			Func<IContextQueryable<TModel>, TQueryArgument, IContextQueryable<TModel>> BuildQuery = null,
			IPagingQueryBuilder<TModel> PagingQueryBuilder = null
			)
			where TModel : class
			where TQueryArgument:IPagingArgument
		{
			Storage.PermissionValidate(Operations.Read);
			return await Storage.UseTransaction(
				$"查询实体主键：{typeof(TModel).Comment().Title}",
				async (trans) =>
				{
					var q = Storage.DataContext.Set<TModel>().AsQueryable(true);
					q = Storage.QueryFilterCache.GetFilter<TModel, TQueryArgument>().Filter(q, Storage, Arg);
					if (BuildQuery != null)
						q = BuildQuery(q, Arg);
					var re = await q.ToQueryResultAsync(
						qs => qs.Select(Entity<TModel>.KeySelector<TKey>()),
						PagingQueryBuilder??Storage.PagingQueryBuilderCache.GetBuilder<TModel>(),
						Arg.Paging
						);
					return re;
				});

		}

		#endregion

		#region Query
		public static async Task<QueryResult<TReadOnlyEntity>> QueryAsync<TReadOnlyEntity, TModel>(
			   this IEntityServiceContext Storage,
			Func<IContextQueryable<TModel>,Task< QueryResult<TReadOnlyEntity>>> Query
			)
			where TModel : class
		{
			Storage.PermissionValidate(Operations.Read);
			return await Storage.UseTransaction(
				$"查询实体{typeof(TModel).Comment().Title}",
				async (trans) =>
				{
					var q = Storage.DataContext.Set<TModel>().AsQueryable(true);
					return await Query(q);
				});
		}
		public static async Task<QueryResult<TReadOnlyEntity>> AutoQueryAsync< TReadOnlyEntity, TQueryArgument, TModel>(
			   this IEntityServiceContext Storage,
			TQueryArgument QueryArgument,
			Func<IContextQueryable<TModel>, TQueryArgument,  IContextQueryable<TModel>> BuildQuery=null,
			IPagingQueryBuilder<TModel> PagingQueryBuilder=null
			)
			where TModel : class
			where TQueryArgument:IPagingArgument
		{
			return await Storage.QueryAsync<TReadOnlyEntity, TModel>(async q => {
				q = Storage.QueryFilterCache.
					GetFilter<TModel, TQueryArgument>().Filter(q, Storage, QueryArgument);
				if (BuildQuery != null)
					q = BuildQuery(q, QueryArgument);
				return await Storage.QueryResultBuildHelperCache.GetHelper<TModel, TReadOnlyEntity>(QueryMode.Summary)
					.Query(
					q,
					PagingQueryBuilder??Storage.PagingQueryBuilderCache.GetBuilder<TModel>(),
					QueryArgument.Paging
					);
			});
		}
		public static async Task<QueryResult<TReadOnlyEntity>> QueryAsync<TTempReadOnlyEntity, TReadOnlyEntity, TQueryArgument, TModel>(
			   this IEntityServiceContext Storage,
			TQueryArgument Arg,
			Func<IContextQueryable<TModel>, TQueryArgument, IContextQueryable<TModel>> BuildQuery,
			IPagingQueryBuilder<TModel> PagingQueryBuilder,
			Func<IContextQueryable<TModel>, IContextQueryable<TTempReadOnlyEntity>> MapModelToReadOnly,
			Func<TTempReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
			)
			where TModel : class
			where TQueryArgument:IPagingArgument
		{
			return await Storage.QueryAsync<TReadOnlyEntity, TModel>(async q => { 
				q = Entity<TModel>.QueryIdentFilter(q,Arg);
				q = BuildQuery(q, Arg);
				var re = await q.ToQueryResultAsync(
					MapModelToReadOnly,
					PrepareReadOnly,
					PagingQueryBuilder,
					Arg.Paging
					);
				return re;
			});
		}


	
		public static Task<QueryResult<TReadOnlyEntity>> QueryAsync<TReadOnlyEntity, TQueryArgument, TModel>(
			   this IEntityServiceContext Storage,
			TQueryArgument Arg,
			Func<IContextQueryable<TModel>, TQueryArgument, IContextQueryable<TModel>> BuildQuery,
			IPagingQueryBuilder<TModel> PagingQueryBuilder,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly,
			Func<TReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
			)
			where TModel : class
			where TQueryArgument:IPagingArgument
			=> QueryAsync<TReadOnlyEntity,TReadOnlyEntity, TQueryArgument, TModel>(
				Storage, 
				Arg, 
				BuildQuery,
				PagingQueryBuilder,
				MapModelToReadOnly, 
				PrepareReadOnly
				);
	

		public static Task<QueryResult<TReadOnlyEntity>> QueryAsync<TReadOnlyEntity, TQueryArgument, TModel>(
			   this IEntityServiceContext Storage,
			TQueryArgument Arg,
			Func<IContextQueryable<TModel>, TQueryArgument, IContextQueryable<TModel>> BuildQuery,
			IPagingQueryBuilder<TModel> PagingQueryBuilder,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly
			)
			where TModel : class
			where TQueryArgument:IPagingArgument
			=> QueryAsync<TReadOnlyEntity, TReadOnlyEntity, TQueryArgument, TModel>(
				Storage,
				Arg,
				BuildQuery,
				PagingQueryBuilder,
				MapModelToReadOnly,
				re=>Task.FromResult(re)
				);

		#endregion

		#region LoadForEdit
		public static async Task<TEditable> LoadForEdit<TKey,TEditable, TModel>(
			this IEntityServiceContext Storage,
			TKey Key,
			Func<IContextQueryable<TModel>, Task<TEditable>> MapModelToEditable
			)
			where TModel:class
		{
			if (Key.IsDefault())
				return default(TEditable);
			Storage.PermissionValidate(Operations.Read);
			return await Storage.UseTransaction(
				$"载入编辑实体{typeof(TModel).Comment().Title}:{Entity<TKey>.GetIdents(Key)?.Join(",")}",
				async (trans) =>
				{
					return await MapModelToEditable(Storage.DataContext.Set<TModel>().AsQueryable(false).Where(Entity<TModel>.ObjectFilter(Key)));
				});
		}
		public static async Task<TEditable> AutoLoadForEdit<TKey, TEditable, TModel>(
			this IEntityServiceContext Storage,
			TKey Key
			)
			where TModel : class
		{
			if (Key.IsDefault())
				return default(TEditable);
			Storage.PermissionValidate(Operations.Read);
			return await Storage.UseTransaction(
				$"载入编辑实体{typeof(TModel).Comment().Title}:{Entity<TKey>.GetIdents(Key)?.Join(",")}",
				async (trans) =>
				{
					return await Storage.QueryResultBuildHelperCache.GetHelper<TModel,TEditable>(QueryMode.Edit).QuerySingleOrDefault(
						Storage.DataContext.Set<TModel>().AsQueryable(false).Where(Entity<TModel>.ObjectFilter(Key))
						);
				});
		}
		#endregion


		#region Events


		public static void PostChangedEvents<TEntity>(
			this IEntityServiceContext Manager,
			TEntity Entity,
			DataActionType DataActionType,
			IEntityMetadata meta=null
			)
		{
			if (meta == null)
			{
				meta = Manager.EntityMetadataCollection.FindByEntityType(typeof(TEntity));
				if (meta == null)
					return;
					//throw new ArgumentException($"找不到类型{typeof(TEntity)}相关的实体");
			}
			IEventEmitter ee = null;
			Manager.DataContext.TransactionScopeManager.AddCommitTracker(
				TransactionCommitNotifyType.BeforeCommit | 
				TransactionCommitNotifyType.AfterCommit | 
				TransactionCommitNotifyType.Rollback,
				async (t, e) =>
				{
					switch (t)
					{
						case TransactionCommitNotifyType.BeforeCommit:
							ee=await Manager.EventEmitService.Create(
								new EntityChanged<TEntity>(Entity)
								{
									ServiceId = Manager.ServiceInstanceDescroptor?.InstanceId,
									//Source=meta.Ident,
									Target= Entity<TEntity>.GetStrIdent(meta.Ident, Entity),
									Time=Manager.Now,
									Exception = e,
									Action= DataActionType
									//Type=DataActionType.ToString()
								});
							break;
						case TransactionCommitNotifyType.AfterCommit:
							await ee?.Commit();
							break;
						case TransactionCommitNotifyType.Rollback:
							await ee?.Cancel(e);
							break;
						default:
							throw new NotSupportedException();
					}
				}
			);
		}
		//public static void PostEntityRelationChangedEvents<TEntity, TRelatedEntity>(
		//	this IEntityServiceContext Manager,
		//	TEntity Entity,
		//	TRelatedEntity RelatedEntity,
		//	DataActionType DataActionType
		//	)
		//{
		//	Manager.AddPostAction(() =>
		//		Manager.EventEmitter.Emit(
		//			new EntityRelationChanged<TEntity,TRelatedEntity>()
		//			{
		//				Entity = Entity,
		//				Action = DataActionType,
		//				ServiceId = Manager.ServiceInstanceDescroptor?.InstanceId,
		//				Time = Manager.Now,
		//				PostActionType = PostActionType.BeforeCommit,
		//				RelatedEntity= RelatedEntity
		//			}
		//		),
		//		PostActionType.BeforeCommit
		//	);
		//	Manager.AddPostAction(() =>
		//		Manager.EventEmitter.Emit(
		//			new EntityRelationChanged<TEntity, TRelatedEntity>()
		//			{
		//				Entity = Entity,
		//				Action = DataActionType,
		//				ServiceId = Manager.ServiceInstanceDescroptor?.InstanceId,
		//				Time = Manager.Now,
		//				PostActionType = PostActionType.AfterCommit,
		//				RelatedEntity = RelatedEntity
		//			},
		//			false
		//		),
		//		PostActionType.AfterCommit
		//	);
		//}
		#endregion
		#region Create

		public static void InitCreate<TModel, TEntity>(
			this IEntityModifyContext<TEntity, TModel> ctx,
			TEntity entity,
			object ExtraArgument = null
			) where TModel:class,new()
		{
			ctx.Action = ModifyAction.Create;
			ctx.Model = new TModel();
			ctx.Editable = entity;
			ctx.ExtraArgument = ExtraArgument;
		}
		public static void InitUpdate<TModel, TEntity>(
			this IEntityModifyContext<TEntity, TModel> ctx,
			TModel model,
			TEntity entity,
			object ExtraArgument=null
			) where TModel : class
		{
			ctx.Action = ModifyAction.Update;
			ctx.Model = model;
			ctx.Editable = entity;
			ctx.ExtraArgument = ExtraArgument;
		}
		public static void InitRemove<TModel,TEntity>(
			this IEntityModifyContext<TEntity, TModel> ctx,
			TModel model,
			TEntity entity,
			object ExtraArgument=null
			) where TModel : class
		{
			ctx.Action = ModifyAction.Delete;
			ctx.Model = model;
			ctx.Editable = entity;
			ctx.ExtraArgument = ExtraArgument;
		}

		public static async Task<TKey> InternalCreateAsync<TKey,TEditable, TModel,TModifyContext>(
			this IEntityServiceContext Storage,
			TModifyContext Context,
			TEditable Entity,
			Func<TModifyContext, Task> UpdateModel,
			Func<TModifyContext, Task> InitModel,
			object ExtraArgument = null,
			bool EnableAutoModifier=false
			)
			where TModel : class, new()
			where TModifyContext: IEntityModifyContext<TEditable, TModel>
		{
			if (Entity.IsDefault())
				throw new ArgumentNullException("需要提供实体");
			Storage.PermissionValidate(Operations.Create);

			return await Storage.UseTransaction(
				$"新建实体{typeof(TModel).Comment().Title}",
				async (trans) =>
				{
					Context.InitCreate<TModel,TEditable>(Entity, ExtraArgument);

					if(InitModel!=null)
						await InitModel(Context);

					if(UpdateModel!=null)
						await UpdateModel(Context);

					Storage.DataContext.Set<TModel>().Add(Context.Model);
					Storage.PostChangedEvents<TEditable>(Context.Editable,DataActionType.Create);
					await Storage.DataContext.SaveChangesAsync();
					return Entity<TModel>.GetKey<TKey>(Context.Model);
				});
		}
		public static async Task<TKey> CreateAsync<TKey, TEditable,TModel>(
			this IEntityServiceContext Storage,
			TEditable Entity,
			Func<IEntityModifyContext<TEditable, TModel>, Task> UpdateModel,
			Func<IEntityModifyContext<TEditable, TModel>, Task> InitModel,
			object ExtraArgument=null,
			bool EnableAutoModifier = false
			) 
			where TModel:class,new()
		{
			var ctx =new EntityModifyContext<TEditable, TModel>();
			return await InternalCreateAsync<TKey, TEditable,TModel,IEntityModifyContext<TEditable, TModel>>(
				Storage, 
				ctx,
				Entity,
				UpdateModel, 
				InitModel,
				ExtraArgument,
				EnableAutoModifier
				);
		}

		#endregion

		#region Update
		public static async Task<bool> InternalUpdateAsync<TKey, TEditable, TModel, TModifyContext>(
			this IEntityServiceContext Storage,
			TModifyContext Context,
			TEditable Entity,
			Func<TModifyContext, Task> UpdateModel,
			Func<TKey, IContextQueryable<TModel>, Task<TModel>> LoadModelForEdit = null
			)
			where TModel : class
			where TModifyContext:IEntityModifyContext<TEditable, TModel>
		{
			if (Entity.IsDefault())
				throw new ArgumentNullException("需要提供实体");

			Storage.PermissionValidate(Operations.Update);

			return await Storage.UseTransaction(
				$"编辑实体{typeof(TModel).Comment().Title}:{Entity<TEditable>.GetIdentValues(Entity)}",
				async (trans) =>
				{
					var q = Storage.DataContext.Set<TModel>().AsQueryable(false).Where(Entity<TModel>.ObjectFilter(Entity));
					var model = LoadModelForEdit == null ? 
						await q.SingleOrDefaultAsync() : 
						await LoadModelForEdit(Entity<TEditable>.GetKey<TKey>(Entity),q);
					if (model == null)
						return false;

					InitUpdate(Context, model, Entity, null);

					if(UpdateModel != null)
						await UpdateModel(Context);

					Storage.DataContext.Set<TModel>().Update(Context.Model);

					Storage.PostChangedEvents<TEditable>(Context.Editable, DataActionType.Update);


					await Storage.DataContext.SaveChangesAsync();
					return true;
				});
		}
		public static async Task<bool> UpdateAsync<TKey, TEditable,TModel>(
			this IEntityServiceContext Storage,
			TEditable Entity,
			Func<IEntityModifyContext<TEditable, TModel>, Task> UpdateModel=null,
			Func<TKey,IContextQueryable<TModel>, Task<TModel>> LoadModelForEdit=null
			)
			where TModel : class
		{
			var ctx = new EntityModifyContext<TEditable, TModel>();
			return await InternalUpdateAsync(Storage, ctx, Entity, UpdateModel, LoadModelForEdit);
		}
		
		#endregion

		public static async Task<bool> InternalRemoveAsync<TKey,TEditable, TModel,TModifyContext>(
			this IEntityServiceContext Storage,
			TModifyContext Context,
			TKey Id,
			Func<TModifyContext, Task> RemoveModel = null,
			Func<TKey, IContextQueryable<TModel>, Task<TModel>> LoadModelForEdit = null
			)
			where TModel : class
			where TModifyContext: IEntityModifyContext<TEditable,TModel>
		{
			if (Id.IsDefault())
				throw new ArgumentNullException("需要指定主键");
			Storage.PermissionValidate(Operations.Remove);

			return await Storage.UseTransaction(
				$"删除实体{typeof(TModel).Comment().Title}:{Id}",
				async (trans) =>
				{
					var q = Storage.DataContext.Set<TModel>().AsQueryable(false).Where(Entity<TModel>.ObjectFilter(Id));
					var model = LoadModelForEdit == null ? await q.SingleOrDefaultAsync() : await LoadModelForEdit(Id,q);
					if (model == null)
						return false;
					var editable = Entity<TKey>.GetKey<TEditable>(Id);
					InitRemove(Context, model, editable, null);

					if (RemoveModel != null)
						await RemoveModel(Context);
					Storage.DataContext.Set<TModel>().Remove(Context.Model);

					Storage.PostChangedEvents<TEditable>(Context.Editable, DataActionType.Delete);

					await Storage.DataContext.SaveChangesAsync();
					return true;
				});
		}
		public static async Task<bool> RemoveAsync<TKey, TEditable, TModel>(
			this IEntityServiceContext Storage,
			TKey Id,
			Func<IEntityModifyContext<TEditable, TModel>, Task> RemoveModel=null,
			Func<TKey, IContextQueryable<TModel>, Task<TModel>> LoadModelForEdit=null
			)
			where TModel : class
		{
			var ctx =(IEntityModifyContext < TEditable, TModel >) new EntityModifyContext<TEditable, TModel>();
			return await InternalRemoveAsync<TKey,TEditable, TModel, IEntityModifyContext<TEditable, TModel>>(
				Storage, 
				ctx, 
				Id, 
				RemoveModel, 
				LoadModelForEdit
				);
		}
		public static async Task RemoveAllAsync<TKey,TEditable,TModel>(
			this IEntityServiceContext Storage,
			Func<TKey, Task> Remove,
			Expression<Func<TModel,bool>> Condition=null,
			int BatchCount=100
			)
			where TModel : class
		{
			var tsm = Storage.DataContext.TransactionScopeManager;
			var paging = new Paging { Count = BatchCount };
			for (; ; )
			{
				var batchIndex = 0;
				var left = await tsm.UseTransaction(
					$"批量删除{typeof(TModel).Comment()}", async s =>
					{
						var ids = await Storage.UseTransaction($"查询批量删除主键{batchIndex++}", async (trans) =>
						 {
							 var q = Storage.DataContext.Set<TModel>().AsQueryable();
							 if (Condition != null)
								 q = q.Where(Condition);
							 return await q.Select(Entity<TModel>.KeySelector<TKey>()).Take(BatchCount).ToArrayAsync();
						 });
						foreach (var id in ids)
							await Remove(id);
						return ids.Length;
					}, 
					TransactionScopeMode.RequireTransaction
				);
				if (left < BatchCount)
					break;
			}
		}

		public static Task RemoveAllScoppedAsync<TKey,TEditable, TModel>(
			this IEntityServiceContext Storage,
			long ScopeId,
			Func<TKey, Task> Remove,
			int BatchCount = 100
			)
			where TModel : class, IEntityWithScope
			=> Storage.RemoveAllAsync<TKey,TEditable,TModel>(Remove, m => m.ServiceDataScopeId == ScopeId, BatchCount);
 
	}
	public interface IReadOnlyEntityHelper<TKey,TReadOnlyTemp, TReadOnly, TQueryArgument, TModel>
		where TModel : class
	{
		Task<TReadOnly> GetAsync(
			TKey Key,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyTemp>> MapModelToReadOnly,
			Func<TReadOnlyTemp[], Task<TReadOnly[]>> PrepareReadOnly
		);

		Task<TReadOnly[]> GetAsync(
			TKey[] Key,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyTemp>> MapModelToReadOnly,
			Func<TReadOnlyTemp[], Task<TReadOnly[]>> PrepareReadOnly
			);

		Task<QueryResult<TReadOnly>> QueryIdentsAsync(
			TQueryArgument Arg,
			Paging paging,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyTemp>> MapModelToReadOnly,
			Func<TReadOnlyTemp[], Task<TReadOnly[]>> PrepareReadOnly
			);
		Task<QueryResult<TReadOnly>> QueryAsync(
			TQueryArgument Arg,
			Paging paging,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyTemp>> MapModelToReadOnly,
			Func<TReadOnlyTemp[], Task<TReadOnly[]>> PrepareReadOnly
			);
	}

	public interface IEntityHelper<TKey, TReadOnlyTemp, TReadOnly, TQueryArgument, TEditable, TModel>:
		IReadOnlyEntityHelper<TKey,TReadOnlyTemp, TReadOnly, TQueryArgument, TModel>
		where TModel:class
	{
		Task<TEditable> LoadForEdit(
			TKey Id, 
			Func<IContextQueryable<TModel>, Task<TEditable>> MapModelToEditable
			);

		Task<TKey> CreateAsync(
			TEditable Entity,
			Func<IEntityModifyContext< TEditable, TModel>, Task> UpdateModel,
			Func<IEntityModifyContext<TEditable,TModel>, Task> InitModel
			);
		Task<bool> UpdateAsync(
			TEditable Entity,
			Func<IEntityModifyContext<TEditable, TModel>, Task<TModel>> LoadModelForEdit,
			Func<IEntityModifyContext<TEditable, TModel>, Task> UpdateModel
			);

		Task<bool> RemoveAsync(
			TKey Entity,
			Func<IEntityModifyContext<TEditable, TModel>, Task<TModel>> LoadModelForEdit,
			Func<IEntityModifyContext<TEditable, TModel>, Task> RemoveModel
			);

		Task RemoveAllAsync();
		
	}
}
