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
		public static T Resolve<T>(this IServiceResolver ServiceResolver, long ServiceId)
			where T : class
		{
			var s = ServiceResolver.Resolve(ServiceId);
			if (s == null)
				throw new InvalidOperationException("找不到服务:" + typeof(T));
			var re = s as T;
			if(re==null)
				throw new InvalidOperationException($"服务:{ServiceId}不是类型:{typeof(T)}");
			return re;
		}
		public static T Resolve<T>(this IServiceProvider ServiceProvider, long ServiceId)
		   where T : class
			=> ServiceProvider.Resolve<IServiceResolver>().Resolve<T>(ServiceId);

		public static T WithScope<T>(this IServiceProvider sp, Func<IServiceResolver, T> action,long ScopeId=0)
		{
			using (var s = sp.Resolve<IServiceScopeFactory>().CreateServiceScope(ScopeId))
				return action(s.ServiceResolver);
		}
		public static async Task<T> WithScope<T>(this IServiceProvider sp, Func<IServiceResolver, Task<T>> action, long ScopeId = 0)
		{
			using (var s = sp.Resolve<IServiceScopeFactory>().CreateServiceScope(ScopeId))
				return await action(s.ServiceResolver);
		}
		public static void WithScope(this IServiceProvider sp, Action<IServiceResolver> action, long ScopeId = 0)
		{
			using (var s = sp.Resolve<IServiceScopeFactory>().CreateServiceScope(ScopeId))
				action(s.ServiceResolver);
		}
		public static async Task WithScope(this IServiceProvider sp, Func<IServiceResolver, Task> action, long ScopeId = 0)
		{
			using (var s = sp.Resolve<IServiceScopeFactory>().CreateServiceScope(ScopeId))
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
