using System.Threading.Tasks;
using System;
using SF.Core.ServiceManagement;
using System.Linq;
using SF.Data;
using System.Reflection;
using System.Linq.Expressions;

namespace SF.Entities
{
	public static class DataSetEntityStorage
	{
		public static void AddPostAction(
			this IDataSetEntityManager Storage,
			Action action,
			bool CallOnSaved = true
			)
		{
			Storage.DataSet.Context.TransactionScopeManager.CurrentScope.AddPostAction(
				action, 
				CallOnSaved
				);
		}
		public static void AddPostAction(
			this IDataSetEntityManager Storage,
			Func<Task> action,
			bool CallOnSaved = true
			)
		{
			Storage.DataSet.Context.TransactionScopeManager.CurrentScope.AddPostAction(
				action,
				CallOnSaved
				);
		}
		public static async Task<T> UseTransaction<TModel,T>(
			this IDataSetEntityManager<TModel> Storage,
			string TransMessage,
			Func<Task<T>> Action
			)
			where TModel : class
		{
			var ctx = Storage.DataSet.Context;
			var tm = ctx.TransactionScopeManager;
			using (var ts = tm.CreateScope(TransMessage, TransactionScopeMode.RequireTransaction))
			{
				var tran = tm.CurrentDbTransaction;
				var provider = ctx.Provider;
				var orgTran = provider.Transaction;
				if (orgTran == tran)
					return await Action();
				provider.Transaction = tran;
				try
				{
					return await Action();
				}
				finally
				{
					provider.Transaction = orgTran;
				}
			}
		}

		#region GetAsync

		public static async Task<TReadOnlyEntity> GetAsync<TKey, TTempReadOnlyEntity, TReadOnlyEntity, TModel>
		(
			this IDataSetEntityManager<TModel> Storage,
			TKey Id,
			Func<IContextQueryable<TModel>, IContextQueryable<TTempReadOnlyEntity>> MapModelToReadOnly,
			Func<TTempReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
		)
			where TKey : IEquatable<TKey>
			where TModel : class, IEntityWithId<TKey>
			where TReadOnlyEntity : class
		{
			return await Storage.UseTransaction(
				$"载入实体{typeof(TModel).Comment().Name}:{Id}",
				async () =>
				{
					var re = await MapModelToReadOnly(
						Storage.DataSet.AsQueryable(true).Where(s => Id.Equals(s.Id))
						).SingleOrDefaultAsync();
					if (re == null)
						return null;
					var res = await PrepareReadOnly(new[] { re });
					return res[0];
				});
		}

		public static Task<TReadOnlyEntity> GetAsync<TKey, TReadOnlyEntity, TModel>
		(
			this IDataSetEntityManager<TModel> Storage,
			TKey Id,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly,
			Func<TReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
		)
			where TKey : IEquatable<TKey>
			where TModel : class, IEntityWithId<TKey>
			where TReadOnlyEntity : class
			=> GetAsync<TKey, TReadOnlyEntity, TReadOnlyEntity, TModel>(
				Storage,
				Id,
				MapModelToReadOnly,
				PrepareReadOnly
				);

		public static Task<TReadOnlyEntity> GetAsync<TKey, TReadOnlyEntity, TModel>
		(
			this IDataSetEntityManager<TModel> Storage,
			TKey Id,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly
		)
			where TKey : IEquatable<TKey>
			where TModel : class, IEntityWithId<TKey>
			where TReadOnlyEntity : class
			=> GetAsync<TKey, TReadOnlyEntity, TReadOnlyEntity, TModel>(
				Storage,
				Id,
				MapModelToReadOnly,
				re => Task.FromResult(re)
				);
		#endregion

		#region Batch Get

