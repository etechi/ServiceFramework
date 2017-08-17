using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
namespace SF.Core.ServiceManagement
{
	public static class ServiceResolverExtension
	{
		static Type TypeServiceResolver { get; } = typeof(IServiceResolver);
		public static IServiceResolver Resolver(this IServiceProvider ServiceProvider)
			=> (IServiceResolver)ServiceProvider.GetService(TypeServiceResolver);

		public static T TryResolve<T>(this IServiceResolver ServiceResolver,string Name=null,long? ScopeId=null)
			where T:class
			=>(T)ServiceResolver.ResolveServiceByType(ScopeId,typeof(T),Name);

		public static T TryResolve<T>(this IServiceProvider ServiceProvider, string Name = null, long? ScopeId = null)
			where T : class
			=> ServiceProvider.Resolver().TryResolve<T>(Name, ScopeId);

		public static T Resolve<T>(this IServiceResolver ServiceResolver, string Name = null, long? ScopeId = null)
			where T : class
		{
			var re = (T)ServiceResolver.ResolveServiceByType(ScopeId, typeof(T), Name);
			if (re == null)
				throw new InvalidOperationException("找不到服务:" + typeof(T));
			return re;
		}
		public static T Resolve<T>(this IServiceProvider ServiceProvider, string Name = null, long? ScopeId = null)
			where T:class
			=> ServiceProvider.Resolver().Resolve<T>(Name, ScopeId);


		public static T Resolve<T>(this IServiceResolver ServiceResolver, long ServiceId)
			where T : class
		{
			var s = ServiceResolver.ResolveServiceByIdent(ServiceId, typeof(T));
			if (s == null)
				throw new InvalidOperationException("找不到服务:" + typeof(T));
			var re = s as T;
			if (re == null)
				throw new InvalidOperationException($"服务:{ServiceId}不是类型:{typeof(T)}");
			return re;
		}

		public static T ResolveInternal<T>(this IServiceProvider ServiceProvider, long ServiceId,string Name=null)
			where T : class
			=> ServiceProvider.Resolve<T>(Name, ServiceId);

		public static T Resolve<T>(this IServiceProvider ServiceProvider, long ServiceId)
			where T : class
			=> ServiceProvider.Resolver().Resolve<T>(ServiceId);


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
		public static Task<T> WithScope<A0, T>(this IServiceProvider sp, Func<A0, Task<T>> action)
			=> sp.WithDelegateScope<T>(action);

		public static Task<T> WithScope<A0, A1, T>(this IServiceProvider sp, Func<A0, A1, Task<T>> action)
			=> sp.WithDelegateScope<T>(action);

		public static Task<T> WithScope<A0, A1, A2, T>(this IServiceProvider sp, Func<A0, A1, A2, Task<T>> action)
			=> sp.WithDelegateScope<T>(action);

		public static Task<T> WithScope<A0, A1, A2, A3, T>(this IServiceProvider sp, Func<A0, A1, A2, A3, Task<T>> action)
			=> sp.WithDelegateScope<T>(action);

		public static async Task<T> WithDelegateScope<T>(this IServiceProvider sp, Delegate action)
		{
			using (var s = sp.Resolve<IServiceScopeFactory>().CreateServiceScope())
			{
				var sv = s.ServiceProvider.Resolver();
				var args = action.Method.GetParameters()
					.Select(p => sv.ResolveServiceByType(null,p.ParameterType,null))
					.ToArray();
				return await (Task<T>)action.DynamicInvoke(args);
			}
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
			return ServiceProvider.Resolver().ResolveServices(
				ScopeServiceId,
				typeof(T),
				Name
				).Cast<T>();
		}
		public static IEnumerable<object> GetServices(this IServiceProvider ServiceProvider,Type ServiceType,long? ScopeServiceId=null,string Name=null)
		{
			return ServiceProvider.Resolver().ResolveServices(
				ScopeServiceId,
				ServiceType,
				Name
				);
		}
	}
}
