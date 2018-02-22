#region Apache License Version 2.0
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

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SF.Sys.Logging;
using SF.Sys.Data;
using SF.Sys.Linq;
using SF.Sys.Collections.Generic;
using SF.Sys.Caching;
using System.Collections.Concurrent;
using SF.Sys.Entities;

namespace SF.Sys.Services
{
	

	public static class EntityCacheExtension
	{
		public class EntityLocalCache<TKey, TItem, LoadServices> : 
			IEntityCache<TKey,TItem>,
			IEntityCacheRemover<TKey> 
			where TItem:class
		{
			ILocalCache<TItem> Cache { get; }
			Func<LoadServices, TKey, Task<TItem>> Loader { get; }
			IServiceProvider ServiceProvider { get; }

			public EntityLocalCache(
				IServiceProvider sp,
				Func<LoadServices,TKey, Task<TItem>> Loader
				)
			{
				this.Loader = Loader;
				this.Cache = sp.Resolve<ILocalCache<TItem>>();
				this.ServiceProvider = sp;
			}
			public Task Remove(TKey Key)
			{
				Cache.Remove(Key.ToString());
				return Task.CompletedTask;
			}
			public async Task<TItem> Find(TKey Id)
			{
				var key = Id.ToString();
				var plan = Cache.Get(key);
				if (plan != null) return plan;
				return await ServiceProvider.WithScopedServices(async (LoadServices services) =>
				{
					var re = await Loader(services,Id);
					if (re == null) return null;
					return Cache.AddOrGetExisting(
						key, re, TimeSpan.FromHours(1)
						) ?? re;
				});
			}
		}
		public static IServiceCollection AddEntityLocalCache<TKey, TItem, TLoadServices, TInitRemoveServices>(
			this IServiceCollection sc,
			Func<TLoadServices, TKey, Task<TItem>> Loader,
			Action<TInitRemoveServices, IEntityCacheRemover<TKey>> InitRemover
			) where TItem : class
		{
			sc.AddSingleton<IEntityCache<TKey, TItem>>(sp =>
			{
				var cache = new EntityLocalCache<TKey, TItem, TLoadServices>(
					sp,
					Loader
					);
				sp.WithServices((TInitRemoveServices svc) =>
				{
					InitRemover(svc, cache);
					return 0;
				});
				return cache;
			});
			return sc;
		}
		public class EntityGlobalCache<TKey, TItem, LoadServices> :
			IEntityCache<TKey, TItem>,
			IEntityCacheRemover<TKey>
			where TItem : class
			where TKey:IEquatable<TKey>
		{
			ConcurrentDictionary<TKey, TItem> Cache { get; } = new ConcurrentDictionary<TKey, TItem>();
			Func<LoadServices, TKey, Task<TItem>> Loader { get; }
			IServiceProvider ServiceProvider { get; }

			public EntityGlobalCache(
				IServiceProvider sp,
				Func<LoadServices, TKey, Task<TItem>> Loader
				)
			{
				this.Loader = Loader;
				this.ServiceProvider = sp;
			}
			public Task Remove(TKey Key)
			{
				Cache.TryRemove(Key,out var re);
				return Task.CompletedTask;
			}
			public async Task<TItem> Find(TKey Id)
			{
				if (Cache.TryGetValue(Id, out var re))
					return re;
				return await ServiceProvider.WithScopedServices(async (LoadServices services) =>
				{
					var item = await Loader(services, Id);
					if (item == null) return null;
					return Cache.GetOrAdd(Id, item);
				});
			}
		}
		public static IServiceCollection AddEntityGlobalCache<TKey, TItem, TLoadServices, TInitRemoveServices>(
			this IServiceCollection sc,
			Func<TLoadServices, TKey, Task<TItem>> Loader,
			Action<IServiceProvider,TInitRemoveServices, IEntityCacheRemover<TKey>> InitRemover
			) where TItem : class
			where TKey:IEquatable<TKey>
			where TInitRemoveServices:class
		{
			sc.AddSingleton<IEntityCache<TKey, TItem>>(sp =>
			{
				var cache = new EntityGlobalCache<TKey, TItem, TLoadServices>(
					sp,
					Loader
					);
				sp.WithScope((IServiceProvider isp) =>
				{
					var svc = isp.Resolve<TInitRemoveServices>();
					InitRemover(isp, svc, cache);
					return 0;
				});
				return cache;
			});
			return sc;
		}

	}
	



}