using SF.Core.ServiceManagement;
using System.Threading.Tasks;

namespace SF.Entities
{
	public interface IEntityBatchLoader<TKey, TEntity>
	{
		Task<TEntity[]> GetAsync(TKey[] Ids);
	}
}
