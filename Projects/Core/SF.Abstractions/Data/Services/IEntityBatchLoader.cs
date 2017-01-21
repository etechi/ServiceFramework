using System.Threading.Tasks;

namespace SF.Data.Services
{
	public interface IEntityBatchLoader<TKey, TEntity>
	{
		Task<TEntity[]> LoadAsync(TKey[] Ids);
	}
}
