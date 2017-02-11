using System;
using System.Collections.Generic;
using SF.Core.DI;
using System.Linq;
using System.Collections;

namespace SF.Core.ManagedServices
{
	class ManagedServiceCollection : 
		IManagedServiceCollection
	{
		public SF.Core.DI.IDIServiceCollection NormalServiceCollection { get; }
		List<ServiceDescriptor> Descriptors { get; } = new List<ServiceDescriptor>();
		HashSet<Type> ServiceTypeHash { get; } = new HashSet<Type>();
		public IEnumerable<Type> ServiceTypes=> ServiceTypeHash;

		public ManagedServiceCollection(SF.Core.DI.IDIServiceCollection sc)
		{
			NormalServiceCollection = sc;
		}

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
			if (Descriptor.Lifetime != ServiceLifetime.Scoped && Descriptor.Lifetime != ServiceLifetime.Transient)
				throw new NotSupportedException();
			if (Descriptor.ImplementType == null || Descriptor.ImplementInstance != null || Descriptor.ImplementCreator != null)
				throw new NotSupportedException();

			Descriptors.Add(Descriptor);
			if(ServiceTypeHash.Add(Descriptor.ServiceType))
				NormalServiceCollection.Add(
					new ServiceDescriptor(
						Descriptor.ServiceType,
						isp => isp.Resolve<Runtime.IManagedServiceScope>().Resolve(
							Descriptor.ServiceType, 
							null
							),
						Descriptor.Lifetime
					));
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