		public static async Task<TReadOnlyEntity[]> GetAsync<TKey, TTempReadOnlyEntity, TReadOnlyEntity, TModel>
		(
			this IDataSetEntityManager<TModel> Storage,
			TKey[] Ids,
			Func<IContextQueryable<TModel>, IContextQueryable<TTempReadOnlyEntity>> MapModelToReadOnly,
			Func<TTempReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
		)
			where TKey : IEquatable<TKey>
			where TModel : class, IEntityWithId<TKey>
			where TReadOnlyEntity : class
		{
			return await Storage.UseTransaction(
				$"批量载入实体：{typeof(TModel).Comment().Name}",
				async () =>
			{
				var re = await MapModelToReadOnly(
					Storage.DataSet.AsQueryable(true).Where(s => Ids.Contains(s.Id))
					).ToArrayAsync();
				if (re == null)
					return null;
				var res = await PrepareReadOnly(re);
				return res;
			});
		}

		public static Task<TReadOnlyEntity[]> GetAsync<TKey, TReadOnlyEntity, TModel>
		(
			this IDataSetEntityManager<TModel> Storage,
			TKey[] Ids,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly,
			Func<TReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
		)
			where TKey : IEquatable<TKey>
			where TModel : class, IEntityWithId<TKey>
			where TReadOnlyEntity : class
			=> GetAsync<TKey, TReadOnlyEntity, TReadOnlyEntity, TModel>(
				Storage,
				Ids,
				MapModelToReadOnly,
				PrepareReadOnly
				);

		public static Task<TReadOnlyEntity[]> GetAsync<TKey, TReadOnlyEntity, TModel>
		(
			this IDataSetEntityManager<TModel> Storage,
			TKey[] Ids,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly
		)
			where TKey : IEquatable<TKey>
			where TModel : class, IEntityWithId<TKey>
			where TReadOnlyEntity : class
			=> GetAsync<TKey, TReadOnlyEntity, TReadOnlyEntity, TModel>(
				Storage,
				Ids,
				MapModelToReadOnly,
				re => Task.FromResult(re)
				);
		#endregion
		
		
		#region Query Idents

		public static async Task<QueryResult<TKey>> QueryIdentsAsync<TKey, TQueryArgument, TModel>(
			this IDataSetEntityManager<TModel> Storage,
			TQueryArgument Arg,
			Paging paging,
			Func<IContextQueryable<TModel>, TQueryArgument, Paging, IContextQueryable<TModel>> BuildQuery,
			PagingQueryBuilder<TModel> PagingQueryBuilder
			)
			where TKey : IEquatable<TKey>
			where TModel : class, IEntityWithId<TKey>
			where TQueryArgument : IQueryArgument<TKey>
		{
			return await Storage.UseTransaction(
				$"查询实体主键：{typeof(TModel).Comment().Name}",
				async () =>
				{
					var q = Storage.DataSet.AsQueryable(true);
					if (Arg.Id.HasValue)
					{
						var id = Arg.Id.Value;
						q = q.Where(e => e.Id.Equals(id));
					}
					q = BuildQuery(q, Arg, paging);
					var re = await q.ToQueryResultAsync(
						qs => qs.Select(i => i.Id),
						PagingQueryBuilder,
						paging
						);
					return re;
				});

		}

		public static Task<QueryResult<long>> QueryIdentsAsync<TQueryArgument, TModel>(
			this IDataSetEntityManager<TModel> Storage,
			TQueryArgument Arg,
			Paging paging,
			Func<IContextQueryable<TModel>, TQueryArgument, Paging, IContextQueryable<TModel>> BuildQuery,
			PagingQueryBuilder<TModel> PagingQueryBuilder
			)
			where TModel : class, IEntityWithId<long>
			where TQueryArgument : IQueryArgument<long>
			=> QueryIdentsAsync<long, TQueryArgument, TModel>(Storage, Arg, paging, BuildQuery, PagingQueryBuilder);
		#endregion

