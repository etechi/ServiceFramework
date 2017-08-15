using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using SF.Data.Storage;

namespace SF.Data.Entity
{
	public abstract class EntitySource<TKey, TPublic, TModel> :
		EntitySource<TKey, TPublic, TPublic, TModel>
		where TPublic : class, IObjectWithId<TKey>
		where TKey : IEquatable<TKey>
		where TModel : class, IObjectWithId<TKey>
	{
		public EntitySource(IDataSet<TModel> DataSet) : base(DataSet)
		{
		}
		protected override Task<TPublic[]> OnPreparePublics(TPublic[] Internals)
		{
			return Task.FromResult(Internals);
		}
	}
	public abstract class EntitySource<TKey, TPublic, TTemp, TModel> :
		IEntityLoader<TKey, TPublic>,
		IEntityBatchLoader<TKey, TPublic>
		where TPublic : class, IObjectWithId<TKey>
		where TKey : IEquatable<TKey>
		where TModel: class,IObjectWithId<TKey>
	{
		public IDataSet<TModel> DataSet { get; }
		public EntitySource(IDataSet<TModel> DataSet)
		{
			this.DataSet = DataSet;
		}
		protected virtual IContextQueryable<TTemp> OnMapModelToPublic(IContextQueryable<TModel> Query)
		{
			return Query.Select(EntityMapper.Map<TModel, TTemp>());
		}
		protected abstract Task<TPublic[]> OnPreparePublics(TTemp[] Internals);

		protected async Task<T> UseTransaction<T>(Func<Task<T>> Action)
		{
			var tm = DataSet.Context.TransactionScopeManager;
			var tran = tm.CurrentDbTransaction;
			if (tran == null)
				return await Action();
			var provider = DataSet.Context.Provider;
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
		public async Task<TPublic[]> GetAsync(TKey[] Ids)
		{
			return await UseTransaction(async () =>
			{
				var re = await OnMapModelToPublic(
					DataSet.AsQueryable(true).Where(s => Ids.Contains(s.Id))
					).ToArrayAsync();

				if (re == null)
					return null;

				var res = await OnPreparePublics(re);
				return res;
			});
		}

		public async Task<TPublic> GetAsync(TKey Id)
		{
			return await UseTransaction(async () =>
			{
				var re = await OnMapModelToPublic(
				DataSet.AsQueryable(true).Where(s => s.Id.Equals(Id))
				).SingleOrDefaultAsync();

				if (re == null)
					return null;

				var res = await OnPreparePublics(new[] { re });

				return res[0];
			});
		}
	}
	
    
   
}