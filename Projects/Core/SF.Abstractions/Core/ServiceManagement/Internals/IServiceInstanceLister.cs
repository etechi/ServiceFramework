using SF.Metadata.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement.Internals
{
	
	[UnmanagedService]
	public interface IServiceInstanceLister
	{
		long[] List(long? ScopeServiceId, string ServiceType, int Limit);
	}
	
}
