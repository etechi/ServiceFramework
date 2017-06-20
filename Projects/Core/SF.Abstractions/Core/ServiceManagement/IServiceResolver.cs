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
	public interface IServiceResolver : IServiceProvider
	{
		object Resolve(
			int AppId,
			Type ServiceType,
			string ServiceInstanceId,
			Type InterfaceType
			);
	}
}
