using SF.Core.ServiceManagement;
using System;
namespace SF.Data.Entity
{

	public interface IEntitySource<TKey, TEntity, TQueryArgument> :
		IEntityLoader<TKey, TEntity>,
		IEntityBatchLoader<TKey, TEntity>,
		IEntityQueryable<TKey, TEntity, TQueryArgument>
		where TQueryArgument: class,IQueryArgument<TKey>
		where TEntity: class,IObjectWithId<TKey>
		where TKey:IEquatable<TKey>
	{
	}
}
