using System;

namespace SF.Core.ServiceManagement
{
	public interface IServiceInstanceDescriptor
	{
		long? InstanceId { get; }
		long? ParentInstanceId { get; }
		IServiceDeclaration ServiceDeclaration { get; }
		IServiceImplement ServiceImplement { get; }

	}

}
