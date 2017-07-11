using System;
namespace SF.Core.ServiceManagement.Internals
{
	[UnmanagedService]
	public interface IServiceFactoryManager
	{
		IServiceFactory GetServiceFactory(
			IServiceProvider ServiceProvider, 
			long ServiceScopeId,
			Type InterfaceType
			);
		IServiceFactory GetServiceFactory(
			IServiceProvider ServiceProvider,
			long ServiceId
			);
	}

}
