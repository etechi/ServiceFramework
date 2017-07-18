using SF.Core.ServiceManagement.Internals;
using System;
using System.Collections.Generic;
namespace SF.Core.ServiceManagement
{
	public abstract class ServiceScopeBase :
		IDisposable,
		IServiceProvider,
		IServiceResolver
	{
		IServiceFactoryManager FactoryManager { get; }
		public IServiceProvider ServiceProvider => this;
		Dictionary<(Type, long), object> _Services;

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
		internal virtual object GetService(IServiceFactory factory)
		{
			if (factory == null)
				return null;
			var cacheType = GetCacheType(factory);

			if (cacheType == CacheType.NoCache)
				return factory.Create(this);


			var key = (factory.ServiceImplement.ServiceType, factory.InstanceId??0);
			object curEntiy = null;
			if (_Services == null)
				_Services = new Dictionary<(Type, long), object>();
			else if (_Services.TryGetValue(key, out curEntiy))
			{
				if (cacheType == CacheType.CacheScoped)
					return curEntiy;
			}

			var service = factory.Create(this);

			if (cacheType == CacheType.CacheScoped)
				TryAddCache(key, curEntiy, service);
			else
				TryAddCache(key, curEntiy, service as IDisposable);
			return service;
		}
		
		public virtual object GetService(Type serviceType)
		{
			return ResolveServiceByType(null, serviceType,null);
		}

		public IServiceInstanceDescriptor ResolveDescriptorByIdent(long ServiceId, Type ServiceType)
		{
			return FactoryManager.GetServiceFactoryByIdent(
				this,
				ServiceId,
				ServiceType
				);
		}
		public IServiceInstanceDescriptor ResolveDescriptorByType(long? ScopeServiceId, Type ServiceType,string Name)
		{
			return FactoryManager.GetServiceFactoryByType(
				this,
				ScopeServiceId,
				ServiceType,
				Name
				);
		}
		static Type ServiceProviderType { get; } = typeof(IServiceProvider);
		static Type ServiceResolverType { get; } = typeof(IServiceResolver);
		public object ResolveServiceByType(long? ScopeServiceId, Type ServiceType,string Name)
		{
			if (ServiceType == ServiceResolverType || ServiceType==ServiceProviderType)
				return this;
			var f = FactoryManager.GetServiceFactoryByType(
					this,
					ScopeServiceId,
					ServiceType,
					Name
					);
			return f==null?null:GetService(f);
		}
		public object ResolveServiceByIdent(long ServiceId, Type ServiceType)
		{
			var f = FactoryManager.GetServiceFactoryByIdent(
					this,
					ServiceId,
					ServiceType
					);
			return f==null?null:GetService(f);
		}

		class ScopedServiceProvider : IServiceProvider
		{
			public long ScopeServiceId { get; set; }
			public ServiceScopeBase ScopeBase { get; set; }
			public object GetService(Type serviceType)
			{
				if(serviceType== ServiceProviderType || serviceType == ServiceResolverType)
					return ScopeBase;
				return ScopeBase.ResolveServiceByType(ScopeServiceId, serviceType,null);
			}
		}
		public IServiceProvider CreateInternalServiceProvider(long ServiceId)
		{
			return new ScopedServiceProvider
			{
				ScopeBase = this,
				ScopeServiceId = ServiceId
			};
		}

		public IEnumerable<IServiceInstanceDescriptor> ResolveServiceDescriptors(
			long? ScopeServiceId, 
			Type ChildServiceType,
			string Name
			)
		{
			return FactoryManager.GetServiceFactoriesByType(
					this,
					ScopeServiceId,
					ChildServiceType,
					Name
					);
		}

		public IEnumerable<object> ResolveServices(long? ScopeServiceId, Type ChildServiceType,string Name)
		{
			foreach(var factory in FactoryManager.GetServiceFactoriesByType(
					this,
					ScopeServiceId,
					ChildServiceType,
					Name
					))
			{
				yield return GetService(factory);
			}
		}
	}

}
