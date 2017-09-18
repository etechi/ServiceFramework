using System;


namespace SF.Core.ServiceManagement
{
	public interface IManagedServiceWithId
	{
		long ServiceInstanceId { get; }
	}
}
