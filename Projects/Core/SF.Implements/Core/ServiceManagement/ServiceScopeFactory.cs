using SF.Core.ServiceManagement.Internals;
using System;
using System.Collections.Generic;
namespace SF.Core.ServiceManagement
{
	public class ServiceScopeFactory : IServiceScopeFactory
	{
		IServiceFactoryManager ServiceFactoryManager { get; }
		public ServiceScopeFactory(IServiceFactoryManager ServiceFactoryManager)
		{
			this.ServiceFactoryManager = ServiceFactoryManager;
		}
		public IServiceScope CreateServiceScope(long ScopeId)
		{
			return new ServiceScope(ServiceFactoryManager, ScopeId);
		}
	}

}
