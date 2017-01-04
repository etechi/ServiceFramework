using System;
using System.Reflection;
namespace SF.ServiceManagement
{
	public class ManagedServiceDescriptor
	{
		public Type ServiceType { get; }
		public Type ImplementType { get; }
		public ManagedServiceDescriptor(Type ServiceType, Type ImplementType)
		{
			if (ServiceType == null)
				throw new ArgumentNullException();

			if (ImplementType == null)
				throw new ArgumentNullException();

			if (!ServiceType.GetTypeInfo().IsAssignableFrom(ImplementType.GetTypeInfo()))
				throw new ArgumentNullException();

			//if (Lifetime != ServiceLifetime.Scoped)
			//	throw new ArgumentException();

			this.ServiceType = ServiceType;
			this.ImplementType = ImplementType;
			//this.Lifetime = Lifetime;
		}
	}
}
