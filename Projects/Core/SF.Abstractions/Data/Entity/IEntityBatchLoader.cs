using SF.Core.ServiceManagement;
using System.Threading.Tasks;

namespace SF.Data.Entity
{
	public interface IEntityBatchLoader<TKey, TEntity>
	{
		Task<TEntity[]> GetAsync(TKey[] Ids);
	}
}
