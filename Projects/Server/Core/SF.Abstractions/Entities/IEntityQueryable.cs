using SF.Core.ServiceManagement;
using System.Threading.Tasks;

namespace SF.Entities
{
	public interface IEntityIdentQueryable<TKey,TQueryArgument>
	{
		Task<QueryResult<TKey>> QueryIdentsAsync(TQueryArgument Arg, Paging paging);
	}
	public interface IEntityQueryable<TEntity, TQueryArgument>
	{
		Task<QueryResult<TEntity>> QueryAsync(TQueryArgument Arg,Paging paging);
	}
}
