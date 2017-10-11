using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement
{
	public interface IServiceInvoker
	{
		object Invoke(string Service, string Method, string Json);
	}
}
