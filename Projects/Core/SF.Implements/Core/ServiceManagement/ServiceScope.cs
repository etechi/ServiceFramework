using SF.Core.ServiceManagement.Internals;
using System;
using System.Collections.Generic;
namespace SF.Core.ServiceManagement
{
	public class ServiceScope : ServiceScopeBase,
		IServiceScope
	{

		IServiceProvider RootServiceProvider { get; }
		public ServiceScope(IServiceProvider ServiceProvider, IServiceFactoryManager ServiceFactoryManager) :
			base(ServiceFactoryManager)
		{
			this.RootServiceProvider = ServiceProvider;
		}
		internal override object GetService(IServiceFactory factory,Type ServiceType)
		{
			if (factory.ServiceImplement.LifeTime == ServiceImplementLifetime.Singleton)
				return ((ServiceScopeBase)RootServiceProvider).GetService(factory, ServiceType);
			//.GetService(factory.ServiceDeclaration.ServiceType);
			return base.GetService(factory, ServiceType);
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
