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

using SF.Sys.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Sys.Services.Internals
{


	class ServiceFactoryManager : IServiceFactoryManager
	{
		//服务实例创建缓存
		ServiceCreatorCache ServiceCreatorCache { get; }
		public IServiceMetadata ServiceMetadata { get; }
		
		ManagedServiceFactoryManager ManagedServiceFactoryManager { get; }
		UnmanagedServiceFactoryManager UnmanagedServiceFactoryManager { get; }
		IServiceRemovable _ServiceRemovable;

		public ServiceFactoryManager(
			Caching.ILocalCache<IServiceEntry> ServiceCache,
			IServiceMetadata ServiceMetadata
			)
		{
			this.ServiceMetadata = ServiceMetadata;
			this.ServiceCreatorCache = new ServiceCreatorCache(ServiceMetadata);
			ManagedServiceFactoryManager = new ManagedServiceFactoryManager(
				ServiceCache,
				ServiceMetadata,
				ServiceCreatorCache,
				sp =>
				{
					var cacheType = typeof(Caching.ILocalCache<IServiceEntry>);
					var factory = UnmanagedServiceFactoryManager.GetUnmanagedServiceFactory(cacheType, null,true);
					var cache = (Caching.ILocalCache<IServiceEntry>)factory.Create(sp);
					return cache;
				}
				);
			UnmanagedServiceFactoryManager = new UnmanagedServiceFactoryManager(
				ServiceMetadata,
				ServiceCreatorCache
				);
		}
		public void BindServiceProvider(IServiceProvider Provider)
		{
			_ServiceRemovable = Provider as IServiceRemovable;

			var OnServiceInstanceChanged = Provider.Resolve<IEventSubscriber<ServiceInstanceChanged>>();
			var OnInternalServiceChanged = Provider.Resolve<IEventSubscriber<InternalServiceChanged>>();

			OnServiceInstanceChanged.Wait(
				sic =>
				{
					var types=ManagedServiceFactoryManager.TryRemoveEntry(sic.Payload.Id);
					if (types != null)
						_ServiceRemovable.RemoveService(sic.Payload.Id, types);

					return Task.CompletedTask;
				});

			OnInternalServiceChanged.Wait(
				isc =>
				{
					ManagedServiceFactoryManager.TryRemoveInternalEntries(isc.Payload.ScopeId, isc.Payload.ServiceType);
					return Task.CompletedTask;
				});
		}

		public Type GetServiceTypeByIdent(IServiceResolver ServiceResolver, long ServiceId)
		{
			return ManagedServiceFactoryManager.GetServiceTypeByIdent(
				ServiceResolver,
				ServiceId
				);
		}
		public IServiceFactory GetServiceFactoryByIdent(
			IServiceResolver ServiceResolver,
			long ServiceId,
			Type ServiceType
			)
		{
			return ManagedServiceFactoryManager.GetServiceFactoryByIdent(
				ServiceResolver,
				ServiceId,
				ServiceType
				);
		}

		
		//static Type UnmanagedServiceAttributeType { get; } = typeof(UnmanagedServiceAttribute);

		//[ThreadStatic]
		//static bool IsUnmanagedServiceScope;

		public IServiceFactory GetServiceFactoryByType(
			IServiceResolver ServiceResolver,
			long? ScopeServiceId,
			Type ServiceType,
			string Name
			)
		{
			var svc = ServiceMetadata.FindServiceByType(ServiceType);
			if (svc == null)
				return null;

			if(svc.HasManagedServiceImplement && ManagedServiceFactoryManager.TryGetServiceFactoryByType(
					ServiceResolver, 
					ScopeServiceId, 
					ServiceType, 
					Name, 
					out var factory
					))
					return factory;

			if (svc.HasUnmanagedServiceImplement)
				return UnmanagedServiceFactoryManager.GetUnmanagedServiceFactory(ServiceType,Name, true);
			return null;

			//	}
			//	//已经是顶级区域， 直接尝试查找服务
			//	return UnmanagedServiceFactoryManager.GetUnmanagedServiceEntry(ServiceType, true)?.Factory.Value;


				//var IsUnmanagedServiceScopeRoot=false;
				//try
				//{

				//	if (!IsUnmanagedServiceScope && (ServiceType.IsDefined(UnmanagedServiceAttributeType) || !ServiceType.IsInterfaceType()))
				//	{
				//		IsUnmanagedServiceScopeRoot = true;
				//		IsUnmanagedServiceScope = true;
				//	}

				//	if (!IsUnmanagedServiceScope && 
				//		(!ServiceType.IsGenericType || ServiceType.GetGenericTypeDefinition() != typeof(IEnumerable<>))
				//		)
				//	{
				//		if (ManagedServiceFactoryManager.TryGetServiceFactoryByType(ServiceResolver, ScopeServiceId, ServiceType, Name, out var factory))
				//			return factory;
				//	}
				//	//已经是顶级区域， 直接尝试查找服务
				//	return UnmanagedServiceFactoryManager.GetUnmanagedServiceEntry(ServiceType, true)?.Factory.Value;
				//}
				//finally
				//{
				//	if (IsUnmanagedServiceScopeRoot)
				//		IsUnmanagedServiceScope = false;
				//}
		}
		public IEnumerable<IServiceFactory> GetServiceFactoriesByType(
			IServiceResolver ServiceResolver,
			long? ScopeServiceId,
			Type ServiceType,
			string Name
			)
		{
			var svc = ServiceMetadata.FindServiceByType(ServiceType);
			if (svc == null)
				return Enumerable.Empty<IServiceFactory>();

			if (svc.HasManagedServiceImplement)
			{
				if (ManagedServiceFactoryManager.TryGetServiceFactoriesByType(ServiceResolver, ScopeServiceId, ServiceType, Name, out var enumerable))
					return enumerable;
			}

			//已经是顶级区域， 直接尝试查找服务
			if (svc.HasUnmanagedServiceImplement)
			{
				return UnmanagedServiceFactoryManager.GetUnmanagedServiceFactories(ServiceType, true);
			}
			return Enumerable.Empty<IServiceFactory>();
		}

	}

}
