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

		public async Task<TPublic[]> GetAsync(TKey[] Ids)
		{
			var re = await OnMapModelToPublic(
				DataSet.AsQueryable(true).Where(s => Ids.Contains(s.Id))
				).ToArrayAsync();

			if (re == null)
				return null;

			var res = await OnPreparePublics(re);
			return res;
		}

		public async Task<TPublic> GetAsync(TKey Id)
		{
			var re = await OnMapModelToPublic(
				DataSet.AsQueryable(true).Where(s => s.Id.Equals(Id))
				).SingleOrDefaultAsync();

			if (re == null)
				return null;

			var res = await OnPreparePublics(new[] { re });

			return res[0];
		}
	}
	
    
   
}