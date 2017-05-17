using System;
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
		public static T WithScope<T>(this IServiceProvider sp, Func<IServiceResolver, T> action)
		{
			using (var s = sp.Resolve<IServiceScopeFactory>().CreateServiceScope())
				return action(s.ServiceResolver);
		}
		public static async Task<T> WithScope<T>(this IServiceProvider sp, Func<IServiceResolver, Task<T>> action)
		{
			using (var s = sp.Resolve<IServiceScopeFactory>().CreateServiceScope())
				return await action(s.ServiceResolver);
		}
		public static void WithScope(this IServiceProvider sp, Action<IServiceResolver> action)
		{
			using (var s = sp.Resolve<IServiceScopeFactory>().CreateServiceScope())
				action(s.ServiceResolver);
		}
		public static async Task WithScope(this IServiceProvider sp, Func<IServiceResolver, Task> action)
		{
			using (var s = sp.Resolve<IServiceScopeFactory>().CreateServiceScope())
				await action(s.ServiceResolver);
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
