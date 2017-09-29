using SF.Core.ServiceManagement;
using System;
namespace SF.Entities
{
	
	public interface IEntitySource<TEntitySummary,TEntityDetail, TQueryArgument> :
		IEntityLoadable<TEntityDetail>,
		IEntityBatchLoadable<TEntityDetail>,
		IEntityQueryable<TEntitySummary, TQueryArgument>
		where TQueryArgument: class
		where TEntitySummary: class
		where TEntityDetail : class
	{
	}
	public interface IEntitySource<TEntity, TQueryArgument> :
		IEntitySource<TEntity, TEntity, TQueryArgument>
		where TQueryArgument : class
		where TEntity : class
	{
	}
	public interface IEntitySource<TEntity> :
		IEntitySource<TEntity,QueryArgument>
		where TEntity : class
	{
	}

}
