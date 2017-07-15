using SF.Core.ServiceManagement;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.Linq;
namespace SF.Core.ManagedServices.Runtime
{
	public class EnumerableService<T> : IEnumerable<T>
	{
		IServiceResolver Resolver { get; }
		IServiceInstanceDescriptor Descriptor { get; }
		public EnumerableService(IServiceResolver Resolver,IServiceInstanceDescriptor descriptor)
		{
			this.Resolver = Resolver;
			this.Descriptor = descriptor;
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (var i in Resolver.ResolveServices(Descriptor.ParentInstanceId, typeof(T),null))
				yield return (T)i;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
