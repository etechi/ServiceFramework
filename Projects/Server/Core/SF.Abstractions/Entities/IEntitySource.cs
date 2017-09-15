using SF.Core.ServiceManagement;
using System;
namespace SF.Entities
{
	public interface IEntitySource<TKey, TEntity> :
		IEntityLoadable<TKey, TEntity>,
		IEntityBatchLoadable<TKey, TEntity>,
		IEntityQueryable<TKey, TEntity, QueryArgument<TKey>>
		where TEntity : class, IEntityWithId<TKey>
		where TKey : IEquatable<TKey>
	{
	}

	public interface IEntitySource<TKey, TEntity, TQueryArgument> :
		IEntityLoadable<TKey, TEntity>,
		IEntityBatchLoadable<TKey, TEntity>,
		IEntityQueryable<TKey, TEntity, TQueryArgument>
		where TQueryArgument: class,IQueryArgument<TKey>
		where TEntity: class,IEntityWithId<TKey>
		where TKey:IEquatable<TKey>
	{
	}
}
