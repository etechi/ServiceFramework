using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement
{
	public static class DIServiceCollectionExtension
	{
		class FuncManagedServiceInitializer : IManagedServiceInitializer
		{
			Func<IServiceProvider , IServiceInstanceDescriptor,Task> FuncInit { get; }
			Func<IServiceProvider, IServiceInstanceDescriptor, Task> FuncUninit { get; }
			public FuncManagedServiceInitializer(Func<IServiceProvider, IServiceInstanceDescriptor, Task> FuncInit, Func<IServiceProvider, IServiceInstanceDescriptor, Task> FuncUninit)
			{
				this.FuncInit = FuncInit;
				this.FuncUninit = FuncUninit;
			}
			public async Task Init(IServiceProvider ServiceProvider, IServiceInstanceDescriptor Descriptor)
			{
				if(FuncInit!=null)
					await FuncInit(ServiceProvider, Descriptor);
			}

			public async Task Uninit(IServiceProvider ServiceProvider, IServiceInstanceDescriptor Descriptor)
			{
				if (FuncInit != null)
					await FuncUninit(ServiceProvider, Descriptor);
			}
		}
		public static IEnumerable<Type> GetServiceTypes(this IServiceCollection sc)
		{
			return sc.Select(d => d.InterfaceType).Distinct();
		}
		public static IEnumerable<Type> GetServiceInterfaceTypes(this IServiceCollection sc)
		{
			return sc.Select(i=>i.InterfaceType).Distinct();
		}
		public static IServiceCollection Replace(this IServiceCollection sc,ServiceDescriptor service)
		{
			sc.Remove(service.InterfaceType);
			sc.Add(service);
			return sc;
		}
		public static IServiceCollection AddRange(this IServiceCollection sc, IEnumerable<ServiceDescriptor> ServiceDescriptors)
		{
			foreach (var sd in ServiceDescriptors)
				sc.Add(sd);
			return sc;
		}
		public static IServiceCollection AddSingleton(this IServiceCollection sc,Type ServiceType, object Implement)
		{
			sc.Add(
				new ServiceDescriptor(
					ServiceType,
					Implement
				)
			);
			return sc;
		}
		public static IServiceCollection AddSingleton(this IServiceCollection sc, Type ServiceType, Type ImplementType)
		{
			sc.Add(new ServiceDescriptor(
				ServiceType, 
				ImplementType,
				ServiceImplementLifetime.Singleton
			));
			return sc;
		}
		public static IServiceCollection AddSingleton<T>(this IServiceCollection sc,T implement)
		{
			sc.AddSingleton(typeof(T), implement);
			return sc;
		}
		public static IServiceCollection AddSingleton(this IServiceCollection sc, Type ImplementType,string Name=null)
		{
			sc.Add(ImplementType, ImplementType,ServiceImplementLifetime.Singleton,Name);
			return sc;
		}
		public static IServiceCollection AddTransient(this IServiceCollection sc, Type ImplementType,string Name=null)
		{
			sc.Add(ImplementType, ImplementType, ServiceImplementLifetime.Transient,Name);
			return sc;
		}
		public static IServiceCollection Add(
			this IServiceCollection sc,
			Type ServiceType,
			Type ImplementType,
			ServiceImplementLifetime Lifetime,
			string Name=null
			)
		{
			sc.Add(new ServiceDescriptor(ServiceType, ImplementType, Lifetime, false, null, Name));
			return sc;
		}
		public static IServiceCollection AddManaged(
			this IServiceCollection sc,
			Type ServiceType,
			Type ImplementType,
			ServiceImplementLifetime Lifetime,
			IManagedServiceInitializer ManagedServiceInitializer=null
			)
		{
			sc.Add(new ServiceDescriptor(ServiceType,ImplementType, Lifetime,true,ManagedServiceInitializer));
			return sc;
		}
		public static IServiceCollection Add(this IServiceCollection sc, Type ServiceType, System.Reflection.MethodInfo Method, ServiceImplementLifetime Lifetime,string Name=null)
		{
			sc.Add(new ServiceDescriptor(ServiceType, Method, Lifetime,Name));
			return sc;
		}
		public static IServiceCollection Add(this IServiceCollection sc, Type ServiceType, Func<IServiceProvider,object> ImplementCreator, ServiceImplementLifetime Lifetime,string Name=null)
		{
			sc.Add(new ServiceDescriptor(ServiceType,ImplementCreator, Lifetime,Name));
			return sc;
		}
		public static IServiceCollection AddSingleton<TService>(this IServiceCollection sc, string Name = null)
			=> sc.AddSingleton<TService, TService>(Name);

		public static IServiceCollection AddSingleton<TService, TImplement>(this IServiceCollection sc,string Name=null)
			where TImplement : TService
		{
			sc.Add(typeof(TService), typeof(TImplement), ServiceImplementLifetime.Singleton,Name);
			return sc;
		}
		public static IServiceCollection AddSingleton<T>(
			this IServiceCollection sc,
			Func<IServiceProvider, T> ImplementCreator,
			string Name=null
			)
		{
			sc.Add(typeof(T), sp => ImplementCreator(sp), ServiceImplementLifetime.Singleton);
			return sc;
		}
		public static IServiceCollection AddTransient(this IServiceCollection sc,Type Service,Type Implement,string Name=null)
		{
			sc.Add(Service, Implement, ServiceImplementLifetime.Transient, Name);
			return sc;
		}
		public static IServiceCollection AddTransient(this IServiceCollection sc, Type Service, System.Reflection.MethodInfo Method)
		{
			sc.Add(Service, Method, ServiceImplementLifetime.Transient);
			return sc;
		}

		public static IServiceCollection AddManagedTransient<TService, TImplement>(
			this IServiceCollection sc,
			Func<IServiceProvider, TService, Task> FuncUninit )
			where TImplement : TService
			where TService : class
			=> sc.AddManagedTransient<TService, TImplement>(
				null,
				FuncUninit == null ? null : new Func<IServiceProvider, IServiceInstanceDescriptor, Task>((sp, sd) => FuncUninit(sp, sp.Resolve<TService>(sd.InstanceId)))
				);
		public static IServiceCollection AddManagedTransient<TService, TImplement>(
			this IServiceCollection sc, 
			Func<IServiceProvider,IServiceInstanceDescriptor,Task> FuncInit,
			Func<IServiceProvider, IServiceInstanceDescriptor, Task> FuncUninit=null)
			where TImplement : TService
		{
			sc.AddManaged(typeof(TService), typeof(TImplement), ServiceImplementLifetime.Transient, new FuncManagedServiceInitializer(FuncInit,FuncUninit));
			return sc;
		}
		public static IServiceCollection AddManagedTransient<TService, TImplement>(this IServiceCollection sc,IManagedServiceInitializer ManagedServiceInitializer=null)
			where TImplement : TService
		{
			sc.AddManaged(typeof(TService), typeof(TImplement), ServiceImplementLifetime.Transient,ManagedServiceInitializer);
			return sc;
		}
		public static IServiceCollection AddTransient<TService, TImplement>(this IServiceCollection sc,string Name=null)
			where TImplement : TService
		{
			sc.Add(typeof(TService), typeof(TImplement), ServiceImplementLifetime.Transient,Name);
			return sc;
		}
		public static IServiceCollection AddTransient<TService>(this IServiceCollection sc,string Name=null)
		   where TService : class
		{
			sc.Add(typeof(TService), typeof(TService), ServiceImplementLifetime.Transient,Name);
			return sc;
		}
		
		public static IServiceCollection AddTransient<T>(
			this IServiceCollection sc,
			Func<IServiceProvider, T> ImplementCreator
			)
		{
			sc.Add(typeof(T), sp => ImplementCreator(sp), ServiceImplementLifetime.Transient);
			return sc;
		}

		public static IServiceCollection AddManagedScoped<TService, TImplement>(
			this IServiceCollection sc,
			Func<IServiceProvider, TService, Task> FuncUninit
			)
			where TImplement : TService
			where TService:class
			=> sc.AddManagedScoped<TService, TImplement>(
				null,
				FuncUninit == null ?null : new Func<IServiceProvider, IServiceInstanceDescriptor, Task>((sp, sd) => FuncUninit(sp, sp.Resolve<TService>(sd.InstanceId)))
				);

		public static IServiceCollection AddManagedScoped<TService, TImplement>(
			this IServiceCollection sc,
			Func<IServiceProvider, IServiceInstanceDescriptor, Task> FuncInit,
			Func<IServiceProvider, IServiceInstanceDescriptor, Task> FuncUninit = null)
			where TImplement : TService
		{
			sc.AddManaged(typeof(TService), typeof(TImplement), ServiceImplementLifetime.Scoped, new FuncManagedServiceInitializer(FuncInit, FuncUninit));
			return sc;
		}
		public static IServiceCollection AddManagedScoped<TService, TImplement>(this IServiceCollection sc,IManagedServiceInitializer ManagedServiceInitializer=null)
			where TImplement : TService
		{
			sc.AddManaged(typeof(TService), typeof(TImplement), ServiceImplementLifetime.Scoped,ManagedServiceInitializer);
			return sc;
		}
		public static IServiceCollection AddScoped<TService, TImplement>(this IServiceCollection sc,string Name=null)
			where TImplement:TService
		{
			sc.Add(typeof(TService), typeof(TImplement), ServiceImplementLifetime.Scoped,Name);
			return sc;
		}
		public static IServiceCollection AddScoped(this IServiceCollection sc, Type Service, System.Reflection.MethodInfo Method)
		{
			sc.Add(Service, Method, ServiceImplementLifetime.Scoped);
			return sc;
		}
		public static IServiceCollection AddScoped<TService>(this IServiceCollection sc,string Name=null)
		   where TService : class
		{
			sc.Add(typeof(TService), typeof(TService), ServiceImplementLifetime.Scoped,Name);
			return sc;
		}
		public static IServiceCollection AddScoped<T>(
			this IServiceCollection sc,
			Func<IServiceProvider, T> ImplementCreator,
			string Name=null
			)
		{
			sc.Add(typeof(T), sp=>ImplementCreator(sp),ServiceImplementLifetime.Scoped,Name);
			return sc;
		}
	}
}
