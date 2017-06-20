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
		public Type ServiceType { get; }
		public ServiceInterfaceDescriptor[] Interfaces { get; }
		public ServiceDescriptor(Type ServiceType, params ServiceInterfaceDescriptor[] InterfaceDescriptors)
		{
			this.ServiceType = ServiceType;
			this.Interfaces = InterfaceDescriptors;
		}
		public ServiceDescriptor(Type InterfaceType, Type ImplementType, ServiceImplementLifetime Lifetime):
			this(InterfaceType,new ServiceInterfaceDescriptor(InterfaceType,ImplementType,Lifetime))
		{
			
		}
		public ServiceDescriptor(Type InterfaceType, object Implement) :
			this(InterfaceType, new ServiceInterfaceDescriptor(InterfaceType, Implement))
		{

		}
		public ServiceDescriptor(Type InterfaceType, Func<IServiceProvider, object> ImplementCreator, ServiceImplementLifetime Lifetime) :
			this(InterfaceType, new ServiceInterfaceDescriptor(InterfaceType, ImplementCreator, Lifetime))
		{

		}
	}
	public class ServiceInterfaceDescriptor
	{
		public Type InterfaceType { get; }
		public Type ImplementType { get; }
		public object ImplementInstance { get; }
		public Func<IServiceProvider, object> ImplementCreator { get; }
		public ServiceImplementLifetime Lifetime { get; }
		public ServiceImplementType ServiceImplementType { get; }
		public ServiceInterfaceDescriptor(Type InterfaceType, Type ImplementType, ServiceImplementLifetime Lifetime)
		{
			this.ServiceImplementType = ServiceImplementType.Type;
			this.InterfaceType = InterfaceType;
			this.ImplementType = ImplementType;
			this.Lifetime = Lifetime;
		}
		public ServiceInterfaceDescriptor(Type InterfaceType, object Implement)
		{
			this.ServiceImplementType = ServiceImplementType.Instance;
			this.InterfaceType = InterfaceType;
			this.ImplementInstance = Implement;
			this.Lifetime = ServiceImplementLifetime.Singleton;
		}
		public ServiceInterfaceDescriptor(Type InterfaceType, Func<IServiceProvider, object> ImplementCreator, ServiceImplementLifetime Lifetime)
		{
			this.ServiceImplementType = ServiceImplementType.Creator;
			this.InterfaceType = InterfaceType;
			this.ImplementCreator = ImplementCreator;
			this.Lifetime = Lifetime;
		}
	}
}
