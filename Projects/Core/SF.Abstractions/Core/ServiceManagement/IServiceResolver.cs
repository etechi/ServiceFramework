using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.ServiceManagement
{
	public interface IServiceResolver : IServiceProvider
	{

		object Resolve(
			Type ServiceType, 
			long ServiceInstanceId
			);
	}
}
