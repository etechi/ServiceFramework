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
			string ServiceInstanceId,
			Type InterfaceType
			);
	}

}
