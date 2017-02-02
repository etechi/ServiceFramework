using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.DI
{


	public interface IDIServiceCollection 
    {
		IEnumerable<Type> ServiceTypes { get; }
		void Add(ServiceDescriptor Descriptor);
		void Remove(Type Service);
	}
}
