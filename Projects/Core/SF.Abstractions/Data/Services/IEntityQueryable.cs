using System.Threading.Tasks;

namespace SF.Data.Services
{
	public interface IEntityQueryable<TKey, TEntity, TQueryArgument>
		where TQueryArgument:IQueryArgument<TKey>
	{
		Task<QueryResult<TEntity>> Query(TQueryArgument Arg,Paging paging);
	}
}
