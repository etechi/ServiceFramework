using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.ServiceManagement
{
	//public interface IServiceContainer
	//{
	//	long Id { get; }
	//	IServiceDeclaration Declaration { get; }
	//	IServiceImplement Implement { get; }
	//	object ServiceInstance { get; }
	//}

	[UnmanagedService]
	public interface IServiceResolver 
	{
		object ResolveServiceByIdent(long ServiceId, Type ServiceType);
		IServiceInstanceDescriptor ResolveDescriptorByIdent(long ServiceId, Type ServiceType);

		object ResolveServiceByType(long? ScopeServiceId, Type ChildServiceType,string Name);
		IServiceInstanceDescriptor ResolveDescriptorByType(long? ScopeServiceId, Type ChildServiceType, string Name);

		IServiceProvider CreateInternalServiceProvider(long ServiceId);

		IEnumerable<IServiceInstanceDescriptor> ResolveServiceDescriptors(
			long? ScopeServiceId, 
			Type ChildServiceType,
			string Name
			);

		IEnumerable<object> ResolveServices(
			long? ScopeServiceId,
			Type ChildServiceType,
			string Name
			);

	}
}
