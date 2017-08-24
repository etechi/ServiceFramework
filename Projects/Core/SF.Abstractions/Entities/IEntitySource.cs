using SF.Core.ServiceManagement;
using System;
namespace SF.Entities
{

	public interface IEntitySource<TKey, TEntity, TQueryArgument> :
		IEntityLoadable<TKey, TEntity>,
		IEntityBatchLoader<TKey, TEntity>,
		IEntityQueryable<TKey, TEntity, TQueryArgument>
		where TQueryArgument: class,IQueryArgument<TKey>
		where TEntity: class,IEntityWithId<TKey>
		where TKey:IEquatable<TKey>
	{
	}
}