		#region Query
		public static async Task<QueryResult<TReadOnlyEntity>> QueryAsync<TKey, TTempReadOnlyEntity, TReadOnlyEntity, TQueryArgument, TModel>(
			this IDataSetEntityManager<TModel> Storage,
			TQueryArgument Arg,
			Paging paging,
			Func<IContextQueryable<TModel>, TQueryArgument, Paging, IContextQueryable<TModel>> BuildQuery,
			PagingQueryBuilder<TModel> PagingQueryBuilder,
			Func<IContextQueryable<TModel>, IContextQueryable<TTempReadOnlyEntity>> MapModelToReadOnly,
			Func<TTempReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
			)
			where TKey : IEquatable<TKey>
			where TModel : class, IEntityWithId<TKey>
			where TQueryArgument : IQueryArgument<TKey>
		{
			return await Storage.UseTransaction(
				$"查询实体{typeof(TModel).Comment().Name}",
				async () =>
			{
				var q = Storage.DataSet.AsQueryable(true);
				if (Arg.Id.HasValue)
				{
					var id = Arg.Id.Value;
					q = q.Where(e => e.Id.Equals(id));
				}
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
		public static  Task<QueryResult<TReadOnlyEntity>> QueryAsync<TTempReadOnlyEntity, TReadOnlyEntity, TQueryArgument, TModel>(
			this IDataSetEntityManager<TModel> Storage,
			TQueryArgument Arg,
			Paging paging,
			Func<IContextQueryable<TModel>, TQueryArgument, Paging, IContextQueryable<TModel>> BuildQuery,
			PagingQueryBuilder<TModel> PagingQueryBuilder,
			Func<IContextQueryable<TModel>, IContextQueryable<TTempReadOnlyEntity>> MapModelToReadOnly,
			Func<TTempReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
			)
			where TModel : class, IEntityWithId<long>
			where TQueryArgument : IQueryArgument<long>
			=> Storage.QueryAsync(Arg, paging, BuildQuery, PagingQueryBuilder, MapModelToReadOnly, PrepareReadOnly);

		public static Task<QueryResult<TReadOnlyEntity>> QueryAsync<TKey, TReadOnlyEntity, TQueryArgument, TModel>(
			this IDataSetEntityManager<TModel> Storage,
			TQueryArgument Arg,
			Paging paging,
			Func<IContextQueryable<TModel>, TQueryArgument, Paging, IContextQueryable<TModel>> BuildQuery,
			PagingQueryBuilder<TModel> PagingQueryBuilder,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly,
			Func<TReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
			)
			where TKey : IEquatable<TKey>
			where TModel : class, IEntityWithId<TKey>
			where TQueryArgument : IQueryArgument<TKey>
			=> QueryAsync<TKey, TReadOnlyEntity,TReadOnlyEntity, TQueryArgument, TModel>(
				Storage, 
				Arg, 
				paging, 
				BuildQuery,
				PagingQueryBuilder,
				MapModelToReadOnly, 
				PrepareReadOnly
				);
		public static Task<QueryResult<TReadOnlyEntity>> QueryAsync<TReadOnlyEntity, TQueryArgument, TModel>(
			this IDataSetEntityManager<TModel> Storage,
			TQueryArgument Arg,
			Paging paging,
			Func<IContextQueryable<TModel>, TQueryArgument, Paging, IContextQueryable<TModel>> BuildQuery,
			PagingQueryBuilder<TModel> PagingQueryBuilder,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly,
			Func<TReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
			)
			where TModel : class, IEntityWithId<long>
			where TQueryArgument : IQueryArgument<long>
			=> Storage.QueryAsync<long,TReadOnlyEntity,TReadOnlyEntity,TQueryArgument, TModel>(
				Arg, 
				paging, 
				BuildQuery, 
				PagingQueryBuilder, 
				MapModelToReadOnly, 
				PrepareReadOnly
				);


		public static Task<QueryResult<TReadOnlyEntity>> QueryAsync<TKey, TReadOnlyEntity, TQueryArgument, TModel>(
			this IDataSetEntityManager<TModel> Storage,
			TQueryArgument Arg,
			Paging paging,
			Func<IContextQueryable<TModel>, TQueryArgument, Paging, IContextQueryable<TModel>> BuildQuery,
			PagingQueryBuilder<TModel> PagingQueryBuilder,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly
			)
			where TKey : IEquatable<TKey>
			where TModel : class, IEntityWithId<TKey>
			where TQueryArgument : IQueryArgument<TKey>
			=> QueryAsync<TKey, TReadOnlyEntity, TReadOnlyEntity, TQueryArgument, TModel>(
				Storage,
				Arg,
				paging,
				BuildQuery,
				PagingQueryBuilder,
				MapModelToReadOnly,
				re=>Task.FromResult(re)
				);

		public static Task<QueryResult<TReadOnlyEntity>> QueryAsync<TReadOnlyEntity, TQueryArgument, TModel>(
			this IDataSetEntityManager<TModel> Storage,
			TQueryArgument Arg,
			Paging paging,
			Func<IContextQueryable<TModel>, TQueryArgument, Paging, IContextQueryable<TModel>> BuildQuery,
			PagingQueryBuilder<TModel> PagingQueryBuilder,
			Func<IContextQueryable<TModel>, IContextQueryable<TReadOnlyEntity>> MapModelToReadOnly
			)
			where TModel : class, IEntityWithId<long>
			where TQueryArgument : IQueryArgument<long>
			=> Storage.QueryAsync<long, TReadOnlyEntity, TReadOnlyEntity, TQueryArgument, TModel>(
				Arg, paging,
				BuildQuery,
				PagingQueryBuilder,
				MapModelToReadOnly,
				re => Task.FromResult(re)
				);

		#endregion

		#region LoadForEdit
		public static async Task<TEditableEntity> LoadForEdit<TKey, TEditableEntity, TModel>(
			this IDataSetEntityManager<TModel> Storage,
			TKey Id,
			Func<IContextQueryable<TModel>, Task<TEditableEntity>> MapModelToEditable
			)
			where TKey:IEquatable<TKey>
			where TModel:class,IEntityWithId<TKey>
		{
			return await Storage.UseTransaction(
				$"载入编辑实体{typeof(TModel).Comment().Name}:{Id}",
				async () =>
				{
					return await MapModelToEditable(Storage.DataSet.AsQueryable(false).Where(m => m.Id.Equals(Id)));
				});
		}

		public static Task<TEditableEntity> LoadForEdit<TEditableEntity, TModel>(
			this IDataSetEntityManager<TModel> Storage,
			long Id,
			Func<IContextQueryable<TModel>, Task<TEditableEntity>> MapModelToEditable
			)	where TModel:class,IEntityWithId<long>
			=> Storage.LoadForEdit<long, TEditableEntity, TModel>(Id, MapModelToEditable);
		#endregion



		#region Create

		public static async Task<TKey> CreateAsync<TKey,TEditableEntity,TModel>(
			this IDataSetEntityManager<TModel> Storage,
			TEditableEntity Entity,
			Func<IEntityModifyContext<TKey, TEditableEntity, TModel>, Task> UpdateModel,
			Func<IEntityModifyContext<TKey, TEditableEntity, TModel>, Task> InitModel,
			object ExtraArgument=null
			) 
			where TKey:IEquatable<TKey>
			where TModel:class,IEntityWithId<TKey>,new()
			where TEditableEntity:class,IEntityWithId<TKey>
		{
			return await Storage.UseTransaction(
				$"新建实体{typeof(TModel).Comment().Name}",
				async () =>
				{
					var ctx = Storage.NewCreateContext<TKey,TEditableEntity>(Entity, ExtraArgument);
					await InitModel(ctx);
					await UpdateModel(ctx);
					Storage.DataSet.Add(ctx.Model);
					await Storage.DataSet.Context.SaveChangesAsync();
					return ctx.Model.Id;
				});

		}
		public static Task<long> CreateAsync<TEditableEntity, TModel>(
			this IDataSetEntityManager<TModel> Storage,
			TEditableEntity Entity,
			Func<IEntityModifyContext<long, TEditableEntity, TModel>, Task> UpdateModel,
			Func<IEntityModifyContext<long, TEditableEntity, TModel>, Task> InitModel,
			object ExtraArgument = null
			)
			where TModel : class, IEntityWithId<long>, new()
			where TEditableEntity : class, IEntityWithId<long>
			=> Storage.CreateAsync<long,TEditableEntity,TModel>(Entity, UpdateModel, InitModel, ExtraArgument);

		#endregion

		#region Update
		public static async Task UpdateAsync<TKey, TEditableEntity,TModel>(
			this IDataSetEntityManager<TModel> Storage,
			TEditableEntity Entity,
			Func<IEntityModifyContext<TKey, TEditableEntity, TModel>, Task> UpdateModel,
			Func<IContextQueryable<TModel>, Task<TModel>> LoadModelForEdit=null
			)
			where TKey:IEquatable<TKey>
			where TModel : class, IEntityWithId<TKey>
			where TEditableEntity :class,IEntityWithId<TKey>
		{
			await Storage.UseTransaction(
				$"编辑实体{typeof(TModel).Comment().Name}:{Entity.Id}",
				async () =>
				{
					var id = Entity.Id;
					var q = Storage.DataSet.AsQueryable(false).Where(s => s.Id.Equals(id));
					var model = LoadModelForEdit == null ? await q.SingleOrDefaultAsync() : await LoadModelForEdit(q);
					var ctx = Storage.NewUpdateContext<TKey, TEditableEntity>(Entity, model, null);
					await UpdateModel(ctx);
					Storage.DataSet.Update(ctx.Model);
					await Storage.DataSet.Context.SaveChangesAsync();
					return 0;
				});
		}
		public static Task UpdateAsync<TEditableEntity, TModel>(
			this IDataSetEntityManager<TModel> Storage,
			TEditableEntity Entity,
			Func<IEntityModifyContext<long, TEditableEntity, TModel>, Task> UpdateModel,
			Func<IContextQueryable<TModel>, Task<TModel>> LoadModelForEdit=null
			)
			where TModel : class, IEntityWithId<long>
			where TEditableEntity : class, IEntityWithId<long>
			=> Storage.UpdateAsync<long,TEditableEntity,TModel>(Entity, UpdateModel, LoadModelForEdit);

		#endregion

		public static async Task<bool> RemoveAsync<TKey,TModel>(
			this IDataSetEntityManager<TModel> Storage,
			TKey Id,
			Func<IEntityModifyContext<TKey, TModel>, Task> RemoveModel=null,
			Func<IContextQueryable<TModel>, Task<TModel>> LoadModelForEdit=null
			)
			where TKey : IEquatable<TKey>
			where TModel : class, IEntityWithId<TKey>
		{
			return await Storage.UseTransaction(
				$"删除实体{typeof(TModel).Comment().Name}:{Id}",
				async () =>
				{
					var q = Storage.DataSet.AsQueryable(false).Where(s => s.Id.Equals(Id));
					var model = LoadModelForEdit==null?await q.SingleOrDefaultAsync(): await LoadModelForEdit(q);
					if (model == null)
						return false;
					var ctx = Storage.NewRemoveContext<TKey>(model, null);
					if(RemoveModel!=null)
						await RemoveModel(ctx);
					Storage.DataSet.Remove(ctx.Model);
					await Storage.DataSet.Context.SaveChangesAsync();
					return true;
				});
		}
		public static async Task RemoveAllAsync<TKey,TModel>(
			this IDataSetEntityManager<TModel> Storage,
			Func<TKey, Task> Remove,
			Expression<Func<TModel,bool>> Condition=null,
			int BatchCount=100
			)
			where TKey : IEquatable<TKey>
			where TModel : class,IEntityWithId<TKey>
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
					var ids = await q.Select(i => i.Id).Take(BatchCount).ToArrayAsync();
					if (ids.Length == 0)
						break;
					foreach (var id in ids)
						await Remove(id);
					await scope.Commit();
					if (ids.Length < BatchCount)
						break;
				}
			}
		}

