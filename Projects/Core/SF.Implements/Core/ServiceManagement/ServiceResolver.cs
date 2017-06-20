using System;
using System.Collections.Generic;

using System.Linq;
using System.Collections;
using SF.Core.ServiceManagement.Internals;

namespace SF.Core.ServiceManagement
{
	public class ServiceResolver : ServiceScopeBase
	{
		public ServiceResolver(IServiceFactoryManager FactoryManager):
			base(FactoryManager)
		{
		}
		protected override CacheType GetCacheType(IServiceInterfaceFactory Factory)
		{
			switch (Factory.ServiceInterface.LifeTime)
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
	}

}
