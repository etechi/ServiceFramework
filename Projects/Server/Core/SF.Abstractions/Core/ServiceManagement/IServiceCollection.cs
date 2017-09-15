using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.ServiceManagement
{
	public interface IServiceCollection : IEnumerable<ServiceDescriptor>
	{	
		void Add(ServiceDescriptor Descriptor);
		void Remove(Type Service);
	}
}
