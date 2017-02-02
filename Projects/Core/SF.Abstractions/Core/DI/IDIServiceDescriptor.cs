using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.DI
{

	public enum ServiceLifetime
	{
		Singleton = 0,
		Scoped = 1,
		Transient = 2
	}
	public enum ServiceImplementType
	{
		Type,
		Instance,
		Creator
	}
	public class ServiceDescriptor
	{
		public Type ServiceType { get; }
		public Type ImplementType { get; }
		public object ImplementInstance { get; }
		public Func<IServiceProvider,object> ImplementCreator { get; }
		public ServiceLifetime Lifetime { get; }
		public ServiceImplementType ServiceImplementType { get; }
		public ServiceDescriptor(Type ServiceType,Type ImplementType,ServiceLifetime Lifetime)
		{
			this.ServiceImplementType = ServiceImplementType.Type;
			this.ServiceType = ServiceType;
			this.ImplementType = ImplementType;
			this.Lifetime = Lifetime;
		}
		public ServiceDescriptor(Type ServiceType, object Implement)
		{
			this.ServiceImplementType = ServiceImplementType.Instance;
			this.ServiceType = ServiceType;
			this.ImplementInstance= Implement;
			this.Lifetime = ServiceLifetime.Singleton;
		}
		public ServiceDescriptor(Type ServiceType, Func<IServiceProvider,object> ImplementCreator, ServiceLifetime Lifetime)
		{
			this.ServiceImplementType = ServiceImplementType.Creator;
			this.ServiceType = ServiceType;
			this.ImplementCreator= ImplementCreator;
			this.Lifetime = Lifetime;
		}
	}
}
