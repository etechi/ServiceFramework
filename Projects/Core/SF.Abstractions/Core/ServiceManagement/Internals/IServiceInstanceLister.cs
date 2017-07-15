using SF.Metadata.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement.Internals
{
	public class ServiceReference
	{
		public long Id { get; set; }
		public string Name { get; set; }
	}
	[UnmanagedService]
	public interface IServiceInstanceLister
	{
		ServiceReference[] List(long? ScopeServiceId, string ServiceType, int Limit);
	}
	
}
