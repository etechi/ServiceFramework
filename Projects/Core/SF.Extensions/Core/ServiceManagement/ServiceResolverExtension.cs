using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SF.Core.ServiceManagement
{
	public static class ServiceResolverExtension
	{
		static Type TypeServiceResolver { get; } = typeof(IServiceResolver);
		public static IServiceResolver Resolver(this IServiceProvider ServiceProvider)
			=> (IServiceResolver)ServiceProvider.GetService(TypeServiceResolver);

		public static T TryResolve<T>(this IServiceResolver ServiceResolver, string Name = null, long? ScopeId = null)
			where T : class
			=> (T)ServiceResolver.ResolveServiceByType(ScopeId, typeof(T), Name);

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
			where T : class
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

		public static T ResolveInternal<T>(this IServiceProvider ServiceProvider, long ServiceId, string Name = null)
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
					.Select(p => sv.ResolveServiceByType(null, p.ParameterType, null))
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
		public static IEnumerable<object> GetServices(this IServiceProvider ServiceProvider, Type ServiceType, long? ScopeServiceId = null, string Name = null)
		{
			return ServiceProvider.Resolver().ResolveServices(
				ScopeServiceId,
				ServiceType,
				Name
				);
		}
		static Type[] FuncTypes { get; } = new[]
		{
			typeof(Func<>),
			typeof(Func<,>),
			typeof(Func<,,>),
			typeof(Func<,,>),
			typeof(Func<,,,>),
			typeof(Func<,,,,>),
			typeof(Func<,,,,,>),
			typeof(Func<,,,,,,>),
			typeof(Func<,,,,,,,>),
			typeof(Func<,,,,,,,,>),
			typeof(Func<,,,,,,,,,>),
			typeof(Func<,,,,,,,,,,>),
			typeof(Func<,,,,,,,,,,,>),
			typeof(Func<,,,,,,,,,,,,>),
			typeof(Func<,,,,,,,,,,,,,>),
			typeof(Func<,,,,,,,,,,,,,,>),
			typeof(Func<,,,,,,,,,,,,,,,>),
			typeof(Func<,,,,,,,,,,,,,,,,>)
		};
		static Type[] ActionTypes { get; } = new[]
		{
			typeof(Action<>),
			typeof(Action<,>),
			typeof(Action<,,>),
			typeof(Action<,,>),
			typeof(Action<,,,>),
			typeof(Action<,,,,>),
			typeof(Action<,,,,,>),
			typeof(Action<,,,,,,>),
			typeof(Action<,,,,,,,>),
			typeof(Action<,,,,,,,,>),
			typeof(Action<,,,,,,,,,>),
			typeof(Action<,,,,,,,,,,>),
			typeof(Action<,,,,,,,,,,,>),
			typeof(Action<,,,,,,,,,,,,>),
			typeof(Action<,,,,,,,,,,,,,>),
			typeof(Action<,,,,,,,,,,,,,,>),
			typeof(Action<,,,,,,,,,,,,,,,>),
		};
		static Action<IServiceResolver> GetDelegateInvoker(Delegate Delegate)
		{
			var typeResolver = typeof(IServiceResolver);
			var ArgServiceProvider = Expression.Parameter(typeResolver);
			var MethodResolve = typeResolver.GetMethod("ResolveDescriptorByType", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Public);
			var ps = Delegate.Method.GetParameters();
			var args = ps.Select(p =>
				ArgServiceProvider.CallMethod(
					MethodResolve,
					Expression.Constant(null, typeof(long?)),
					Expression.Constant(p.ParameterType),
					Expression.Constant(null, typeof(string))
					)
				);
			var func = Expression.Lambda<Action<IServiceResolver>>(
				Expression.Invoke(
					Expression.Constant(
						Delegate,
						ps.Length == 0 ? typeof(Action) :
						ActionTypes[ps.Length - 1].MakeGenericType(
						ps.Select(p => p.ParameterType)
						)
					),
					args
					),
				ArgServiceProvider
				).Compile();
			return func;
		}
		static System.Collections.Concurrent.ConcurrentDictionary<MethodInfo, Lazy<Action<IServiceResolver>>> Invokers = new System.Collections.Concurrent.ConcurrentDictionary<MethodInfo, Lazy<Action<IServiceResolver>>>();
		public static void RawInvoke(this IServiceProvider ServiceProvider, Delegate Delegate)
		{
			if (!Invokers.TryGetValue(Delegate.Method, out var l))
				l = Invokers.GetOrAdd(Delegate.Method, new Lazy<Action<IServiceResolver>>(() => GetDelegateInvoker(Delegate)));
			l.Value(ServiceProvider.Resolver());
		}
		public static void Invoke<I0, I1, I2, I3, I4, I5, I6, I7>(this IServiceProvider ServiceProvider, Action<I0, I1, I2, I3, I4, I5, I6, I7> callback) =>
			ServiceProvider.RawInvoke((Delegate)callback);
		public static void Invoke<I0, I1, I2, I3, I4, I5, I6>(this IServiceProvider ServiceProvider, Action<I0, I1, I2, I3, I4, I5, I6> callback) =>
			ServiceProvider.RawInvoke((Delegate)callback);
		public static void Invoke<I0, I1, I2, I3, I4, I5>(this IServiceProvider ServiceProvider, Action<I0, I1, I2, I3, I4, I5> callback) =>
			ServiceProvider.RawInvoke((Delegate)callback);
		public static void Invoke<I0, I1, I2, I3, I4>(this IServiceProvider ServiceProvider, Action<I0, I1, I2, I3, I4> callback) =>
			ServiceProvider.RawInvoke((Delegate)callback);
		public static void Invoke<I0, I1, I2, I3>(this IServiceProvider ServiceProvider, Action<I0, I1, I2, I3> callback) =>
			ServiceProvider.RawInvoke((Delegate)callback);
		public static void Invoke<I0, I1, I2>(this IServiceProvider ServiceProvider, Action<I0, I1, I2> callback) =>
			ServiceProvider.RawInvoke((Delegate)callback);
		public static void Invoke<I0, I1>(this IServiceProvider ServiceProvider, Action<I0, I1> callback) =>
			ServiceProvider.RawInvoke((Delegate)callback);
		public static void Invoke<I0>(this IServiceProvider ServiceProvider, Action<I0> callback) =>
			ServiceProvider.RawInvoke((Delegate)callback);

		public static R Invoke<I0, I1, I2, I3, I4, I5, I6, I7, R>(this IServiceProvider ServiceProvider, Func<I0, I1, I2, I3, I4, I5, I6, I7, R> callback)
		{
			R re=default(R);
			ServiceProvider.Invoke<I0, I1, I2, I3, I4, I5, I6, I7>((i0, i1, i2, i3, i4, i5, i6, i7)=>{
				re =callback(i0, i1, i2, i3, i4, i5, i6, i7); }
			);
			return re;
		}
		public static R Invoke<I0, I1, I2, I3, I4, I5, I6, R>(this IServiceProvider ServiceProvider, Func<I0, I1, I2, I3, I4, I5, I6, R> callback)
		{
			R re = default(R);
			ServiceProvider.Invoke<I0, I1, I2, I3, I4, I5, I6>((i0, i1, i2, i3, i4, i5, i6) => {
				re = callback(i0, i1, i2, i3, i4, i5, i6);
			}
			);
			return re;
		}
		public static R Invoke<I0, I1, I2, I3, I4, I5, R>(this IServiceProvider ServiceProvider, Func<I0, I1, I2, I3, I4, I5, R> callback)
		{
			R re = default(R);
			ServiceProvider.Invoke<I0, I1, I2, I3, I4, I5>((i0, i1, i2, i3, i4, i5) => {
				re = callback(i0, i1, i2, i3, i4, i5);
			}
			);
			return re;
		}
		public static R Invoke<I0, I1, I2, I3, I4, R>(this IServiceProvider ServiceProvider, Func<I0, I1, I2, I3, I4, R> callback)
		{
			R re = default(R);
			ServiceProvider.Invoke<I0, I1, I2, I3, I4>((i0, i1, i2, i3, i4) => {
				re = callback(i0, i1, i2, i3, i4);
			}
			);
			return re;
		}
		public static R Invoke<I0, I1, I2, I3, R>(this IServiceProvider ServiceProvider, Func<I0, I1, I2, I3, R> callback)
		{
			R re = default(R);
			ServiceProvider.Invoke<I0, I1, I2, I3>((i0, i1, i2, i3) => {
				re = callback(i0, i1, i2, i3);
			}
			);
			return re;
		}
		public static R Invoke<I0, I1, I2, R>(this IServiceProvider ServiceProvider, Func<I0, I1, I2, R> callback)
		{
			R re = default(R);
			ServiceProvider.Invoke<I0, I1, I2>((i0, i1, i2) => {
				re = callback(i0, i1, i2);
			}
			);
			return re;
		}
		public static R Invoke<I0, I1, R>(this IServiceProvider ServiceProvider, Func<I0, I1, R> callback)
		{
			R re = default(R);
			ServiceProvider.Invoke<I0, I1>((i0, i1) => {
				re = callback(i0, i1);
			}
			);
			return re;
		}
		public static R Invoke<I0, R>(this IServiceProvider ServiceProvider, Func<I0, R> callback)
		{
			R re = default(R);
			ServiceProvider.Invoke<I0>((i0) => {
				re = callback(i0);
			}
			);
			return re;
		}
	}
}
