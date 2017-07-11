using System;

namespace SF.Core.ServiceManagement
{
	public interface IServiceInstanceMeta
	{

		long Id { get; }
		long? ParentId { get; }

		IServiceProvider InternalServiceProvider { get; }

	}

}
