using SF.Auth;
using SF.Core.ServiceManagement;
using SF.Metadata;
using System.Threading.Tasks;

namespace SF.Entities
{
	
	public interface IEntityInstanceUpdater<TEntity> : IAbstractEntityManager<TEntity>
	{
		Task<TEntity> Load();
		Task Save(TEntity value);
	}
}
