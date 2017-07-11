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
		public long ScopeId { get; }
		IServiceFactoryManager FactoryManager { get; }
		public IServiceProvider ServiceProvider => this;
		Dictionary<(Type,long), object> _Services;

		class CachedServices : HashSet<object>
		{

		}

		public ServiceScopeBase(IServiceFactoryManager FactoryManager, long ScopeId)
		{
			this.FactoryManager = FactoryManager;
			this.ScopeId = ScopeId;
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
		object GetService(IServiceFactory factory)
		{
			if (factory == null)
				return null;
			var cacheType = GetCacheType(factory);

			if (cacheType == CacheType.NoCache)
				return factory.Create(this);


			var key = (factory.ServiceImplement.ServiceType, factory.ServiceInstanceId);
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
		public virtual object Resolve(long ServiceId)
		{
			return GetService(
				FactoryManager.GetServiceFactory(
					this,
					ServiceId
					)
				);
		}
		public virtual object GetService(Type serviceType)
		{
			return GetService(
				FactoryManager.GetServiceFactory(
					this,
					ScopeId,
					serviceType
					)
				);

		}
	}

}
