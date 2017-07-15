using System;
using System.Collections.Generic;
namespace SF.Core.ServiceManagement.Internals
{
	public interface IServiceEntry { }

	[UnmanagedService]
	public interface IServiceFactoryManager
	{
		IServiceFactory GetServiceFactoryByIdent(
			IServiceProvider ServiceProvider, 
			long ServiceId,
			Type ServiceType
			);
		IServiceFactory GetServiceFactoryByType(
			IServiceProvider ServiceProvider,
			long? ScopeServiceId,
			Type ServiceType,
			string Name
			);
		IEnumerable<IServiceFactory> GetServiceFactoriesByType(
			IServiceProvider ServiceProvider,
			long? ScopeServiceId,
			Type ChildServiceType,
			string Name
			);

	}

}
