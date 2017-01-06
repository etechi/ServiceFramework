﻿using System;
using System.Linq;

namespace SF.DI
{
	public static class DIServiceCollectionExtension
	{
		public static IDIServiceCollection Replace(this IDIServiceCollection sc,ServiceDescriptor service)
		{
			sc.Remove(service.ServiceType);
			sc.Add(service);
			return sc;
		}
		public static IDIServiceCollection AddSingleton(this IDIServiceCollection sc,Type ServiceType, object Implement)
		{
			sc.Add(new ServiceDescriptor(ServiceType, Implement));
			return sc;
		}

		public static IDIServiceCollection AddSingleton<T>(this IDIServiceCollection sc,T implement)
		{
			sc.AddSingleton(typeof(T), implement);
			return sc;
		}
		public static IDIServiceCollection Add(this IDIServiceCollection sc,Type ServiceType,Type ImplementType,ServiceLifetime Lifetime)
		{
			sc.Add(new ServiceDescriptor(ServiceType, ImplementType, Lifetime));
			return sc;
		}
		public static IDIServiceCollection Add(this IDIServiceCollection sc, Type ServiceType, Func<IServiceProvider,object> ImplementCreator, ServiceLifetime Lifetime)
		{
			sc.Add(new ServiceDescriptor(ServiceType, ImplementCreator, Lifetime));
			return sc;
		}
		public static IDIServiceCollection AddSingleton<TService, TImplement>(this IDIServiceCollection sc)
			where TImplement : TService
		{
			sc.Add(typeof(TService), typeof(TImplement), ServiceLifetime.Singleton);
			return sc;
		}
		public static IDIServiceCollection AddSingleton<T>(
			this IDIServiceCollection sc,
			Func<IServiceProvider, T> ImplementCreator
			)
		{
			sc.Add(typeof(T), sp => ImplementCreator(sp), ServiceLifetime.Singleton);
			return sc;
		}
		
		public static IDIServiceCollection AddTransient<TService, TImplement>(this IDIServiceCollection sc)
			where TImplement : TService
		{
			sc.Add(typeof(TService), typeof(TImplement), ServiceLifetime.Transient);
			return sc;
		}
		public static IDIServiceCollection AddTransient<T>(
			this IDIServiceCollection sc,
			Func<IServiceProvider, T> ImplementCreator
			)
		{
			sc.Add(typeof(T), sp => ImplementCreator(sp), ServiceLifetime.Transient);
			return sc;
		}



		public static IDIServiceCollection AddScoped<TService, TImplement>(this IDIServiceCollection sc)
			where TImplement:TService
		{
			sc.Add(typeof(TService), typeof(TImplement), ServiceLifetime.Scoped);
			return sc;
		}
		public static IDIServiceCollection AddScoped<T>(
			this IDIServiceCollection sc,
			Func<IServiceProvider, T> ImplementCreator
			)
		{
			sc.Add(typeof(T), sp=>ImplementCreator(sp),ServiceLifetime.Scoped);
			return sc;
		}
	}
}
