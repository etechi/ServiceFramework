using SF.Auth;
using System.Threading.Tasks;

namespace SF.Data.Entity
{
	public interface IEntityLoader<TKey, TEntity>
	{
		Task<TEntity> GetAsync(TKey Id);
	}
}
