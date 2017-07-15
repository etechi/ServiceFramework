using SF.Core.ServiceManagement.Internals;
using System;
using System.Collections.Generic;
namespace SF.Core.ServiceManagement
{
	public class ServiceScopeFactory : IServiceScopeFactory
	{
		IServiceProvider ServiceProvider { get; }
		IServiceFactoryManager ServiceFactoryManager { get; }
		public ServiceScopeFactory(IServiceProvider ServiceProvider, IServiceFactoryManager ServiceFactoryManager)
		{
			this.ServiceProvider = ServiceProvider;
			this.ServiceFactoryManager = ServiceFactoryManager;
		}
		public IServiceScope CreateServiceScope()
		{
			return new ServiceScope(ServiceProvider, ServiceFactoryManager);
		}
	}

}
