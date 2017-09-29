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

		public static async Task<TReadOnlyEntity> GetAsync<TTempReadOnlyEntity, TReadOnlyEntity, TModel>
		(
			this IReadOnlyDataSetEntityManager<TModel> Storage,
			TReadOnlyEntity Id,
			Func<IContextQueryable<TModel>, IContextQueryable<TTempReadOnlyEntity>> MapModelToReadOnly,
			Func<TTempReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
		)
			where TModel : class
			where TReadOnlyEntity : class
		{
			return await Storage.UseTransaction(
				$"载入实体{typeof(TModel).Comment().Name}:{Id}",
				async (trans) =>
				{
					var re = await MapModelToReadOnly(
						Storage.DataSet.AsQueryable(true).Where(Entity<TModel>.ObjectFilter(Id))
						).SingleOrDefaultAsync();
					if (re == null)
						return null;
					var res = await PrepareReadOnly(new[] { re });
					return res[0];
				});
		}

		public static Task<TReadOnlyEntity> GetAsync<TReadOnlyEntity, TModel>
		(
			this IReadOnlyDataSetEntityManager<TModel> Storage,
			TReadOnlyEntity Id,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly,
			Func<TReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
		)
			where TModel : class
			where TReadOnlyEntity : class
			=> GetAsync<TReadOnlyEntity, TReadOnlyEntity, TModel>(
				Storage,
				Id,
				MapModelToReadOnly,
				PrepareReadOnly
				);

		public static Task<TReadOnlyEntity> GetAsync<TKey, TReadOnlyEntity, TModel>
		(
			this IReadOnlyDataSetEntityManager<TModel> Storage,
			TReadOnlyEntity Id,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly
		)
			where TModel : class
			where TReadOnlyEntity : class
			=> GetAsync<TReadOnlyEntity, TReadOnlyEntity, TModel>(
				Storage,
				Id,
				MapModelToReadOnly,
				re => Task.FromResult(re)
				);
		#endregion

		#region Batch Get

		public static async Task<TReadOnlyEntity[]> BatchGetAsync< TTempReadOnlyEntity, TReadOnlyEntity, TModel>
		(
			this IReadOnlyDataSetEntityManager<TModel> Storage,
			TReadOnlyEntity[] Ids,
			Func<IContextQueryable<TModel>, IContextQueryable<TTempReadOnlyEntity>> MapModelToReadOnly,
			Func<TTempReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
		)
			where TModel : class
			where TReadOnlyEntity : class
		{
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

		public static Task<TReadOnlyEntity[]> BatchGetAsync<TReadOnlyEntity, TModel>
		(
			this IReadOnlyDataSetEntityManager<TModel> Storage,
			TReadOnlyEntity[] Ids,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly,
			Func<TReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
		)
			where TModel : class
			where TReadOnlyEntity : class
			=> BatchGetAsync<TReadOnlyEntity, TReadOnlyEntity, TModel>(
				Storage,
				Ids,
				MapModelToReadOnly,
				PrepareReadOnly
				);

		public static Task<TReadOnlyEntity[]> BatchGetAsync<TReadOnlyEntity, TModel>
		(
			this IReadOnlyDataSetEntityManager<TModel> Storage,
			TReadOnlyEntity[] Ids,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly
		)
			where TModel : class
			where TReadOnlyEntity : class
			=> BatchGetAsync<TReadOnlyEntity, TReadOnlyEntity, TModel>(
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
					q = Entity<TModel>.TryFilterIdent(q, Arg);
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
				q = Entity<TModel>.TryFilterIdent(q,Arg);
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
		public static async Task<QueryResult<TReadOnlyEntity>> QueryAsync<TKey, TTempReadOnlyEntity, TReadOnlyEntity, TQueryArgument, TModel>(
			this IReadOnlyDataSetEntityManager<TModel> Storage,
			TQueryArgument Arg,
			Paging paging,
			Func<IContextQueryable<TModel>, TQueryArgument, Paging, IContextQueryable<TModel>> BuildQuery,
			IPagingQueryBuilder<TModel> PagingQueryBuilder,
			Func<IContextQueryable<TModel>, IContextQueryable<TTempReadOnlyEntity>> MapModelToReadOnly,
			Func<TTempReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
			)
			where TModel:class
		{
			return await Storage.UseTransaction(
				$"查询实体{typeof(TModel).Comment().Name}",
				async (trans) =>
				{
					var q = Storage.DataSet.AsQueryable(true);
					q = Entity<TModel>.TryFilterIdent(q, Arg);
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
		public static async Task<TEditable> LoadForEdit<TEditable, TModel>(
			this IDataSetEntityManager<TEditable,TModel> Storage,
			TEditable Key,
			Func<IContextQueryable<TModel>, Task<TEditable>> MapModelToEditable
			)
			where TModel:class
			where TEditable:class
		{
			return await Storage.UseTransaction(
				$"载入编辑实体{typeof(TModel).Comment().Name}:{Entity<TEditable>.GetIdents(Key).Join(",")}",
				async (trans) =>
				{
					return await MapModelToEditable(Storage.DataSet.AsQueryable(false).Where(Entity<TModel>.ObjectFilter(Key)));
				});
		}

		
		#endregion



		#region Create
		public static async Task InternalCreateAsync<TEditable, TModel,TModifyContext>(
			this IDataSetEntityManager<TEditable,TModel> Storage,
			TModifyContext Context,
			TEditable Entity,
			Func<TModifyContext, Task> UpdateModel,
			Func<TModifyContext, Task> InitModel,
			object ExtraArgument = null
			)
			where TModel : class, new()
			where TEditable : class
			where TModifyContext: IEntityModifyContext<TEditable, TModel>
		{
			await Storage.UseTransaction(
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
					return 0;
				});
		}
		public static async Task<TEditable> CreateAsync<TEditable,TModel>(
			this IDataSetEntityManager<TEditable,TModel> Storage,
			TEditable Entity,
			Func<IEntityModifyContext<TEditable, TModel>, Task> UpdateModel,
			Func<IEntityModifyContext<TEditable, TModel>, Task> InitModel,
			object ExtraArgument=null
			) 
			where TModel:class,new()
			where TEditable:class
		{
			var ctx =new EntityModifyContext<TEditable, TModel>();
			await InternalCreateAsync<TEditable,TModel,IEntityModifyContext<TEditable, TModel>>(
				Storage, 
				ctx,
				Entity,
				UpdateModel, 
				InitModel,
				ExtraArgument
				);
			return ctx.Editable;
		}

		#endregion

		#region Update
		public static async Task<TEditable> InternalUpdateAsync<TEditable, TModel,TModifyContext>(
			this IDataSetEntityManager<TEditable,TModel> Storage,
			TModifyContext Context,
			TEditable Entity,
			Func<TModifyContext, Task> UpdateModel,
			Func<TEditable,IContextQueryable<TModel>, Task<TModel>> LoadModelForEdit = null
			)
			where TModel : class
			where TEditable : class
			where TModifyContext:IEntityModifyContext<TEditable, TModel>
		{
			return await Storage.UseTransaction(
				$"编辑实体{typeof(TModel).Comment().Name}:{Entity<TEditable>.GetIdentString(Entity)}",
				async (trans) =>
				{
					var q = Storage.DataSet.AsQueryable(false).Where(Entity<TModel>.ObjectFilter(Entity));
					var model = LoadModelForEdit == null ? await q.SingleOrDefaultAsync() : await LoadModelForEdit(Entity,q);
					if (model == null)
						return null;
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
					return Entity;
				});
		}
		public static async Task<TEditable> UpdateAsync< TEditable,TModel>(
			this IDataSetEntityManager<TEditable,TModel> Storage,
			TEditable Entity,
			Func<IEntityModifyContext<TEditable, TModel>, Task> UpdateModel,
			Func<TEditable,IContextQueryable<TModel>, Task<TModel>> LoadModelForEdit=null
			)
			where TModel : class
			where TEditable :class
		{
			var ctx = new EntityModifyContext<TEditable, TModel>();
			return await InternalUpdateAsync(Storage, ctx, Entity, UpdateModel, LoadModelForEdit);
		}
		
		#endregion

		public static async Task<TEditable> InternalRemoveAsync<TModel,TEditable,TModifyContext>(
			this IDataSetEntityManager<TEditable,TModel> Storage,
			TModifyContext Context,
			TEditable Id,
			Func<TModifyContext, Task> RemoveModel = null,
			Func<TEditable,IContextQueryable<TModel>, Task<TModel>> LoadModelForEdit = null
			)
			where TModel : class
			where TEditable:class
			where TModifyContext: IEntityModifyContext<TEditable,TModel>
		{
			return await Storage.UseTransaction(
				$"删除实体{typeof(TModel).Comment().Name}:{Id}",
				async (trans) =>
				{
					var q = Storage.DataSet.AsQueryable(false).Where(Entity<TModel>.ObjectFilter(Id));
					var model = LoadModelForEdit == null ? await q.SingleOrDefaultAsync() : await LoadModelForEdit(Id,q);
					if (model == null)
						return null;
					Storage.InitRemoveContext(Context,Id, model, null);
					if (RemoveModel != null)
						await RemoveModel(Context);
					Storage.DataSet.Remove(Context.Model);

					Storage.AddPostAction(() =>
						Storage.EventEmitter.Emit(
							new EntityModified<TEditable>()
							{
								Entity=Id,
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
								Entity = Id,
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
					return Context.Editable;
				});
		}
		public static async Task<TEditable> RemoveAsync<TModel, TEditable>(
			this IDataSetEntityManager<TEditable, TModel> Storage,
			TEditable Id,
			Func<IEntityModifyContext<TEditable, TModel>, Task> RemoveModel=null,
			Func<TEditable, IContextQueryable<TModel>, Task<TModel>> LoadModelForEdit=null
			)
			where TModel : class
			where TEditable: class
		{
			var ctx =(IEntityModifyContext < TEditable, TModel >) new EntityModifyContext<TEditable, TModel>();
			return await InternalRemoveAsync<TModel,TEditable, IEntityModifyContext<TEditable, TModel>>(Storage, ctx, Id, RemoveModel, LoadModelForEdit);
		}
		public static async Task RemoveAllAsync<TEditable,TModel>(
			this IDataSetEntityManager<TEditable,TModel> Storage,
			Func<TEditable, Task> Remove,
			Expression<Func<TModel,bool>> Condition=null,
			int BatchCount=100
			)
			where TEditable:class
			where TModel : class
		{
			var tsm = Storage.DataSet.Context.TransactionScopeManager;
			var paging = new Paging { Count = BatchCount };
			for (; ; )
			{
				using (var scope = await tsm.CreateScope($"批量删除{typeof(TModel).Comment()}", TransactionScopeMode.RequireTransaction))
				{
					var q = Storage.DataSet.AsQueryable();
					if (Condition != null)
						q = q.Where(Condition);
					var ids = await q.Select(Entity<TModel>.KeySelector<TEditable>()).Take(BatchCount).ToArrayAsync();
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
			where TEditable : class
			where TModel : class, IEntityWithScope
			=> Storage.RemoveAllAsync(Remove, m => m.ScopeId == ScopeId, BatchCount);
 
	}
	public interface IReadOnlyEntityHelper<TReadOnlyTemp, TReadOnly, TQueryArgument, TModel>
		where TModel : class
	{
		Task<TReadOnly> GetAsync(
			TReadOnly Key,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyTemp>> MapModelToReadOnly,
			Func<TReadOnlyTemp[], Task<TReadOnly[]>> PrepareReadOnly
		);

		Task<TReadOnly[]> GetAsync(
			TReadOnly[] Key,
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

	public interface IEntityHelper<TReadOnlyTemp, TReadOnly, TQueryArgument, TEditable, TModel>:
		IReadOnlyEntityHelper<TReadOnlyTemp, TReadOnly, TQueryArgument, TModel>
		where TModel:class
	{
		Task<TEditable> LoadForEdit(
			TEditable Id, 
			Func<IContextQueryable<TModel>, Task<TEditable>> MapModelToEditable
			);

		Task<TEditable> CreateAsync(
			TEditable Entity,
			Func<IEntityModifyContext< TEditable, TModel>, Task> UpdateModel,
			Func<IEntityModifyContext<TEditable,TModel>, Task> InitModel
			);
		Task UpdateAsync(
			TEditable Entity,
			Func<IEntityModifyContext<TEditable, TModel>, Task<TModel>> LoadModelForEdit,
			Func<IEntityModifyContext<TEditable, TModel>, Task> UpdateModel
			);

		Task RemoveAsync(
			TEditable Entity,
			Func<IEntityModifyContext<TEditable, TModel>, Task<TModel>> LoadModelForEdit,
			Func<IEntityModifyContext<TEditable, TModel>, Task> RemoveModel
			);

		Task RemoveAllAsync();
		
	}
}
