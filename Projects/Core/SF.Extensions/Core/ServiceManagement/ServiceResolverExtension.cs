using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
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
		public static IServiceResolver NewResolver(this IServiceProvider ServiceProvider)
			=> ServiceProvider.Resolve<IServiceResolver>();

		public static T Resolve<T>(this IServiceProvider ServiceProvider, long ServiceId)
			where T : class
		{
			var s = ServiceProvider.NewResolver().ResolveServiceByIdent(ServiceId, typeof(T));
			if (s == null)
				throw new InvalidOperationException("找不到服务:" + typeof(T));
			var re = s as T;
			if (re == null)
				throw new InvalidOperationException($"服务:{ServiceId}不是类型:{typeof(T)}");
			return re;
		}
		public static T ResolveInternal<T>(this IServiceProvider ServiceProvider, long ScopeServiceId,string Name=null)
			where T : class
		{
			var s = ServiceProvider.NewResolver().ResolveServiceByType(ScopeServiceId, typeof(T), Name);
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
		public static IEnumerable<T> GetServices<T>(this IServiceProvider ServiceProvider, long? ScopeServiceId = null, string Name = null)
		{
			return ServiceProvider.NewResolver().ResolveServices(
				ScopeServiceId,
				typeof(T),
				Name
				).Cast<T>();
		}
		public static IEnumerable<object> GetServices(this IServiceProvider ServiceProvider,Type ServiceType,long? ScopeServiceId=null,string Name=null)
		{
			return ServiceProvider.NewResolver().ResolveServices(
				ScopeServiceId,
				ServiceType,
				Name
				);
		}
	}
}
