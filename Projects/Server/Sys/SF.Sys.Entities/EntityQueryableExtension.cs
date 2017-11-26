﻿#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using SF.Sys.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace SF.Sys.Entities
{
	public static class EntityQueryableExtension
	{
		public static IContextQueryable<T> WithScope<T>(this IContextQueryable<T> q, IServiceInstanceDescriptor ServiceInstanceDescriptor)
			where T : IEntityWithScope
		{
			var sid = ServiceInstanceDescriptor?.DataScopeId;
			return q.Where(i => i.ServiceDataScopeId == sid);
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


		public static async Task<IEnumerable<TEntity>> QueryAllAsync<TEntity, TQueryArgument>(
			this IEntityQueryable<TEntity,TQueryArgument> Queryable
			)
			where TEntity:class
			where TQueryArgument :  new()
		{
			var re = await Queryable.QueryAsync(new TQueryArgument(), Paging.All);
			return re.Items;
		}
		public static async Task<TEntity> QuerySingleAsync<TEntity, TQueryArgument>(
			this IEntityQueryable<TEntity, TQueryArgument> Queryable,
			TQueryArgument Arg
			)
			where TQueryArgument : new()
		{
			var re = await Queryable.QueryAsync(Arg, Paging.Single);
			return re.Items.FirstOrDefault();
		}

	}
}