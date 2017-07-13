using SF.Core.ServiceManagement.Internals;
using System;
using System.Collections.Generic;
namespace SF.Core.ServiceManagement
{
	public class ServiceScope : ServiceScopeBase,
		IServiceScope
	{

		public ServiceScope(IServiceFactoryManager FactoryManager):base(FactoryManager)
		{
		}
		static Type TypeServiceProvider = typeof(IServiceProvider);
		static Type TypeServiceResolver = typeof(IServiceResolver);
		public override object GetService(Type serviceType)
		{
			if (serviceType == TypeServiceProvider || serviceType == TypeServiceResolver)
				return this;
			return base.GetService(serviceType);
		}
		
		protected override CacheType GetCacheType(IServiceFactory Factory)
		{
			switch (Factory.ServiceImplement.LifeTime)
			{
				case ServiceImplementLifetime.Scoped:
					return CacheType.CacheScoped;
				case ServiceImplementLifetime.Singleton:
					return CacheType.NoCache;
				case ServiceImplementLifetime.Transient:
					return CacheType.CacheDisposable;
				default:
					throw new NotSupportedException();
			}
		}
	}

}
