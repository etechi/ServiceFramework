using System.Threading.Tasks;

namespace SF.Data.Services
{
	public interface IEntityLoader<TKey, TEntity>
	{
		Task<TEntity> LoadAsync(TKey Id);
	}
}
