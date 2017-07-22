using SF.Core.ServiceManagement.Internals;
using System;
using System.Collections.Generic;
namespace SF.Core.ServiceManagement
{
	
	public abstract class ServiceScopeBase :
		IDisposable,
		IServiceProvider
	{ 
		IServiceFactoryManager FactoryManager { get; }
		public IServiceProvider ServiceProvider => this;

		Dictionary<(Type, long), object> _Services;
		static Type ServiceProviderType { get; } = typeof(IServiceProvider);
		static Type ServiceResolverType { get; } = typeof(IServiceResolver);

		class ServiceResolver : HashSet<Type>, IServiceResolver,IServiceProvider
		{
			IServiceFactoryManager FactoryManager { get; }
			ServiceScopeBase ScopeBase { get; }

			public IServiceProvider Provider => this;

			public ServiceResolver(
				ServiceScopeBase ScopeBase,
				IServiceFactoryManager FactoryManager
				)
			{
				this.ScopeBase = ScopeBase;
				this.FactoryManager = FactoryManager;
			}
			void AddType(Type ServiceType)
			{
				if (!Add(ServiceType))
					throw new InvalidOperationException($"服务{ServiceType}已经在获取中");
			}
			public IServiceInstanceDescriptor ResolveDescriptorByIdent(long ServiceId, Type ServiceType)
			{
				AddType(ServiceType);
				try
				{
					return FactoryManager.GetServiceFactoryByIdent(
						this,
						ServiceId,
						ServiceType
						);
				}
				finally
				{
					Remove(ServiceType);
				}
			}
			public IServiceInstanceDescriptor ResolveDescriptorByType(long? ScopeServiceId, Type ServiceType, string Name)
			{
				AddType(ServiceType);
				try
				{
					return FactoryManager.GetServiceFactoryByType(
						this,
						ScopeServiceId,
						ServiceType,
						Name
						);
				}
				finally
				{
					Remove(ServiceType);
				}
			}
			public object ResolveServiceByType(long? ScopeServiceId, Type ServiceType, string Name)
			{
				AddType(ServiceType);
				try
				{
					var f = FactoryManager.GetServiceFactoryByType(
						this,
						ScopeServiceId,
						ServiceType,
						Name
						);
					return f == null ? null : GetService(f, ServiceType);
				}
				finally
				{
					Remove(ServiceType);
				}
			}
			public object ResolveServiceByIdent(long ServiceId, Type ServiceType)
			{
				AddType(ServiceType);
				try
				{
					var f = ScopeBase.FactoryManager.GetServiceFactoryByIdent(
						this,
						ServiceId,
						ServiceType
						);
					return f == null ? null : GetService(f, ServiceType);
				}
				finally
				{
					Remove(ServiceType);
				}
			}
			class ScopedServiceProvider : IServiceProvider
			{
				public long ScopeServiceId { get; set; }
				public ServiceResolver ServiceResolver { get; set; }
				public object GetService(Type serviceType)
				{
					if (serviceType == ServiceResolverType)
						return ServiceResolver;
					return ServiceResolver.ResolveServiceByType(ScopeServiceId, serviceType, null);
				}
			}
			public IServiceProvider CreateInternalServiceProvider(long ServiceId)
			{
				return new ScopedServiceProvider
				{
					ServiceResolver = this,
					ScopeServiceId = ServiceId
				};
			}

			public IEnumerable<IServiceInstanceDescriptor> ResolveServiceDescriptors(
				long? ScopeServiceId,
				Type ChildServiceType,
				string Name
				)
			{
				AddType(ChildServiceType);
				try
				{
					return FactoryManager.GetServiceFactoriesByType(
						this,
						ScopeServiceId,
						ChildServiceType,
						Name
						);
				}
				finally
				{
					Remove(ChildServiceType);
				}
			}

			public IEnumerable<object> ResolveServices(long? ScopeServiceId, Type ChildServiceType, string Name)
			{
				AddType(ChildServiceType);
				try
				{
					foreach (var factory in FactoryManager.GetServiceFactoriesByType(
						this,
						ScopeServiceId,
						ChildServiceType,
						Name
						))
					{
						yield return GetService(factory, ChildServiceType);
					}
				}
				finally
				{
					Remove(ChildServiceType);
				}
			}
			object GetService(IServiceFactory factory, Type ServiceType)=>
				ScopeBase.GetService(factory, ServiceType,this);

			public object GetService(Type serviceType)
			{
				if (serviceType == ServiceResolverType)
					return this;
				return ResolveServiceByType(null, serviceType, null);
			}
		}

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

		void TryAddCache((Type, long) key, object CurEntry, object Service)
		{
			if (Service == null)
				return;
			if (CurEntry == Service)
				return;
			if (CurEntry == null)
				CurEntry = Service;
			else
			{
				var Services = CurEntry as CachedServices;
				if (Services == null)
					CurEntry = new CachedServices { CurEntry, Service };
				else
				{
					Services.Add(Service);
					return;
				}
			}
			_Services[key] = CurEntry;
		}

		public virtual void Dispose()
		{
			if (_Services != null)
				foreach (var v in _Services.Values)
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
			if (_Services == null)
				_Services = new Dictionary<(Type, long), object>();
			else if (_Services.TryGetValue(key, out curEntiy))
			{
				if (cacheType == CacheType.CacheScoped)
					return curEntiy;
			}

			var service = factory.Create(ServiceResolver);

			if (cacheType == CacheType.CacheScoped)
				TryAddCache(key, curEntiy, service);
			else
				TryAddCache(key, curEntiy, service as IDisposable);
			return service;
		}
		
		public virtual object GetService(Type serviceType)
		{
			var resolver= new ServiceResolver(this, FactoryManager);
			
			return serviceType == ServiceResolverType?
				resolver:
				resolver.ResolveServiceByType(null, serviceType,null);
		}

		

		
	}

}
