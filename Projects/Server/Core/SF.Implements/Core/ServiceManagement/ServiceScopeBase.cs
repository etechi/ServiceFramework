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

using SF.Core.ServiceManagement.Internals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SF.Core.ServiceManagement
{
	interface IServiceRemovable
	{
		bool RemoveService(long ServiceInstanceId, Type[] ServiceTypes);
	}
	public abstract class ServiceScopeBase :
		IDisposable,
		IServiceProvider,
		IServiceRemovable
	{ 
		public IServiceFactoryManager FactoryManager { get; }
		public IServiceProvider ServiceProvider => this;

		System.Collections.Concurrent.ConcurrentDictionary<(Type, long), object> Services { get; } = new System.Collections.Concurrent.ConcurrentDictionary<(Type, long), object>();
		

		class CachedServices : HashSet<object>
		{

		}

		public ServiceScopeBase(IServiceFactoryManager FactoryManager)
		{
			this.FactoryManager = FactoryManager;
		}
		protected enum CacheType
		{
			NoCache,
			CacheDisposable,
			CacheScoped
		}
		protected abstract CacheType GetCacheType(IServiceFactory Factory);

		void TryAddCache((Type, long) key, object Service)
		{
			if (Service == null)
				return;
			if (Service == this)
				return;

			Services.AddOrUpdate(
				key,
				(k) => Service,
				(k, s) =>
				{
					if (s == Service)
						return s;
					var Services = s as CachedServices;
					if (Services == null)
						s = new CachedServices { s, Service };
					else
					{
						Services.Add(Service);
						s = Services;
					}
					return s;
				});
		}

		public virtual void Dispose()
		{
			foreach (var v in Services.Values)
			{
				var p = v as IDisposable;
				if (p != null)
				{
					try {
						p.Dispose();
					}
					catch
					{

					}
				}
			}
		}

		internal virtual object GetService(IServiceFactory factory,Type ServiceType,IServiceResolver ServiceResolver)
		{
			if (factory == null)
				return null;
			var cacheType = GetCacheType(factory);

			if (cacheType == CacheType.NoCache)
				return factory.Create(ServiceResolver);

			var key = (ServiceType, factory.InstanceId);
			object curEntiy = null;
			if (Services.TryGetValue(key, out curEntiy))
			{
				if (cacheType == CacheType.CacheScoped)
					return curEntiy;
			}

			var service = factory.Create(ServiceResolver);

			if (cacheType == CacheType.CacheScoped)
				TryAddCache(key, service);
			else
				TryAddCache(key, service as IDisposable);
			return service;
		}
		
		public virtual object GetService(Type serviceType)
		{
			var resolver= new ServiceResolver(this, FactoryManager);
			
			return serviceType == Types.ServiceResolverType ?
				resolver:
				resolver.ResolveServiceByType(null, serviceType,null);
		}
		
		public virtual bool RemoveService(long ServiceInstanceId,Type[] ServiceTypes)
		{
			foreach (var type in ServiceTypes)
			{
				if (Services.TryRemove((type,ServiceInstanceId), out var svc))
				{
					var p = svc as IDisposable;
					if (p != null)
					{
						try
						{
							p.Dispose();
						}
						catch
						{

						}
					}
					return true;
				}
			}
			
			return false;
		}
	}

}
