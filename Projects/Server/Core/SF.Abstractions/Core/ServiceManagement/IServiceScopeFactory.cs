using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.ServiceManagement
{
	public interface IServiceScopeFactory
	{
		IServiceScope CreateServiceScope();
	}
}
