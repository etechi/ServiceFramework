using System;
using System.Collections.Generic;

using System.Linq;
using System.Collections;
using SF.Core.ServiceManagement.Internals;

namespace SF.Core.ServiceManagement
{
	public class ServiceProvider : ServiceScopeBase
	{
		public ServiceProvider(IServiceFactoryManager FactoryManager):
			base(FactoryManager)
		{
		}
		protected override CacheType GetCacheType(IServiceFactory Factory)
		{
			switch (Factory.ServiceImplement.LifeTime)
			{
				case ServiceImplementLifetime.Singleton:
				case ServiceImplementLifetime.Scoped:
					return CacheType.CacheScoped;
				case ServiceImplementLifetime.Transient:
					return CacheType.NoCache;
				default:
					throw new NotSupportedException();
			}
		}
		internal override object GetService(IServiceFactory factory, Type ServiceType, IServiceResolver ServiceResolver)
		{
			lock (this)
			{
				return base.GetService(factory, ServiceType, ServiceResolver);
			}
		}
	}

}
