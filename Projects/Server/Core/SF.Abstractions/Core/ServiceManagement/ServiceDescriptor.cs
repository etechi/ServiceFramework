using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
		Creator,
		Method
	}

	public interface IManagedServiceInitializer
	{
		Task Init(IServiceProvider ServiceProvider, IServiceInstanceDescriptor Descriptor);
		Task Uninit(IServiceProvider ServiceProvider, IServiceInstanceDescriptor Descriptor);
	}
	
	public class ServiceDescriptor
	{
		public string Name { get; }
		public Type InterfaceType { get; }
		public Type ImplementType { get; }
		public object ImplementInstance { get; }
		public Func<IServiceProvider, object> ImplementCreator { get; }
		public ServiceImplementLifetime Lifetime { get; }
		public ServiceImplementType ServiceImplementType { get; }
		public System.Reflection.MethodInfo ImplementMethod { get; }
		public bool IsManagedService { get; }
		public IManagedServiceInitializer ManagedServiceInitializer { get; }

		public ServiceDescriptor(Type InterfaceType, Type ImplementType, ServiceImplementLifetime Lifetime,bool IsManagedService=false,IManagedServiceInitializer ManagedServiceInitializer=null,string Name=null)
		{
			if(ImplementType==null)
				throw new ArgumentNullException();
			if(InterfaceType==null)
				throw new ArgumentNullException();
			if (InterfaceType.IsGenericTypeDefinition != ImplementType.IsGenericTypeDefinition)
				throw new ArgumentException($"{ImplementType}是{(ImplementType.IsGenericTypeDefinition?"泛型定义":"普通类")}，而{InterfaceType}是{(InterfaceType.IsGenericTypeDefinition ? "泛型定义" : "普通类")}");

			if (InterfaceType.IsGenericTypeDefinition)
			{
				var ifgpc = InterfaceType.GenericTypeArguments.Length;
				var impgpc = ImplementType.GenericTypeArguments.Length;
				if (ifgpc!=impgpc)
					throw new ArgumentException($"{ImplementType}需要{impgpc}个泛型参数，而{InterfaceType}需要{ifgpc}个泛型参数");
			}
			else if (!InterfaceType.IsAssignableFrom(ImplementType))
				throw new ArgumentException($"{ImplementType}没有实现接口{InterfaceType}");

			this.Name = Name;
			this.ServiceImplementType = ServiceImplementType.Type;
			this.InterfaceType = InterfaceType;
			this.ImplementType = ImplementType;
			this.Lifetime = Lifetime;
			this.IsManagedService = IsManagedService;
			this.ManagedServiceInitializer = this.ManagedServiceInitializer;
		}
		public ServiceDescriptor(Type InterfaceType, object Implement,string Name=null)
		{
			if (Implement == null )
				throw new ArgumentNullException();
			if (!InterfaceType.IsAssignableFrom(Implement.GetType()))
				throw new ArgumentException($"给定对象和接口类型不符合{InterfaceType}");

			this.Name = Name;
			this.ServiceImplementType = ServiceImplementType.Instance;
			this.InterfaceType = InterfaceType;
			this.ImplementInstance = Implement;
			this.Lifetime = ServiceImplementLifetime.Singleton;
		}
		public ServiceDescriptor(Type InterfaceType, Func<IServiceProvider, object> ImplementCreator, ServiceImplementLifetime Lifetime,string Name=null)
		{
			this.ServiceImplementType = ServiceImplementType.Creator;
			this.InterfaceType = InterfaceType;
			this.ImplementCreator = ImplementCreator;
			this.Lifetime = Lifetime;
			this.Name = Name;
		}
		public ServiceDescriptor(Type InterfaceType, MethodInfo ImplementMethod, ServiceImplementLifetime Lifetime,string Name=null)
		{
			this.ServiceImplementType = ServiceImplementType.Method;
			this.InterfaceType = InterfaceType;
			this.ImplementMethod = ImplementMethod;
			this.Lifetime = Lifetime;
			this.Name = Name;
		}
	}
}
