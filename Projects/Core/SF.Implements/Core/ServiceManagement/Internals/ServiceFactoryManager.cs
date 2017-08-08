using System;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SF.Core.ServiceManagement.Internals
{
	
	
	class ServiceFactoryManager : IServiceFactoryManager,IServiceInstanceConfigChangedNotifier
	{
		//服务实例创建缓存
		ServiceCreatorCache ServiceCreatorCache { get; }

		public IServiceMetadata ServiceMetadata { get; }

		ManagedServiceFactoryManager ManagedServiceFactoryManager { get; }
		UnmanagedServiceFactoryManager UnmanagedServiceFactoryManager { get; }

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
					var entry = UnmanagedServiceFactoryManager.GetUnmanagedServiceEntry(cacheType, true);
					var factory = entry.Factory.Value;
					var cache = (Caching.ILocalCache<IServiceEntry>)factory.Create(sp);
					return cache;
				}
				);
			UnmanagedServiceFactoryManager = new UnmanagedServiceFactoryManager(
				ServiceMetadata,
				ServiceCreatorCache
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
				return UnmanagedServiceFactoryManager.GetUnmanagedServiceEntry(ServiceType, true)?.Factory.Value;
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
				var es = UnmanagedServiceFactoryManager.GetUnmanagedServiceEntries(ServiceType, true);
				if (es != null)
					return es.Select(e => e.Factory.Value);
			}
			return Enumerable.Empty<IServiceFactory>();
		}

		void IServiceInstanceConfigChangedNotifier.NotifyChanged( long Id)
		{
			ManagedServiceFactoryManager.NotifyChanged(Id);
		}

		void IServiceInstanceConfigChangedNotifier.NotifyInternalServiceChanged(long? ScopeId, string ServiceType)
		{
			ManagedServiceFactoryManager.NotifyInternalServiceChanged(ScopeId, ServiceType);
		}

	}

}
