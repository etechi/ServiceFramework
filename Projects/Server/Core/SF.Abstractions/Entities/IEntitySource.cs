using SF.Core.ServiceManagement;
using System;
namespace SF.Entities
{
	
	public interface IEntitySource<TKey, TEntitySummary,TEntityDetail, TQueryArgument> :
		IEntityLoadable<TKey, TEntityDetail>,
		IEntityBatchLoadable<TKey, TEntityDetail>,
		IEntityQueryable<TKey, TEntitySummary, TQueryArgument>
		where TQueryArgument: class,IQueryArgument<TKey>
		where TEntitySummary: class,IEntityWithId<TKey>
		where TEntityDetail : class, IEntityWithId<TKey>
		where TKey:IEquatable<TKey>
	{
	}
	public interface IEntitySource<TKey, TEntity, TQueryArgument> :
		IEntitySource<TKey, TEntity, TEntity, TQueryArgument>
		where TQueryArgument : class, IQueryArgument<TKey>
		where TEntity : class, IEntityWithId<TKey>
		where TKey : IEquatable<TKey>
	{
	}
	public interface IEntitySource<TKey, TEntity> :
		IEntitySource<TKey,TEntity,QueryArgument<TKey>>
		where TEntity : class, IEntityWithId<TKey>
		where TKey : IEquatable<TKey>
	{
	}

}
