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
using System.Reflection;

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
		public IDataContext DataContext { get; set; }
	}

	public class EntityModifyContext<TEditable, TModel> :
		EntityModifyContext<TModel>,
		IEntityModifyContext<TEditable, TModel>
		where TModel : class
	{

		public TEditable Editable { get; set; }
	}

	public interface IEntityModifySyncQueue<TEditable>
	{
		Task<T> Queue<T>(TEditable Editable, Func<Task<T>> Callback);
	}

	public class EntityModifySyncQueue<TEditable,TSyncKey> :
		 IEntityModifySyncQueue<TEditable>
	{
		public SF.Sys.Threading.ISyncQueue<TSyncKey> SyncQueue { get; }
		public Func<TEditable, TSyncKey> GetSyncKey { get; }
		public EntityModifySyncQueue(
			SF.Sys.Threading.ISyncQueue<TSyncKey> SyncQueue,
			Func<TEditable, TSyncKey> GetSyncKey
			)
		{
			this.SyncQueue = SyncQueue;
			this.GetSyncKey = GetSyncKey;
		}

		public Task<T> Queue<T>(TEditable Editable, Func<Task<T>> Callback)
		{
			return SyncQueue.Queue(GetSyncKey(Editable), Callback);
		}
	}
	public static class DataSetEntityStorage
	{
		//public static Task<T> UseTransaction<T>(
		//	this IEntityServiceContext ServiceContext,
		//	string TransMessage,
		//	Func<DbTransaction, Task<T>> Action
		//	)
		//{
		//	return ServiceContext.DataContext.UseTransaction(TransMessage, Action);
		//}

		#region GetAsync

		public static void PermissionValidate(this IEntityServiceContext EntityManager,string Operation)
		{
			//EntityManager.AccessToken.Operator.PermissionValidate(EntityManager.EntityMetadata.Ident, Operation);
		}
		public static async Task<TReadOnlyEntity> GetAsync<TKey, TReadOnlyEntity, TModel>
		(
			this IEntityServiceContext ServiceContext,
			TKey Id,
			Func<IContextQueryable<TModel>, Task<TReadOnlyEntity >> MapModelToReadOnly
		)
			where TModel : class
		{
			if (Id.IsDefault())
				return default(TReadOnlyEntity);

			ServiceContext.PermissionValidate(Operations.Read);

			return await ServiceContext.DataScope.Use(
				$"载入实体{typeof(TModel).Comment().Title}:{Id}",
				ctx =>
					MapModelToReadOnly(
						ctx.Queryable<TModel>()
							.Where(Entity<TModel>.ObjectFilter(Id))
						)
				);
		}
		public static async Task<TReadOnlyEntity> AutoGetAsync<TKey, TReadOnlyEntity, TModel>
		(
			this IEntityServiceContext ServiceContext,
			TKey Id,
			int QueryDeep,
			string[] Fields = null
		)
			where TModel : class
		{
			return await ServiceContext.GetAsync<TKey,TReadOnlyEntity,TModel>(
				Id,
				async q =>
				{
					return await ServiceContext.QueryResultBuildHelperCache
						.GetHelper<TModel, TReadOnlyEntity>(QueryMode.Detail)
						.QuerySingleOrDefault(q,QueryDeep, PropertySelector.Get(Fields));
				}
				);
		}

		public static async Task<TReadOnlyEntity> GetAsync<TKey,TTempReadOnlyEntity, TReadOnlyEntity, TModel>
		(
			this IEntityServiceContext ServiceContext,
			TKey Id,
			Func<IContextQueryable<TModel>, IContextQueryable<TTempReadOnlyEntity>> MapModelToReadOnly,
			Func<TTempReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
		)
			where TModel : class
		{
			return await ServiceContext.GetAsync<TKey, TReadOnlyEntity, TModel>(
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
			this IEntityServiceContext ServiceContext,
			TKey Id,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly,
			Func<TReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
		)
			where TModel : class
			=> GetAsync<TKey, TReadOnlyEntity, TReadOnlyEntity, TModel>(
				ServiceContext,
				Id,
				MapModelToReadOnly,
				PrepareReadOnly
				);

		public static Task<TReadOnlyEntity> GetAsync<TKey, TReadOnlyEntity, TModel>
		(
			this IEntityServiceContext ServiceContext,
			TKey Id,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly
		)
			where TModel : class
			=> GetAsync<TKey,TReadOnlyEntity, TReadOnlyEntity, TModel>(
				ServiceContext,
				Id,
				MapModelToReadOnly,
				re => Task.FromResult(re)
				);
		#endregion

		#region Batch Get
		public static async Task<TReadOnlyEntity[]> BatchGetAsync<TKey, TReadOnlyEntity, TModel>
		(
			this IEntityServiceContext ServiceContext,
			TKey[] Ids,
			Func<IContextQueryable<TModel>, Task<TReadOnlyEntity[]>> Query
		)
			where TModel : class
		{
			if (Ids == null || Ids.Length == 0)
				return Array.Empty<TReadOnlyEntity>();
			ServiceContext.PermissionValidate(Operations.Read);
			return await ServiceContext.DataScope.Use(
				$"批量载入实体：{typeof(TModel).Comment().Title}",
				(ctx) =>
					Query(ctx.Queryable<TModel>().Where(Entity<TModel>.MultipleObjectFilter(Ids)))
				);
		}
		public static async Task<TReadOnlyEntity[]> AutoBatchGetAsync<TKey, TReadOnlyEntity, TModel>
		(
			this IEntityServiceContext ServiceContext,
			TKey[] Ids,
			string[] Properties,
			int DetailModelDeep
		)
			where TModel : class
		{
			return await ServiceContext.BatchGetAsync<TKey, TReadOnlyEntity, TModel>(
				Ids,
				async q => {
					return (await ServiceContext.QueryResultBuildHelperCache.GetHelper<TModel, TReadOnlyEntity>(QueryMode.Detail).Query(
						q,
						ServiceContext.PagingQueryBuilderCache.GetBuilder<TModel>(),
						new Paging
						{
							Count = Paging.All.Count,
							Properties = Properties,
							ResultDeep = DetailModelDeep
						}
						)).Items.ToArray();
				});
		}
		public static async Task<TReadOnlyEntity[]> BatchGetAsync<TKey, TTempReadOnlyEntity, TReadOnlyEntity, TModel>
		(
			this IEntityServiceContext ServiceContext,
			TKey[] Ids,
			Func<IContextQueryable<TModel>, IContextQueryable<TTempReadOnlyEntity>> MapModelToReadOnly,
			Func<TTempReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
		)
			where TModel : class
		{
			return await ServiceContext.BatchGetAsync<TKey, TReadOnlyEntity, TModel>(
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
			this IEntityServiceContext ServiceContext,
			TKey[] Ids,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly,
			Func<TReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
		)
			where TModel : class
			=> BatchGetAsync<TKey, TReadOnlyEntity, TReadOnlyEntity, TModel>(
				ServiceContext,
				Ids,
				MapModelToReadOnly,
				PrepareReadOnly
				);

		public static Task<TReadOnlyEntity[]> BatchGetAsync<TKey,TReadOnlyEntity, TModel>
		(
			this IEntityServiceContext ServiceContext,
			TKey[] Ids,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly
		)
			where TModel : class
			where TReadOnlyEntity : class
			=> BatchGetAsync<TKey,TReadOnlyEntity, TReadOnlyEntity, TModel>(
				ServiceContext,
				Ids,
				MapModelToReadOnly,
				re => Task.FromResult(re)
				);
		#endregion


		#region Query Idents

		public static async Task<QueryResult<TKey>> QueryIdentsAsync<TKey, TQueryArgument, TModel>(
			this IEntityServiceContext ServiceContext,
			TQueryArgument Arg,
			Func<IContextQueryable<TModel>, TQueryArgument, IContextQueryable<TModel>> BuildQuery,
			IPagingQueryBuilder<TModel> PagingQueryBuilder
			)
			where TModel : class
			where TQueryArgument:IPagingArgument
		{
			ServiceContext.PermissionValidate(Operations.Read);
			return await ServiceContext.DataScope.Use(
				$"查询实体主键：{typeof(TModel).Comment().Title}",
				async (ctx) =>
				{
					var q = ctx.Queryable<TModel>();
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
			   this IEntityServiceContext ServiceContext,
			TQueryArgument Arg,
			Func<IContextQueryable<TModel>, TQueryArgument, IContextQueryable<TModel>> BuildQuery = null,
			IPagingQueryBuilder<TModel> PagingQueryBuilder = null
			)
			where TModel : class
			where TQueryArgument:IPagingArgument
		{
			ServiceContext.PermissionValidate(Operations.Read);
			return await ServiceContext.DataScope.Use(
				$"查询实体主键：{typeof(TModel).Comment().Title}",
				async (ctx) =>
				{
					var q = ctx.Queryable<TModel>();
					q = ServiceContext.QueryFilterCache.GetFilter<TModel, TQueryArgument>().Filter(q, ServiceContext, Arg);
					if (BuildQuery != null)
						q = BuildQuery(q, Arg);
					var re = await q.ToQueryResultAsync(
						qs => qs.Select(Entity<TModel>.KeySelector<TKey>()),
						PagingQueryBuilder??ServiceContext.PagingQueryBuilderCache.GetBuilder<TModel>(),
						Arg.Paging
						);
					return re;
				});

		}

		#endregion

		#region Query
		public static async Task<QueryResult<TReadOnlyEntity>> QueryAsync<TReadOnlyEntity, TModel>(
			   this IEntityServiceContext ServiceContext,
			Func<IContextQueryable<TModel>,Task< QueryResult<TReadOnlyEntity>>> Query
			)
			where TModel : class
		{
			ServiceContext.PermissionValidate(Operations.Read);
			return await ServiceContext.DataScope.Use(
				$"查询实体{typeof(TModel).Comment().Title}",
				(ctx) =>
					Query(ctx.Queryable<TModel>())
				);
		}
		public static async Task<QueryResult<TReadOnlyEntity>> AutoQueryAsync< TReadOnlyEntity, TQueryArgument, TModel>(
			   this IEntityServiceContext ServiceContext,
			TQueryArgument QueryArgument,
			Func<IContextQueryable<TModel>, TQueryArgument,  IContextQueryable<TModel>> BuildQuery=null,
			IPagingQueryBuilder<TModel> PagingQueryBuilder=null
			)
			where TModel : class
			where TQueryArgument:IPagingArgument
		{
			return await ServiceContext.QueryAsync<TReadOnlyEntity, TModel>(async q => {
				q = ServiceContext.QueryFilterCache.
					GetFilter<TModel, TQueryArgument>().Filter(q, ServiceContext, QueryArgument);
				if (BuildQuery != null)
					q = BuildQuery(q, QueryArgument);
				return await ServiceContext.QueryResultBuildHelperCache.GetHelper<TModel, TReadOnlyEntity>(QueryMode.Summary)
					.Query(
					q,
					PagingQueryBuilder??ServiceContext.PagingQueryBuilderCache.GetBuilder<TModel>(),
					QueryArgument.Paging
					);
			});
		}
		public static async Task<QueryResult<TReadOnlyEntity>> QueryAsync<TTempReadOnlyEntity, TReadOnlyEntity, TQueryArgument, TModel>(
			   this IEntityServiceContext ServiceContext,
			TQueryArgument Arg,
			Func<IContextQueryable<TModel>, TQueryArgument, IContextQueryable<TModel>> BuildQuery,
			IPagingQueryBuilder<TModel> PagingQueryBuilder,
			Func<IContextQueryable<TModel>, IContextQueryable<TTempReadOnlyEntity>> MapModelToReadOnly,
			Func<TTempReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
			)
			where TModel : class
			where TQueryArgument:IPagingArgument
		{
			return await ServiceContext.QueryAsync<TReadOnlyEntity, TModel>(async q => { 
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
			   this IEntityServiceContext ServiceContext,
			TQueryArgument Arg,
			Func<IContextQueryable<TModel>, TQueryArgument, IContextQueryable<TModel>> BuildQuery,
			IPagingQueryBuilder<TModel> PagingQueryBuilder,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly,
			Func<TReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
			)
			where TModel : class
			where TQueryArgument:IPagingArgument
			=> QueryAsync<TReadOnlyEntity,TReadOnlyEntity, TQueryArgument, TModel>(
				ServiceContext, 
				Arg, 
				BuildQuery,
				PagingQueryBuilder,
				MapModelToReadOnly, 
				PrepareReadOnly
				);
	

		public static Task<QueryResult<TReadOnlyEntity>> QueryAsync<TReadOnlyEntity, TQueryArgument, TModel>(
			   this IEntityServiceContext ServiceContext,
			TQueryArgument Arg,
			Func<IContextQueryable<TModel>, TQueryArgument, IContextQueryable<TModel>> BuildQuery,
			IPagingQueryBuilder<TModel> PagingQueryBuilder,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly
			)
			where TModel : class
			where TQueryArgument:IPagingArgument
			=> QueryAsync<TReadOnlyEntity, TReadOnlyEntity, TQueryArgument, TModel>(
				ServiceContext,
				Arg,
				BuildQuery,
				PagingQueryBuilder,
				MapModelToReadOnly,
				re=>Task.FromResult(re)
				);

		#endregion

		#region LoadForEdit
		public static async Task<TEditable> LoadForEdit<TKey,TEditable, TModel>(
			this IEntityServiceContext ServiceContext,
			TKey Key,
			Func<IDataContext,IContextQueryable<TModel>, Task<TEditable>> MapModelToEditable
			)
			where TModel:class
		{
			if (Key.IsDefault())
				return default(TEditable);
			ServiceContext.PermissionValidate(Operations.Read);
			return await ServiceContext.DataScope.Use(
				$"载入编辑实体{typeof(TModel).Comment().Title}:{Entity<TKey>.GetIdents(Key)?.Join(",")}",
				ctx =>
					MapModelToEditable(
						ctx,
						ctx.Queryable<TModel>().
						Where(Entity<TModel>.ObjectFilter(Key))
					)
				);
		}
		public static async Task<TEditable> AutoLoadForEdit<TKey, TEditable, TModel>(
			this IEntityServiceContext ServiceContext,
			TKey Key,
			int QueryDeep
			)
			where TModel : class
		{
			if (Key.IsDefault())
				return default(TEditable);
			ServiceContext.PermissionValidate(Operations.Read);
			return await ServiceContext.DataScope.Use(
				$"载入编辑实体{typeof(TModel).Comment().Title}:{Entity<TKey>.GetIdents(Key)?.Join(",")}",
				ctx =>
					ServiceContext.QueryResultBuildHelperCache
						.GetHelper<TModel,TEditable>(QueryMode.Edit)
						.QuerySingleOrDefault(
							ctx.Queryable<TModel>().Where(Entity<TModel>.ObjectFilter(Key)),
							QueryDeep,
							PropertySelector.All
						)
				);
		}
		#endregion


		#region Events


		public static void PostChangedEvents<TEntity>(
			this IEntityServiceContext Manager,
			IDataContext DataContext,
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
			DataContext.AddCommitTracker(
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
							if(ee!=null)
								await ee.Commit();
							break;
						case TransactionCommitNotifyType.Rollback:
							if(ee!=null)
								await ee.Cancel(e);
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
			IDataContext dataContext,
			TEntity entity,
			object ExtraArgument = null
			) where TModel:class,new()
		{
			ctx.DataContext = dataContext;
			ctx.Action = ModifyAction.Create;
			ctx.Model = new TModel();
			ctx.Editable = entity;
			ctx.ExtraArgument = ExtraArgument;
		}
		public static void InitUpdate<TModel, TEntity>(
			this IEntityModifyContext<TEntity, TModel> ctx,
			IDataContext dataContext,
			TModel model,
			TEntity entity,
			object ExtraArgument=null
			) where TModel : class
		{
			ctx.DataContext = dataContext;
			ctx.Action = ModifyAction.Update;
			ctx.Model = model;
			ctx.Editable = entity;
			ctx.ExtraArgument = ExtraArgument;
		}
		public static void InitRemove<TModel,TEntity>(
			this IEntityModifyContext<TEntity, TModel> ctx,
			IDataContext dataContext,
			TModel model,
			TEntity entity,
			object ExtraArgument=null
			) where TModel : class
		{
			ctx.DataContext = dataContext;
			ctx.Action = ModifyAction.Delete;
			ctx.Model = model;
			ctx.Editable = entity;
			ctx.ExtraArgument = ExtraArgument;
		}

		public static async Task<TKey> InternalCreateAsync<TKey,TEditable, TModel,TModifyContext>(
			this IEntityServiceContext ServiceContext,
			TModifyContext Context,
			TEditable Entity,
			Func<TModifyContext, Task> UpdateModel,
			Func<TModifyContext, Task> InitModel,
			object ExtraArgument = null,
			bool UseLightContext=false
			)
			where TModel : class, new()
			where TModifyContext: IEntityModifyContext<TEditable, TModel>
		{
			if (Entity.IsDefault())
				throw new ArgumentNullException("需要提供实体");
			ServiceContext.PermissionValidate(Operations.Create);

			return await ServiceContext.DataScope.Use(
				$"新建实体{typeof(TModel).Comment().Title}",
				async ctx =>
				{
					Context.InitCreate<TModel,TEditable>(ctx,Entity, ExtraArgument);

					if(InitModel!=null)
						await InitModel(Context);

					var set = ctx.Set<TModel>();
					set.Add(Context.Model);

					if (UpdateModel != null)
						await UpdateModel(Context);

					ServiceContext.PostChangedEvents<TEditable>(ctx,Context.Editable,DataActionType.Create);
					await ctx.SaveChangesAsync();
					return Entity<TModel>.GetKey<TKey>(Context.Model);
				},
				UseLightContext?DataContextFlag.LightMode:DataContextFlag.None
				);
		}
		public static async Task<TKey> CreateAsync<TKey, TEditable,TModel>(
			this IEntityServiceContext ServiceContext,
			TEditable Entity,
			Func<IEntityModifyContext<TEditable, TModel>, Task> UpdateModel,
			Func<IEntityModifyContext<TEditable, TModel>, Task> InitModel,
			object ExtraArgument=null,
			bool UseLightContext=false
			) 
			where TModel:class,new()
		{
			var ctx =new EntityModifyContext<TEditable, TModel>();
			return await InternalCreateAsync<TKey, TEditable,TModel,IEntityModifyContext<TEditable, TModel>>(
				ServiceContext, 
				ctx,
				Entity,
				UpdateModel, 
				InitModel,
				ExtraArgument,
				UseLightContext
				);
		}

		#endregion

		#region CreateOrUpdate

		public class CreateOrUpdateResult<TEditable,TModel>
		{
			public CreateOrUpdateResult()
			{

			}
			public TEditable Editable { get; set; }
			public TModel Model { get; set; }

			public static PropertyInfo PropEditable { get; } = typeof(CreateOrUpdateResult<TEditable, TModel>).GetProperty(nameof(Editable));
			public static PropertyInfo PropModel { get; } = typeof(CreateOrUpdateResult<TEditable, TModel>).GetProperty(nameof(Model));

			public static ParameterExpression Argument { get; } = Expression.Parameter(typeof(TModel), "m");
		}
		

		
		public static async Task<TKey> InternalCreateOrUpdateAsync<TKey, TEditable, TModel, TModifyContext>(
			this IEntityServiceContext ServiceContext,
			TModifyContext Context,
			
			TEditable Entity,
			Expression<Func<TModel,bool>> Selector,
			Func<TModifyContext, Task> UpdateModel,
			Func<TModifyContext, Task> InitModel,
			object ExtraArgument = null,
			bool EnableAutoModifier = false,
			bool AutoSaveChange=true
			)
			where TModel : class, new()
			where TModifyContext : IEntityModifyContext<TEditable, TModel>
		{
			ServiceContext.PermissionValidate(Operations.Create);


			return await ServiceContext.DataScope.Use(
				$"创建或修改实体{typeof(TModel).Comment().Title}",
				async ctx =>
				{
					var q = ctx.Queryable<TModel>(false).Where(Selector);
					var helper = ServiceContext.QueryResultBuildHelperCache.GetHelper<TModel, TEditable>(QueryMode.Edit);

					var expr = helper.BuildEntityMapper(CreateOrUpdateResult<TEditable, TModel>.Argument, 1, PropertySelector.All);
					var existQuery=q.Select(
						Expression.Lambda<Func<TModel, CreateOrUpdateResult<TEditable, TModel>>>(
							Expression.MemberInit(
								Expression.New(typeof(CreateOrUpdateResult<TEditable, TModel>)),
								Expression.Bind(CreateOrUpdateResult<TEditable, TModel>.PropEditable, expr),
								Expression.Bind(CreateOrUpdateResult<TEditable, TModel>.PropModel, CreateOrUpdateResult<TEditable, TModel>.Argument)
							),
							CreateOrUpdateResult<TEditable, TModel>.Argument
							)
						);

					var exist = await existQuery.SingleOrDefaultAsync();
					if (exist == null)
					{
						if (Entity.IsDefault())
							throw new ArgumentNullException("需要提供实体");
						Context.InitCreate<TModel, TEditable>(ctx,Entity, ExtraArgument);
						if (InitModel != null)
							await InitModel(Context);
						ctx.Add(Context.Model);
					}
					else
					{
						Context.InitUpdate<TModel, TEditable>(ctx,exist.Model, exist.Editable, ExtraArgument);
						ctx.Update(exist.Model);
					}

					if (UpdateModel != null)
						await UpdateModel(Context);

					ServiceContext.PostChangedEvents<TEditable>(ctx,Context.Editable, DataActionType.Create);
					if(AutoSaveChange)
						await ctx.SaveChangesAsync();
					return Entity<TModel>.GetKey<TKey>(Context.Model);
				},
				AutoSaveChange?DataContextFlag.None:DataContextFlag.LightMode
				);
		}
		public static async Task<TKey> CreateOrUpdateAsync<TKey, TEditable, TModel>(
			this IEntityServiceContext ServiceContext,
			TEditable Entity,
			Expression<Func<TModel, bool>> Selector,
			Func<IEntityModifyContext<TEditable, TModel>, Task> UpdateModel,
			Func<IEntityModifyContext<TEditable, TModel>, Task> InitModel,
			object ExtraArgument = null,
			bool EnableAutoModifier = false
			)
			where TModel : class, new()
		{
			var ctx = new EntityModifyContext<TEditable, TModel>();
			return await InternalCreateOrUpdateAsync<TKey, TEditable, TModel, IEntityModifyContext<TEditable, TModel>>(
				ServiceContext,
				ctx,
				Entity,
				Selector,
				UpdateModel,
				InitModel,
				ExtraArgument,
				EnableAutoModifier
				);
		}

		#endregion
		#region Update
		public static async Task<bool> InternalUpdateAsync<TKey, TEditable, TModel, TModifyContext>(
			this IEntityServiceContext ServiceContext,
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

			ServiceContext.PermissionValidate(Operations.Update);

			return await ServiceContext.DataScope.Use(
				$"编辑实体{typeof(TModel).Comment().Title}:{Entity<TEditable>.GetIdentValues(Entity)}",
				async ctx =>
				{
					var q = ctx.Queryable<TModel>(false).Where(Entity<TModel>.ObjectFilter(Entity));
					var model = LoadModelForEdit == null ? 
						await q.SingleOrDefaultAsync() : 
						await LoadModelForEdit(Entity<TEditable>.GetKey<TKey>(Entity),q);
					if (model == null)
						return false;

					InitUpdate(Context, ctx, model, Entity, null);

					if(UpdateModel != null)
						await UpdateModel(Context);

					ctx.Update(Context.Model);

					ServiceContext.PostChangedEvents<TEditable>(ctx,Context.Editable, DataActionType.Update);

					await ctx.SaveChangesAsync();
					return true;
				});
		}
		public static async Task<bool> UpdateAsync<TKey, TEditable,TModel>(
			this IEntityServiceContext ServiceContext,
			TEditable Entity,
			Func<IEntityModifyContext<TEditable, TModel>, Task> UpdateModel=null,
			Func<TKey,IContextQueryable<TModel>, Task<TModel>> LoadModelForEdit=null
			)
			where TModel : class
		{
			var ctx = new EntityModifyContext<TEditable, TModel>();
			return await InternalUpdateAsync(ServiceContext, ctx, Entity, UpdateModel, LoadModelForEdit);
		}

		#endregion

		#region remove
		public static async Task<bool> InternalRemoveAsync<TKey,TEditable, TModel,TModifyContext>(
			this IEntityServiceContext ServiceContext,
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
			ServiceContext.PermissionValidate(Operations.Remove);

			return await ServiceContext.DataScope.Use(
				$"删除实体{typeof(TModel).Comment().Title}:{Id}",
				async (ctx) =>
				{
					var q = ctx.Queryable<TModel>().Where(Entity<TModel>.ObjectFilter(Id));
					var model = LoadModelForEdit == null ? await q.SingleOrDefaultAsync() : await LoadModelForEdit(Id,q);
					if (model == null)
						return false;
					var editable = Entity<TKey>.GetKey<TEditable>(Id);
					InitRemove(Context, ctx, model, editable, null);

					if (RemoveModel != null)
						await RemoveModel(Context);
					ctx.Remove(Context.Model);

					ServiceContext.PostChangedEvents<TEditable>(ctx,Context.Editable, DataActionType.Delete);

					await ctx.SaveChangesAsync();
					return true;
				});
		}
		public static async Task<bool> RemoveAsync<TKey, TEditable, TModel>(
			this IEntityServiceContext ServiceContext,
			TKey Id,
			Func<IEntityModifyContext<TEditable, TModel>, Task> RemoveModel=null,
			Func<TKey, IContextQueryable<TModel>, Task<TModel>> LoadModelForEdit=null
			)
			where TModel : class
		{
			var ctx =(IEntityModifyContext < TEditable, TModel >) new EntityModifyContext<TEditable, TModel>();
			return await InternalRemoveAsync<TKey,TEditable, TModel, IEntityModifyContext<TEditable, TModel>>(
				ServiceContext, 
				ctx, 
				Id, 
				RemoveModel, 
				LoadModelForEdit
				);
		}
		public static async Task RemoveAllAsync<TKey,TEditable,TModel>(
			this IEntityServiceContext ServiceContext,
			Func<TKey, Task> Remove,
			Expression<Func<TModel,bool>> Condition=null,
			int BatchCount=100
			)
			where TModel : class
		{
			var paging = new Paging { Count = BatchCount };
			for (; ; )
			{
				var left = await ServiceContext.DataScope.Use(
					$"批量删除{typeof(TModel).Comment()}",
					async ctx =>
					{
						var q = ctx.Set<TModel>().AsQueryable();
						if (Condition != null)
							q = q.Where(Condition);
						var ids=await q.Select(Entity<TModel>.KeySelector<TKey>())
							.Take(BatchCount)
							.ToArrayAsync();

						foreach (var id in ids)
							await Remove(id);

						return ids.Length;
					}
				);
				if (left < BatchCount)
					break;
			}
		}

		public static Task RemoveAllScoppedAsync<TKey,TEditable, TModel>(
			this IEntityServiceContext ServiceContext,
			long ScopeId,
			Func<TKey, Task> Remove,
			int BatchCount = 100
			)
			where TModel : class, IEntityWithScope
			=> ServiceContext.RemoveAllAsync<TKey,TEditable,TModel>(
				Remove, 
				m =>
					m.ServiceDataScopeId == ScopeId, 
				BatchCount
				);
		#endregion

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
