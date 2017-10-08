using SF.Core.Events;
using System;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement.Internals
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
					var types=ManagedServiceFactoryManager.TryRemoveEntry(sic.Id);
					if (types != null)
						_ServiceRemovable.RemoveService(sic.Id, types);

					return Task.CompletedTask;
				});

			OnInternalServiceChanged.Wait(
				isc =>
				{
					ManagedServiceFactoryManager.TryRemoveInternalEntries(isc.ScopeId, isc.ServiceType);
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
