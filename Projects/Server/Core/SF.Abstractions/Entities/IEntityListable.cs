using SF.Core.ServiceManagement;
using System.Threading.Tasks;

namespace SF.Entities
{
	public interface IEntityListable<TKey, TEntity>
	{
		Task<TEntity[]> ListAsync();
	}
}
