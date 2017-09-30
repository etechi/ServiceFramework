using SF.Core.ServiceManagement;
using System;
namespace SF.Entities
{
	
	public interface IEntitySource<TKey,TEntitySummary,TEntityDetail, TQueryArgument> :
		IEntityLoadable<TKey, TEntityDetail>,
		IEntityBatchLoadable<TKey, TEntityDetail>,
		IEntityQueryable<TEntitySummary, TQueryArgument>,
		IEntityIdentQueryable<TKey, TQueryArgument>
		where TQueryArgument : class
		where TEntitySummary: class
		where TEntityDetail : class
	{
	}
	public interface IEntitySource<TKey, TEntity, TQueryArgument> :
		IEntitySource<TKey, TEntity, TEntity, TQueryArgument>
		where TQueryArgument : class
		where TEntity : class
	{
	}
	public interface IEntitySource<TKey, TEntity> :
		IEntitySource<TKey, TEntity,QueryArgument>
		where TEntity : class
	{
	}

}
