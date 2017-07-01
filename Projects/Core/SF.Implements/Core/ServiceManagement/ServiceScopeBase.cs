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
		Dictionary<(Type, int, long, Type), object> _Services;

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
		protected abstract CacheType GetCacheType(IServiceInterfaceFactory Factory);

		void TryAddCache((Type, int, long, Type) key,object CurEntry,object Service)
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


		public virtual object Resolve(int AppId,Type ServiceType, long ServiceInstanceId,Type InterfaceType)
		{
			var factory = FactoryManager.GetServiceFactory(
				ServiceResolver,
				AppId, 
				ServiceType, 
				ServiceInstanceId,
				InterfaceType
				);
			if (factory == null)
				return null;

			var cacheType = GetCacheType(factory);

			if (cacheType==CacheType.NoCache)
				return factory.Create(this);

			var key = (InterfaceType,AppId, ServiceInstanceId, ServiceType);

			object curEntiy=null;
			if (_Services == null)
				_Services = new Dictionary<(Type, int, long, Type), object>();
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
			return Resolve(0, serviceType, 0, serviceType);
		}
	}

}