		public static Task RemoveAllScoppedAsync<TKey, TModel>(
			this IDataSetEntityManager<TModel> Storage,
			long ScopeId,
			Func<TKey, Task> Remove,
			int BatchCount = 100
			)
			where TKey : IEquatable<TKey>
			where TModel : class, IEntityWithId<TKey>, IEntityWithScope
			=> Storage.RemoveAllAsync(Remove, m => m.ScopeId == ScopeId, BatchCount);
 
	}
	public interface IReadOnlyEntityHelper<TKey, TTempReadOnlyEntity, TReadOnlyEntity, TQueryArgument, TModel>
		where TKey : IEquatable<TKey>
		where TQueryArgument : QueryArgument<TKey>
		where TModel : class
	{
		Task<TReadOnlyEntity> GetAsync(
			TKey Key,
			Func<IContextQueryable<TModel>, IContextQueryable<TTempReadOnlyEntity>> MapModelToReadOnly,
			Func<TTempReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
		);

		Task<TReadOnlyEntity[]> GetAsync(
			TKey[] Key,
			Func<IContextQueryable<TModel>, IContextQueryable<TTempReadOnlyEntity>> MapModelToReadOnly,
			Func<TTempReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
			);

		Task<QueryResult<TKey>> QueryIdentsAsync(
			TQueryArgument Arg,
			Paging paging,
			Func<IContextQueryable<TModel>, IContextQueryable<TTempReadOnlyEntity>> MapModelToReadOnly,
			Func<TTempReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
			);
		Task<QueryResult<TReadOnlyEntity>> QueryAsync(
			TQueryArgument Arg,
			Paging paging,
			Func<IContextQueryable<TModel>, IContextQueryable<TTempReadOnlyEntity>> MapModelToReadOnly,
			Func<TTempReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
			);
	}

