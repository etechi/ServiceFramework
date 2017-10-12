using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.NetworkService
{
	public interface IServiceInvoker
	{
		object Invoke(IServiceProvider ServiceProvider,long? ScopeId,string Service, string Method, string Argument);
	}
}
