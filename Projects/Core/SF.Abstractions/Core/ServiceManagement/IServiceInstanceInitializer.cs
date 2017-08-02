using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement
{
	public interface IServiceInstanceInitializer
	{
		string Name { get; }
		Task<long> Ensure(IServiceProvider ServiceProvider, long? ParentId);
	}
	public interface IServiceInstanceInitializer<T> : IServiceInstanceInitializer
	{

	}
}
