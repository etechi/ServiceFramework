using System;
namespace SF.Core.ServiceManagement.Internals
{
	[UnmanagedService]
	public interface IServiceFactoryManager
	{
		IServiceFactory GetServiceFactory(
			IServiceResolver ServiceResolver, 
			Type ServiceType, 
			long ServiceInstanceId
			);
	}

}
