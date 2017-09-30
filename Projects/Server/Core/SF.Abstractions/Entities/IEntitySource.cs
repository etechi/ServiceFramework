using SF.Core.ServiceManagement;
using System;
namespace SF.Entities
{
	
	public interface IEntitySource<TKey,TEntitySummary,TEntityDetail, TQueryArgument> :
		IEntityLoadable<TKey, TEntityDetail>,
		IEntityBatchLoadable<TKey, TEntityDetail>,
		IEntityQueryable<TEntitySummary, TQueryArgument>,
		IEntityIdentQueryable<TKey, TQueryArgument>
	{
	}
	public interface IEntitySource<TKey, TEntity, TQueryArgument> :
		IEntitySource<TKey, TEntity, TEntity, TQueryArgument>
	{
	}
	public interface IEntitySource<TKey, TEntity> :
		IEntitySource<TKey, TEntity,QueryArgument>
	{
	}

}