	public interface IEntityHelper<TKey, TTempReadOnlyEntity, TReadOnlyEntity, TQueryArgument, TEditableEntity, TModel>:
		IReadOnlyEntityHelper<TKey, TTempReadOnlyEntity, TReadOnlyEntity, TQueryArgument, TModel>
		where TKey : IEquatable<TKey>
		where TQueryArgument : QueryArgument<TKey>
		where TModel:class
	{
		Task<TEditableEntity> LoadForEdit(
			TKey Id, 
			Func<IContextQueryable<TModel>, Task<TEditableEntity>> MapModelToEditable
			);

		Task<TKey> CreateAsync(
			TEditableEntity Entity,
			Func<IEntityModifyContext<TKey, TEditableEntity, TModel>, Task> UpdateModel,
			Func<IEntityModifyContext<TKey,TEditableEntity,TModel>, Task> InitModel
			);
		Task UpdateAsync(
			TEditableEntity Entity,
			Func<IEntityModifyContext<TKey, TEditableEntity, TModel>, Task<TModel>> LoadModelForEdit,
			Func<IEntityModifyContext<TKey, TEditableEntity, TModel>, Task> UpdateModel
			);

		Task RemoveAsync(
			TKey Key,
			Func<IEntityModifyContext<TKey, TEditableEntity, TModel>, Task<TModel>> LoadModelForEdit,
			Func<IEntityModifyContext<TKey, TEditableEntity, TModel>, Task> RemoveModel
			);
		Task RemoveAllAsync();
		
	}
}
