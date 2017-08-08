using SF.Core.ServiceManagement;
using System.Threading.Tasks;

namespace SF.Data.Entity
{
	public interface IEntityIdentQueryable<TKey, TQueryArgument>
	{
		Task<QueryResult<TKey>> QueryIdentsAsync(TQueryArgument Arg, Paging paging);
	}
	public interface IEntityQueryable<TKey, TEntity, TQueryArgument>:
		IEntityIdentQueryable<TKey,TQueryArgument>
		where TQueryArgument:IQueryArgument<TKey>
	{
		Task<QueryResult<TEntity>> QueryAsync(TQueryArgument Arg,Paging paging);
	}
}
