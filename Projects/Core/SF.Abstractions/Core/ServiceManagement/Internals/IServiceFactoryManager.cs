using System;
namespace SF.Core.ServiceManagement.Internals
{
	[UnmanagedService]
	public interface IServiceFactoryManager
	{
		IServiceInterfaceFactory GetServiceFactory(
			IServiceResolver ServiceResolver, 
			int AppId,
			Type ServiceType,
			long ServiceInstanceId,
			Type InterfaceType
			);
	}

}
