using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.ServiceManagement
{
	[UnmanagedService]
	public interface IServiceResolver : IServiceProvider
	{

		object Resolve(
			Type ServiceType, 
			long ServiceInstanceId
			);
	}
}
