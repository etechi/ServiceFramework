using SF.Core.ServiceManagement;
using System.Threading.Tasks;

namespace SF.Entities
{
	public interface IEntityIdentQueryable<TEntity, TQueryArgument>
	{
		Task<QueryResult<TEntity>> QueryIdentsAsync(TQueryArgument Arg, Paging paging);
	}
	public interface IEntityQueryable<TEntity, TQueryArgument>:
		IEntityIdentQueryable<TEntity, TQueryArgument>
	{
		Task<QueryResult<TEntity>> QueryAsync(TQueryArgument Arg,Paging paging);
	}
}
