using SF.Auth;
using SF.Core.ServiceManagement;
using SF.Metadata;
using System.Threading.Tasks;

namespace SF.Entities
{
	
	public interface IEntityInstanceUpdater<TEntity> : 
		IAbstractEntityManager<TEntity>,
		IEntityLoadable<ObjectKey<long>,TEntity>,
		IEntityEditableLoader<ObjectKey<long>,TEntity>,
		IEntityUpdator<TEntity>
	{
	}
}
