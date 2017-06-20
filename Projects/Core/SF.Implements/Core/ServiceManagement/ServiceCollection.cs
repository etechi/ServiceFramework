using System;
using System.Collections.Generic;

using System.Linq;
using System.Collections;

namespace SF.Core.ServiceManagement
{
	public class ServiceCollection : IServiceCollection
	{
		List<ServiceDescriptor> Descriptors { get; } = new List<ServiceDescriptor>();
		HashSet<Type> ServiceTypeHash { get; } = new HashSet<Type>();
		public IEnumerable<Type> ServiceTypes=> ServiceTypeHash;

		public void Remove(Type Service)
		{
			for (var i = 0; i < Descriptors.Count; i++)
				if (Descriptors[i].ServiceType == Service)
				{
					Descriptors.RemoveAt(i);
					i--;
				}
		}

		public void Add(ServiceDescriptor Descriptor)
		{
			Descriptors.Add(Descriptor);
			//if (Descriptor.Lifetime != ServiceInterfaceImplementLifetime.Scoped && Descriptor.Lifetime != ServiceInterfaceImplementLifetime.Transient)
			//	throw new NotSupportedException();
			//if (Descriptor.ImplementType == null || Descriptor.ImplementInstance != null || Descriptor.ImplementCreator != null)
			//	throw new NotSupportedException();
			//if (ServiceTypeHash.Add(Descriptor.ServiceType))
			//	NormalServiceCollection.Add(
			//		new ServiceDescriptor(
			//			Descriptor.ServiceType,
			//			isp => isp.Resolve<Runtime.IManagedServiceScope>().Resolve(
			//				Descriptor.ServiceType, 
			//				null
			//				),
			//			Descriptor.Lifetime
			//		));
		}

		public IEnumerator<ServiceDescriptor> GetEnumerator()
		{
			return Descriptors.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

}
