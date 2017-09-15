using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using ServiceProtocol.Data.Entity;
using SF.Data.Entity;

namespace SF.Data.Services
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
		protected abstract IContextQueryable<TTemp> MapModelToPublic(IContextQueryable<TModel> Query);
		protected abstract Task<TPublic[]> OnPreparePublics(TTemp[] Internals);
		public virtual string ResourceType => ResourceTypeCache.GetResourceType(GetType());

		public async Task<TPublic[]> LoadAsync(TKey[] Ids)
		{
			var re = await MapModelToPublic(
				DataSet.AsQueryable(true).Where(s => Ids.Contains(s.Id))
				).ToArrayAsync();

			if (re == null)
				return null;

			var res = await OnPreparePublics(re);
			return res;
		}

		public async Task<TPublic> LoadAsync(TKey Id)
		{
			var re = await MapModelToPublic(
				DataSet.AsQueryable(true).Where(s => s.Id.Equals(Id))
				).SingleOrDefaultAsync();

			if (re == null)
				return null;

			var res = await OnPreparePublics(new[] { re });

			return res[0];
		}
	}
	
    
   
}