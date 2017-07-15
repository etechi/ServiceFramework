﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement
{
	public static class ServiceResolverExtension
	{
		public static T TryResolve<T>(this IServiceProvider ServiceProvider)
		{
			return (T)ServiceProvider.GetService(typeof(T));
		}
		public static T Resolve<T>(this IServiceProvider ServiceProvider)
			where T:class
		{
			var s=(T)ServiceProvider.GetService(typeof(T));
			if (s == null)
				throw new InvalidOperationException("找不到服务:" + typeof(T));
			return s;
		}
		public static T Resolve<T>(this IServiceProvider ServiceProvider, long ServiceId)
			where T : class
		{
			var s = ServiceProvider.Resolve<IServiceResolver>().ResolveServiceByIdent(ServiceId,typeof(T));
			if (s == null)
				throw new InvalidOperationException("找不到服务:" + typeof(T));
			var re = s as T;
			if(re==null)
				throw new InvalidOperationException($"服务:{ServiceId}不是类型:{typeof(T)}");
			return re;
		}
		public static T ResolveInternal<T>(this IServiceProvider ServiceProvider, long ScopeServiceId)
			where T : class
		{
			var s = ServiceProvider.Resolve<IServiceResolver>().ResolveServiceByType(ScopeServiceId, typeof(T));
			if (s == null)
				throw new InvalidOperationException("找不到服务:" + typeof(T));
			return (T)s;
		}

		public static T WithScope<T>(this IServiceProvider sp, Func<IServiceProvider, T> action)
		{
			using (var s = sp.Resolve<IServiceScopeFactory>().CreateServiceScope())
				return action(s.ServiceProvider);
		}
		public static async Task<T> WithScope<T>(this IServiceProvider sp, Func<IServiceProvider, Task<T>> action)
		{
			using (var s = sp.Resolve<IServiceScopeFactory>().CreateServiceScope())
				return await action(s.ServiceProvider);
		}
		public static void WithScope(this IServiceProvider sp, Action<IServiceProvider> action)
		{
			using (var s = sp.Resolve<IServiceScopeFactory>().CreateServiceScope())
				action(s.ServiceProvider);
		}
		public static async Task WithScope(this IServiceProvider sp, Func<IServiceProvider, Task> action)
		{
			using (var s = sp.Resolve<IServiceScopeFactory>().CreateServiceScope())
				await action(s.ServiceProvider);
		}
		interface IServicesGetter
		{
			IEnumerable<object> GetServices(IServiceProvider ServiceProvider);
		}
		class ServicesGetter<T>: IServicesGetter
		{
			public IEnumerable<object> GetServices(IServiceProvider ServiceProvider)
			{
				foreach (var v in ((IEnumerable<T>)ServiceProvider.GetService(typeof(IEnumerable<T>))))
					yield return v;
			}
		}
		public static IEnumerable<object> GetServices<T>(this IServiceProvider ServiceProvider)
		{
			return new ServicesGetter<T>().GetServices(ServiceProvider);
		}
		static System.Collections.Concurrent.ConcurrentDictionary<Type, IServicesGetter> ServicesGetters { get; } = new System.Collections.Concurrent.ConcurrentDictionary<Type, IServicesGetter>();
		public static IEnumerable<object> GetServices(this IServiceProvider ServiceProvider,Type Type)
		{
			IServicesGetter sc;
			if (!ServicesGetters.TryGetValue(Type, out sc))
				sc = ServicesGetters.GetOrAdd(
					Type,
					(IServicesGetter)Activator.CreateInstance(
					typeof(ServicesGetter<>).MakeGenericType(Type))
					);
			return sc.GetServices(ServiceProvider);
		}
	}
}
