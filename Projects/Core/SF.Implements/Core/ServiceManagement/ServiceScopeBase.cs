using SF.Core.ServiceManagement.Internals;
using System;
using System.Collections.Generic;
namespace SF.Core.ServiceManagement
{
	public abstract class ServiceScopeBase : 
		IDisposable,
		IServiceResolver
	{
		IServiceFactoryManager FactoryManager { get; }
		public IServiceResolver ServiceResolver => this;
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

		void TryAddCache((Type,long) key,object CurEntry,object Service)
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


		public virtual object Resolve(Type ServiceType, long ServiceInstanceId)
		{
			var factory = FactoryManager.GetServiceFactory(
				ServiceResolver, 
				ServiceType, 
				ServiceInstanceId
				);
			if (factory == null)
				return null;

			var cacheType = GetCacheType(factory);

			if (cacheType==CacheType.NoCache)
				return factory.Create(this);

			var key = (ServiceType, ServiceInstanceId);

			object curEntiy=null;
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

		public virtual void Dispose()
		{
			if(_Services != null)
				foreach (var v in _Services.Values)
				{
					var p = v as IDisposable;
					if (p != null)
					{
						try{
							p.Dispose();
						}
						catch
						{

						}
					}
				}
		}

		public object GetService(Type serviceType)
		{
			return Resolve(serviceType, 0);
		}
	}

}
