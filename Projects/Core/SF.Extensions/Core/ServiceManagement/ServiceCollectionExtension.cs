using System;
using System.Collections.Generic;
using System.Linq;

namespace SF.Core.ServiceManagement
{
	public static class DIServiceCollectionExtension
	{
		public static IEnumerable<Type> GetServiceTypes(this IServiceCollection sc)
		{
			return sc.Select(d => d.ServiceType).Distinct();
		}
		public static IEnumerable<Type> GetServiceInterfaceTypes(this IServiceCollection sc)
		{
			return sc.SelectMany(d => d.Interfaces.Select(i=>i.InterfaceType)).Distinct();
		}
		public static IServiceCollection Replace(this IServiceCollection sc,ServiceDescriptor service)
		{
			sc.Remove(service.ServiceType);
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
		public static IServiceCollection AddSingleton(this IServiceCollection sc, Type ImplementType)
		{
			sc.Add(ImplementType, ImplementType,ServiceImplementLifetime.Singleton);
			return sc;
		}
		public static IServiceCollection AddTransient(this IServiceCollection sc, Type ImplementType)
		{
			sc.Add(ImplementType, ImplementType, ServiceImplementLifetime.Transient);
			return sc;
		}
		public static IServiceCollection Add(this IServiceCollection sc,Type ServiceType,Type ImplementType,ServiceImplementLifetime Lifetime)
		{
			sc.Add(new ServiceDescriptor(ServiceType,ImplementType, Lifetime));
			return sc;
		}
		public static IServiceCollection Add(this IServiceCollection sc, Type ServiceType, Func<IServiceProvider,object> ImplementCreator, ServiceImplementLifetime Lifetime)
		{
			sc.Add(new ServiceDescriptor(ServiceType,ImplementCreator, Lifetime));
			return sc;
		}
		public static IServiceCollection AddSingleton<TService, TImplement>(this IServiceCollection sc)
			where TImplement : TService
		{
			sc.Add(typeof(TService), typeof(TImplement), ServiceImplementLifetime.Singleton);
			return sc;
		}
		public static IServiceCollection AddSingleton<T>(
			this IServiceCollection sc,
			Func<IServiceProvider, T> ImplementCreator
			)
		{
			sc.Add(typeof(T), sp => ImplementCreator(sp), ServiceImplementLifetime.Singleton);
			return sc;
		}
		public static IServiceCollection AddTransient(this IServiceCollection sc,Type Service,Type Implement)
		{
			sc.Add(Service, Implement, ServiceImplementLifetime.Transient);
			return sc;
		}
		public static IServiceCollection AddTransient<TService, TImplement>(this IServiceCollection sc)
			where TImplement : TService
		{
			sc.Add(typeof(TService), typeof(TImplement), ServiceImplementLifetime.Transient);
			return sc;
		}
		public static IServiceCollection AddTransient<TService>(this IServiceCollection sc)
		   where TService : class
		{
			sc.Add(typeof(TService), typeof(TService), ServiceImplementLifetime.Transient);
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



		public static IServiceCollection AddScoped<TService, TImplement>(this IServiceCollection sc)
			where TImplement:TService
		{
			sc.Add(typeof(TService), typeof(TImplement), ServiceImplementLifetime.Scoped);
			return sc;
		}
		public static IServiceCollection AddScoped<T>(
			this IServiceCollection sc,
			Func<IServiceProvider, T> ImplementCreator
			)
		{
			sc.Add(typeof(T), sp=>ImplementCreator(sp),ServiceImplementLifetime.Scoped);
			return sc;
		}
	}
}
