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
	public delegate T TypedInstanceResolver<T>(long Id);


	public interface IServiceResolver 
	{
		IServiceProvider Provider { get; }
		long? CurrentServiceId { get; }
		IDisposable WithScopeService(long? ServiceId);
		object ResolveServiceByIdent(long ServiceId, Type ServiceType);
		IServiceInstanceDescriptor ResolveDescriptorByIdent(long ServiceId, Type ServiceType);

		object ResolveServiceByType(long? ScopeServiceId, Type ChildServiceType,string Name);
		IServiceInstanceDescriptor ResolveDescriptorByType(long? ScopeServiceId, Type ChildServiceType, string Name);

		//IServiceProvider CreateInternalServiceProvider(IServiceInstanceDescriptor Descriptor);

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
