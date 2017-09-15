using SF.Auth;
using SF.Core.ServiceManagement;
using SF.Metadata;
using System.Threading.Tasks;

namespace SF.Entities
{
	public interface IEntitySearchable<TEntity>
	{
		[Comment("通过关键字搜索对象")]
		Task<QueryResult<TEntity>> SearchAsync(string Key,Paging Paging);
	}
}
