using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Data;
using SF.Core.ServiceManagement;
namespace SF.Entities
{
	public static class EntityQueryableExtension
	{
		public static IContextQueryable<T> WithScope<T>(this IContextQueryable<T> q, IServiceInstanceDescriptor ServiceInstanceDescriptor)
			where T : IEntityWithScope
		{
			var sid = ServiceInstanceDescriptor?.InstanceId;
			return q.Where(i => i.ScopeId == sid);
		}

		
		public static IContextQueryable<T> IsEnabled<T>(this IContextQueryable<T> q)
			where T : IEntityWithLogicState
		{
			return q.Where(i => i.LogicState == EntityLogicState.Enabled);
		}
		public static IContextQueryable<T> WithId<T,TKey>(this IContextQueryable<T> q,TKey Id)
			where T : IEntityWithId<TKey>
			where TKey:IEquatable<TKey>
		{
			return q.Where(i => i.Id.Equals(Id));
		}
		public static IContextQueryable<T> EnabledScopedById<T, TKey>(
			this IContextQueryable<T> q, 
			TKey Id, 
			IServiceInstanceDescriptor ServiceInstanceDescriptor
			)
			where T : IEntityWithId<TKey>, IEntityWithLogicState, IEntityWithScope
			where TKey : IEquatable<TKey>
			=> q.WithId(Id).WithScope(ServiceInstanceDescriptor).IsEnabled();


		public static async Task<IEnumerable<TEntity>> QueryAllAsync<TKey,TEntity, TQueryArgument>(
			this IEntityQueryable<TEntity,TQueryArgument> Queryable
			)
			where TKey:IEquatable<TKey>
			where TEntity:class,IEntityWithId<TKey>
			where TQueryArgument : IQueryArgument<TKey>, new()
		{
			var re = await Queryable.QueryAsync(new TQueryArgument(), Paging.All);
			return re.Items;
		}
		public static async Task<TEntity> QuerySingleAsync<TKey, TEntity, TQueryArgument>(
			this IEntityQueryable<TEntity, TQueryArgument> Queryable,
			TQueryArgument Arg
			)
			where TKey : IEquatable<TKey>
			where TEntity : class, IEntityWithId<TKey>
			where TQueryArgument : IQueryArgument<TKey>, new()
		{
			var re = await Queryable.QueryAsync(Arg, Paging.Single);
			return re.Items.FirstOrDefault();
		}

	}
}