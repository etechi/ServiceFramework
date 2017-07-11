using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.ServiceManagement
{
	public enum ServiceImplementLifetime
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
		public Type InterfaceType { get; }
		public Type ImplementType { get; }
		public object ImplementInstance { get; }
		public Func<IServiceProvider, object> ImplementCreator { get; }
		public ServiceImplementLifetime Lifetime { get; }
		public ServiceImplementType ServiceImplementType { get; }
		public ServiceDescriptor(Type InterfaceType, Type ImplementType, ServiceImplementLifetime Lifetime)
		{
			this.ServiceImplementType = ServiceImplementType.Type;
			this.InterfaceType = InterfaceType;
			this.ImplementType = ImplementType;
			this.Lifetime = Lifetime;
		}
		public ServiceDescriptor(Type InterfaceType, object Implement)
		{
			this.ServiceImplementType = ServiceImplementType.Instance;
			this.InterfaceType = InterfaceType;
			this.ImplementInstance = Implement;
			this.Lifetime = ServiceImplementLifetime.Singleton;
		}
		public ServiceDescriptor(Type InterfaceType, Func<IServiceProvider, object> ImplementCreator, ServiceImplementLifetime Lifetime)
		{
			this.ServiceImplementType = ServiceImplementType.Creator;
			this.InterfaceType = InterfaceType;
			this.ImplementCreator = ImplementCreator;
			this.Lifetime = Lifetime;
		}
	}
}
