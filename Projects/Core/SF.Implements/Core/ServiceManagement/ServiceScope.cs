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
		public override object Resolve(int AppId, Type ServiceType, string ServiceInstanceId,Type InterfaceType)
		{
			if (InterfaceType == TypeServiceProvider || InterfaceType == TypeServiceResolver)
				return ServiceResolver;
			return base.Resolve(AppId, ServiceType, ServiceInstanceId, InterfaceType);
		}
		protected override CacheType GetCacheType(IServiceInterfaceFactory Factory)
		{
			switch (Factory.ServiceInterface.LifeTime)
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
