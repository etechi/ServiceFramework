using SF.Auth;
using SF.Core.ServiceManagement;
using SF.Metadata;
using System.Threading.Tasks;

namespace SF.Entities
{
	public interface IEntityLoadableByKey<TEntity>
	{
		[Comment("通过快速访问键值获取对象")]
		Task<TEntity> GetByKeyAsync(string Key);
	}
}
