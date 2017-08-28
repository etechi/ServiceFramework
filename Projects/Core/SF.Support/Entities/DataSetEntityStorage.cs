using System.Threading.Tasks;
using System;
using SF.Core.ServiceManagement;
using System.Linq;
using SF.Data;

namespace SF.Entities
{
	
	public static class DataSetEntityStorage
	{
		public static async Task<T> UseTransaction<TModel,T>(
			this IDataSetEntityStorage<TModel> Storage,
			Func<Task<T>> Action)
			where TModel : class
		{
			var ctx = Storage.DataSet.Context;
			var tm = ctx.TransactionScopeManager;
			var tran = tm.CurrentDbTransaction;
			if (tran == null)
				return await Action();
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

		#region GetAsync

		public static async Task<TReadOnlyEntity> GetAsync<TKey, TTempReadOnlyEntity, TReadOnlyEntity, TModel>
		(
			this IDataSetEntityStorage<TModel> Storage,
			TKey Id,
			Func<IContextQueryable<TModel>, IContextQueryable<TTempReadOnlyEntity>> MapModelToReadOnly,
			Func<TTempReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
		)
			where TKey : IEquatable<TKey>
			where TModel : class, IEntityWithId<TKey>
			where TReadOnlyEntity : class
		{
			return await Storage.UseTransaction(async () =>
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
			this IDataSetEntityStorage<TModel> Storage,
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
			this IDataSetEntityStorage<TModel> Storage,
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
			this IDataSetEntityStorage<TModel> Storage,
			TKey[] Ids,
			Func<IContextQueryable<TModel>, IContextQueryable<TTempReadOnlyEntity>> MapModelToReadOnly,
			Func<TTempReadOnlyEntity[], Task<TReadOnlyEntity[]>> PrepareReadOnly
		)
			where TKey : IEquatable<TKey>
			where TModel : class, IEntityWithId<TKey>
			where TReadOnlyEntity : class
		{
			return await Storage.UseTransaction(async () =>
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
			this IDataSetEntityStorage<TModel> Storage,
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
			this IDataSetEntityStorage<TModel> Storage,
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
			this IDataSetEntityStorage<TModel> Storage,
			TQueryArgument Arg,
			Paging paging,
			Func<IContextQueryable<TModel>, TQueryArgument, Paging, IContextQueryable<TModel>> BuildQuery,
			PagingQueryBuilder<TModel> PagingQueryBuilder
			)
			where TKey : IEquatable<TKey>
			where TModel : class, IEntityWithId<TKey>
			where TQueryArgument : IQueryArgument<TKey>
		{
			return await Storage.UseTransaction(async () =>
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
			this IDataSetEntityStorage<TModel> Storage,
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
			this IDataSetEntityStorage<TModel> Storage,
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
			return await Storage.UseTransaction(async () =>
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
			this IDataSetEntityStorage<TModel> Storage,
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
			this IDataSetEntityStorage<TModel> Storage,
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
			this IDataSetEntityStorage<TModel> Storage,
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
			this IDataSetEntityStorage<TModel> Storage,
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
			this IDataSetEntityStorage<TModel> Storage,
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
			this IDataSetEntityStorage<TModel> Storage,
			TKey Id,
			Func<IContextQueryable<TModel>, Task<TEditableEntity>> MapModelToEditable
			)
			where TKey:IEquatable<TKey>
			where TModel:class,IEntityWithId<TKey>
		{
			return await Storage.UseTransaction(async () =>
			{
				return await MapModelToEditable(Storage.DataSet.AsQueryable(false).Where(m => m.Id.Equals(Id)));
			});
		}

		public static Task<TEditableEntity> LoadForEdit<TEditableEntity, TModel>(
			this IDataSetEntityStorage<TModel> Storage,
			long Id,
			Func<IContextQueryable<TModel>, Task<TEditableEntity>> MapModelToEditable
			)	where TModel:class,IEntityWithId<long>
			=> Storage.LoadForEdit<long, TEditableEntity, TModel>(Id, MapModelToEditable);
		#endregion



		#region Create

		public static async Task<TKey> CreateAsync<TKey,TEditableEntity,TModel>(
			this IDataSetEntityStorage<TModel> Storage,
			TEditableEntity Entity,
			Func<IEntityModifyContext<TKey, TEditableEntity, TModel>, Task> UpdateModel,
			Func<IEntityModifyContext<TKey, TEditableEntity, TModel>, Task> InitModel,
			object ExtraArgument=null
			) 
			where TKey:IEquatable<TKey>
			where TModel:class,IEntityWithId<TKey>,new()
			where TEditableEntity:class,IEntityWithId<TKey>
		{
			return await Storage.UseTransaction(async () =>
			{
				var ctx = Storage.NewContext<TKey,TEditableEntity>(ModifyAction.Create, Entity, ExtraArgument);
				ctx.Model = new TModel();
				await InitModel(ctx);
				await UpdateModel(ctx);
				Storage.DataSet.Add(ctx.Model);
				await Storage.DataSet.Context.SaveChangesAsync();
				return ctx.Model.Id;
			});

		}
		public static Task<long> CreateAsync<TEditableEntity, TModel>(
			this IDataSetEntityStorage<TModel> Storage,
			TEditableEntity Entity,
			Func<IEntityModifyContext<long, TEditableEntity, TModel>, Task> UpdateModel,
			Func<IEntityModifyContext<long, TEditableEntity, TModel>, Task> InitModel,
			object ExtraArgument = null
			)
			where TModel : class, IEntityWithId<long>, new()
			where TEditableEntity : class, IEntityWithId<long>
			=> Storage.CreateAsync<long,TEditableEntity,TModel>(Entity, UpdateModel, InitModel);

		#endregion

		#region Update
		public static Task UpdateAsync<TKey, TEditableEntity,TModel>(
			this IDataSetEntityStorage<TModel> Storage,
			Func<IEntityModifyContext<TKey, TEditableEntity, TModel>, Task<TModel>> LoadModelForEdit,
			Func<IEntityModifyContext<TKey, TEditableEntity, TModel>, Task> UpdateModel
			)
			where TKey:IEquatable<TKey>
			where TEditableEntity:class,IEntityWithId<TKey>
		{

		}

		#endregion

		Task RemoveAsync(
			TKey Key,
			Func<IEntityModifyContext<TKey, TEditableEntity, TModel>, Task<TModel>> LoadModelForEdit,
			Func<IEntityModifyContext<TKey, TEditableEntity, TModel>, Task> RemoveModel
			);
		Task RemoveAllAsync();

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
