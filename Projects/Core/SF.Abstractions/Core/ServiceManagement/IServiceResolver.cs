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
		long? Resolve(long? ParentServiceId, Type ServiceType);
		object Resolve(long ServiceId);
		IServiceProvider CreateInternalServiceProvider(long ServiceId);
	}
}
