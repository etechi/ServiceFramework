using System;
using System.Collections.Generic;
namespace SF.Core.ServiceManagement.Internals
{
	public interface IServiceEntry { }

	public interface IServiceFactoryManager
	{
		Type GetServiceTypeByIdent(IServiceResolver ServiceResolver, long ServiceId);

		IServiceFactory GetServiceFactoryByIdent(
			IServiceResolver ServiceResolver, 
			long ServiceId,
			Type ServiceType
			);
		IServiceFactory GetServiceFactoryByType(
			IServiceResolver ServiceResolver,
			long? ScopeServiceId,
			Type ServiceType,
			string Name
			);
		IEnumerable<IServiceFactory> GetServiceFactoriesByType(
			IServiceResolver ServiceResolver,
			long? ScopeServiceId,
			Type ChildServiceType,
			string Name
			);

		void SubscribeEvents(IServiceProvider Provider);

	}

}
