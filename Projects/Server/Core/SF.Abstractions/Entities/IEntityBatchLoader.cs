using SF.Core.ServiceManagement;
using System.Threading.Tasks;

namespace SF.Entities
{
	public interface IEntityBatchLoadable<TEntity>
	{
		Task<TEntity[]> GetAsync(TEntity[] Ids);
	}
}
