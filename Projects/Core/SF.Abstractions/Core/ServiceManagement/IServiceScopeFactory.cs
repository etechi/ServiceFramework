using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.ServiceManagement
{
	[UnmanagedService]
	public interface IServiceScopeFactory
	{
		IServiceScope CreateServiceScope();
	}
}
