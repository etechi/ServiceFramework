using SF.Auth;
using SF.Core.ServiceManagement;
using System.Threading.Tasks;

namespace SF.Data.Entity
{

	public interface IEntityLoader<TKey, TEntity>
	{
		Task<TEntity> GetAsync(TKey Id);
	}
}
