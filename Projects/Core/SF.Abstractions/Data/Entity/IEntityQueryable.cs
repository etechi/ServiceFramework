using SF.Core.ServiceManagement;
using System.Threading.Tasks;

namespace SF.Data.Entity
{
	public interface IEntityQueryable<TKey, TEntity, TQueryArgument>
		where TQueryArgument:IQueryArgument<TKey>
	{
		Task<QueryResult<TKey>> QueryIdentsAsync(TQueryArgument Arg, Paging paging);
		Task<QueryResult<TEntity>> QueryAsync(TQueryArgument Arg,Paging paging);
	}
}
