using System;
namespace SF.Core.ServiceManagement.Internals
{

	public interface IServiceFactoryManager
	{
		IServiceFactory GetServiceFactory(
			IServiceResolver ServiceResolver, 
			Type ServiceType, 
			long ServiceInstanceId
			);
	}

}
