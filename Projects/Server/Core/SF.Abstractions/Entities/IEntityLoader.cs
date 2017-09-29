using SF.Auth;
using SF.Core.ServiceManagement;
using SF.Metadata;
using System.Threading.Tasks;

namespace SF.Entities
{

	public interface IEntityLoadable<TEntity>: IAbstractEntityManager<TEntity>
	{
		[Comment("通过主键获取对象")]
		Task<TEntity> GetAsync(TEntity Key);
	}
}
