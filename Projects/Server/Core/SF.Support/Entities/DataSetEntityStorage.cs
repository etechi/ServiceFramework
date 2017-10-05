using System.Threading.Tasks;
using System;
using SF.Core.ServiceManagement;
using System.Linq;
using SF.Data;
using System.Reflection;
using System.Linq.Expressions;
using System.Data.Common;
using System.Collections.Generic;

namespace SF.Entities
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
		public static void AddPostAction(
			this IDataSetEntityManager Storage,
			Action action,
			PostActionType Type
			)
		{
			Storage.DataSet.Context.TransactionScopeManager.CurrentScope.AddPostAction(
				action,
				Type
				);
		}
		public static void AddPostAction(
			this IDataSetEntityManager Storage,
			Func<Task> action,
			PostActionType Type
			)
		{
			Storage.DataSet.Context.TransactionScopeManager.CurrentScope.AddPostAction(
				action,
				Type
				);
		}
		public static Task<T> UseTransaction<T>(
			this IDataSetEntityManager Storage,
			string TransMessage,
			Func<DbTransaction, Task<T>> Action
			)
		{
			var ctx = Storage.DataSet.Context;
			return ctx.UseTransaction(TransMessage, Action);
		}

		#region GetAsync

		public static async Task<TReadOnlyEntity> GetAsync<TKey,TTempReadOnlyEntity, TReadOnlyEntity, TModel>
		(
			this IReadOnlyDataSetEntityManager<TModel> Storage,
			TKey Id,
			Func<IContextQueryable<TModel>, IContextQueryable<TTempReadOnlyEntity>> MapModelToReadOnly,
			Func<TTempReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
		)
			where TModel : class
		{
			if (Id.IsDefault())
				return default(TReadOnlyEntity);

			return await Storage.UseTransaction(
				$"载入实体{typeof(TModel).Comment().Name}:{Id}",
				async (trans) =>
				{
					var re = await MapModelToReadOnly(
						Storage.DataSet.AsQueryable(true).Where(Entity<TModel>.ObjectFilter(Id))
						).SingleOrDefaultAsync();
					if (re == null)
						return default(TReadOnlyEntity);
					var res = await PrepareReadOnly(new[] { re });
					return res[0];
				});
		}

		public static Task<TReadOnlyEntity> GetAsync<TKey, TReadOnlyEntity, TModel>
		(
			this IReadOnlyDataSetEntityManager<TModel> Storage,
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
			this IReadOnlyDataSetEntityManager<TModel> Storage,
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

		public static async Task<TReadOnlyEntity[]> BatchGetAsync<TKey, TTempReadOnlyEntity, TReadOnlyEntity, TModel>
		(
			this IReadOnlyDataSetEntityManager<TModel> Storage,
			TKey[] Ids,
			Func<IContextQueryable<TModel>, IContextQueryable<TTempReadOnlyEntity>> MapModelToReadOnly,
			Func<TTempReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
		)
			where TModel : class
		{
			if (Ids == null || Ids.Length == 0)
				return Array.Empty<TReadOnlyEntity>();

			return await Storage.UseTransaction(
				$"批量载入实体：{typeof(TModel).Comment().Name}",
				async (trans) =>
			{
				var re = await MapModelToReadOnly(
					Storage.DataSet.AsQueryable(true).Where(Entity<TModel>.MultipleObjectFilter(Ids))
					).ToArrayAsync();
				if (re == null)
					return null;
				var res = await PrepareReadOnly(re);
				return res;
			});
		}

		public static Task<TReadOnlyEntity[]> BatchGetAsync<TKey,TReadOnlyEntity, TModel>
		(
			this IReadOnlyDataSetEntityManager<TModel> Storage,
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
			this IReadOnlyDataSetEntityManager<TModel> Storage,
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
			this IReadOnlyDataSetEntityManager<TModel> Storage,
			TQueryArgument Arg,
			Paging paging,
			Func<IContextQueryable<TModel>, TQueryArgument, Paging, IContextQueryable<TModel>> BuildQuery,
			IPagingQueryBuilder<TModel> PagingQueryBuilder
			)
			where TModel : class
		{
			return await Storage.UseTransaction(
				$"查询实体主键：{typeof(TModel).Comment().Name}",
				async (trans) =>
				{
					var q = Storage.DataSet.AsQueryable(true);
					q = Entity<TModel>.QueryIdentFilter(q, Arg);
					q = BuildQuery(q, Arg, paging);
					var re = await q.ToQueryResultAsync(
						qs => qs.Select(Entity<TModel>.KeySelector<TKey>()),
						PagingQueryBuilder,
						paging
						);
					return re;
				});

		}

		#endregion

		#region Query
		public static async Task<QueryResult<TReadOnlyEntity>> QueryAsync<TTempReadOnlyEntity, TReadOnlyEntity, TQueryArgument, TModel>(
			this IReadOnlyDataSetEntityManager<TModel> Storage,
			TQueryArgument Arg,
			Paging paging,
			Func<IContextQueryable<TModel>, TQueryArgument, Paging, IContextQueryable<TModel>> BuildQuery,
			IPagingQueryBuilder<TModel> PagingQueryBuilder,
			Func<IContextQueryable<TModel>, IContextQueryable<TTempReadOnlyEntity>> MapModelToReadOnly,
			Func<TTempReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
			)
			where TModel : class
		{
			return await Storage.UseTransaction(
				$"查询实体{typeof(TModel).Comment().Name}",
				async (trans) =>
			{
				var q = Storage.DataSet.AsQueryable(true);
				q = Entity<TModel>.QueryIdentFilter(q,Arg);
				q = BuildQuery(q, Arg, paging);
				var re = await q.ToQueryResultAsync(
					MapModelToReadOnly,
					PrepareReadOnly,
					PagingQueryBuilder,
					paging
					);
				return re;
			});
		}
	
		public static Task<QueryResult<TReadOnlyEntity>> QueryAsync<TReadOnlyEntity, TQueryArgument, TModel>(
			this IReadOnlyDataSetEntityManager<TModel> Storage,
			TQueryArgument Arg,
			Paging paging,
			Func<IContextQueryable<TModel>, TQueryArgument, Paging, IContextQueryable<TModel>> BuildQuery,
			IPagingQueryBuilder<TModel> PagingQueryBuilder,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly,
			Func<TReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
			)
			where TModel : class
			=> QueryAsync<TReadOnlyEntity,TReadOnlyEntity, TQueryArgument, TModel>(
				Storage, 
				Arg, 
				paging, 
				BuildQuery,
				PagingQueryBuilder,
				MapModelToReadOnly, 
				PrepareReadOnly
				);
	

		public static Task<QueryResult<TReadOnlyEntity>> QueryAsync<TReadOnlyEntity, TQueryArgument, TModel>(
			this IReadOnlyDataSetEntityManager<TModel> Storage,
			TQueryArgument Arg,
			Paging paging,
			Func<IContextQueryable<TModel>, TQueryArgument, Paging, IContextQueryable<TModel>> BuildQuery,
			IPagingQueryBuilder<TModel> PagingQueryBuilder,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly
			)
			where TModel : class
			=> QueryAsync<TReadOnlyEntity, TReadOnlyEntity, TQueryArgument, TModel>(
				Storage,
				Arg,
				paging,
				BuildQuery,
				PagingQueryBuilder,
				MapModelToReadOnly,
				re=>Task.FromResult(re)
				);

		#endregion

		#region LoadForEdit
		public static async Task<TEditable> LoadForEdit<TKey,TEditable, TModel>(
			this IDataSetEntityManager<TEditable,TModel> Storage,
			TKey Key,
			Func<IContextQueryable<TModel>, Task<TEditable>> MapModelToEditable
			)
			where TModel:class
		{
			if (Key.IsDefault())
				return default(TEditable);
			return await Storage.UseTransaction(
				$"载入编辑实体{typeof(TModel).Comment().Name}:{Entity<TKey>.GetIdents(Key)?.Join(",")}",
				async (trans) =>
				{
					return await MapModelToEditable(Storage.DataSet.AsQueryable(false).Where(Entity<TModel>.ObjectFilter(Key)));
				});
		}

		
		#endregion



		#region Create
		public static async Task<TKey> InternalCreateAsync<TKey,TEditable, TModel,TModifyContext>(
			this IDataSetEntityManager<TEditable,TModel> Storage,
			TModifyContext Context,
			TEditable Entity,
			Func<TModifyContext, Task> UpdateModel,
			Func<TModifyContext, Task> InitModel,
			object ExtraArgument = null
			)
			where TModel : class, new()
			where TModifyContext: IEntityModifyContext<TEditable, TModel>
		{
			if (Entity.IsDefault())
				throw new ArgumentNullException("需要提供实体");

			return await Storage.UseTransaction(
				$"新建实体{typeof(TModel).Comment().Name}",
				async (trans) =>
				{
					Storage.InitCreateContext(Context,Entity, ExtraArgument);
					
					await InitModel(Context);
					await UpdateModel(Context);
					Storage.DataSet.Add(Context.Model);
					Storage.AddPostAction(() =>
						Storage.EventEmitter.Emit(
							new EntityModified<TEditable>()
							{
								Entity=Context.Editable,
								Action = DataActionType.Create,
								ServiceId = Storage.ServiceInstanceDescroptor?.InstanceId,
								Time = Storage.Now,
								PostActionType=PostActionType.BeforeCommit
							}
							),
							PostActionType.BeforeCommit
						);
					Storage.AddPostAction(() =>
						Storage.EventEmitter.Emit(
							new EntityModified< TEditable>()
							{
								Entity = Context.Editable,
								Action = DataActionType.Create,
								ServiceId = Storage.ServiceInstanceDescroptor?.InstanceId,
								Time = Storage.Now,
								PostActionType = PostActionType.AfterCommit
							},
							false
							),
							PostActionType.AfterCommit
						);
					await Storage.DataSet.Context.SaveChangesAsync();
					return Entity<TModel>.GetKey<TKey>(Context.Model);
				});
		}
		public static async Task<TKey> CreateAsync<TKey, TEditable,TModel>(
			this IDataSetEntityManager<TEditable,TModel> Storage,
			TEditable Entity,
			Func<IEntityModifyContext<TEditable, TModel>, Task> UpdateModel,
			Func<IEntityModifyContext<TEditable, TModel>, Task> InitModel,
			object ExtraArgument=null
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
				ExtraArgument
				);
		}

		#endregion

		#region Update
		public static async Task<bool> InternalUpdateAsync<TKey,TEditable, TModel,TModifyContext>(
			this IDataSetEntityManager<TEditable,TModel> Storage,
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

			return await Storage.UseTransaction(
				$"编辑实体{typeof(TModel).Comment().Name}:{Entity<TEditable>.GetIdentString(Entity)}",
				async (trans) =>
				{
					var q = Storage.DataSet.AsQueryable(false).Where(Entity<TModel>.ObjectFilter(Entity));
					var model = LoadModelForEdit == null ? 
						await q.SingleOrDefaultAsync() : 
						await LoadModelForEdit(Entity<TEditable>.GetKey<TKey>(Entity),q);
					if (model == null)
						return false;
					Storage.InitUpdateContext(Context,Entity, model, null);
					await UpdateModel(Context);
					Storage.DataSet.Update(Context.Model);

					Storage.AddPostAction(() =>
						Storage.EventEmitter.Emit(
							new EntityModified<TEditable>()
							{
								Entity = Entity,
								Action = DataActionType.Update,
								ServiceId = Storage.ServiceInstanceDescroptor?.InstanceId,
								Time = Storage.Now,
								PostActionType = PostActionType.BeforeCommit
							}
							),
							PostActionType.BeforeCommit
						);
					Storage.AddPostAction(() =>
						Storage.EventEmitter.Emit(
							new EntityModified<TEditable>()
							{
								Entity=Entity,
								Action = DataActionType.Update,
								ServiceId = Storage.ServiceInstanceDescroptor?.InstanceId,
								Time = Storage.Now,
								PostActionType = PostActionType.AfterCommit
							},
							false
							),
							PostActionType.AfterCommit
						);

					await Storage.DataSet.Context.SaveChangesAsync();
					return true;
				});
		}
		public static async Task<bool> UpdateAsync<TKey, TEditable,TModel>(
			this IDataSetEntityManager<TEditable,TModel> Storage,
			TEditable Entity,
			Func<IEntityModifyContext<TEditable, TModel>, Task> UpdateModel,
			Func<TKey,IContextQueryable<TModel>, Task<TModel>> LoadModelForEdit=null
			)
			where TModel : class
		{
			var ctx = new EntityModifyContext<TEditable, TModel>();
			return await InternalUpdateAsync(Storage, ctx, Entity, UpdateModel, LoadModelForEdit);
		}
		
		#endregion

		public static async Task<bool> InternalRemoveAsync<TKey,TEditable, TModel,TModifyContext>(
			this IDataSetEntityManager<TEditable, TModel> Storage,
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

			return await Storage.UseTransaction(
				$"删除实体{typeof(TModel).Comment().Name}:{Id}",
				async (trans) =>
				{
					var q = Storage.DataSet.AsQueryable(false).Where(Entity<TModel>.ObjectFilter(Id));
					var model = LoadModelForEdit == null ? await q.SingleOrDefaultAsync() : await LoadModelForEdit(Id,q);
					if (model == null)
						return false;
					var editable = Entity<TKey>.GetKey<TEditable>(Id);
					Storage.InitRemoveContext(Context,editable, model, null);
					if (RemoveModel != null)
						await RemoveModel(Context);
					Storage.DataSet.Remove(Context.Model);

					Storage.AddPostAction(() =>
						Storage.EventEmitter.Emit(
							new EntityModified<TEditable>()
							{
								Entity= editable,
								Action = DataActionType.Delete,
								ServiceId = Storage.ServiceInstanceDescroptor?.InstanceId,
								Time = Storage.Now,
								PostActionType = PostActionType.BeforeCommit
							}
							),
							PostActionType.BeforeCommit
						);
					Storage.AddPostAction(() =>
						Storage.EventEmitter.Emit(
							new EntityModified<TEditable>()
							{
								Entity = editable,
								Action = DataActionType.Delete,
								ServiceId = Storage.ServiceInstanceDescroptor?.InstanceId,
								Time = Storage.Now,
								PostActionType = PostActionType.AfterCommit
							},
							false
							),
							PostActionType.AfterCommit
						);

					await Storage.DataSet.Context.SaveChangesAsync();
					return true;
				});
		}
		public static async Task<bool> RemoveAsync<TKey, TEditable, TModel>(
			this IDataSetEntityManager<TEditable, TModel> Storage,
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
			this IDataSetEntityManager<TEditable,TModel> Storage,
			Func<TKey, Task> Remove,
			Expression<Func<TModel,bool>> Condition=null,
			int BatchCount=100
			)
			where TModel : class
		{
			var tsm = Storage.DataSet.Context.TransactionScopeManager;
			var paging = new Paging { Count = BatchCount };
			for (; ; )
			{
				var batchIndex = 0;
				using (var scope = await tsm.CreateScope($"批量删除{typeof(TModel).Comment()}", TransactionScopeMode.RequireTransaction))
				{
					var ids = await Storage.UseTransaction($"查询批量删除主键{batchIndex++}",async (trans) =>
					{
						var q = Storage.DataSet.AsQueryable();
						if (Condition != null)
							q = q.Where(Condition);
						return await q.Select(Entity<TModel>.KeySelector<TKey>()).Take(BatchCount).ToArrayAsync();
					});
					foreach (var id in ids)
						await Remove(id);
					await scope.Commit();
					if (ids.Length < BatchCount)
						break;
				}
			}
		}

		public static Task RemoveAllScoppedAsync<TEditable, TModel>(
			this IDataSetEntityManager<TEditable, TModel> Storage,
			long ScopeId,
			Func<TEditable, Task> Remove,
			int BatchCount = 100
			)
			where TModel : class, IEntityWithScope
			=> Storage.RemoveAllAsync(Remove, m => m.ScopeId == ScopeId, BatchCount);
 
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
