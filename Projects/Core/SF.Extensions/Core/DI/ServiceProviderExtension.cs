using System;
using System.Collections.Generic;

namespace SF.Core.DI
{
	public static class ServiceProviderExtension
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
