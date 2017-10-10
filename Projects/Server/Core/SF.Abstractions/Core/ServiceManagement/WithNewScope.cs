using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement
{
	public interface IScoped<S>
	{
		Task<T> Use<T>(Func<S, Task<T>> Callback);
	}

}
