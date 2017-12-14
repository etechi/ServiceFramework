#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SF.Sys.Reflection;
using SF.Sys.Linq.Expressions;

namespace SF.Sys.Services
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
				throw new InvalidOperationException($"找不到服务:{typeof(T)},名称:{Name},范围:{ScopeId}");
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


		
		public static async Task WithScope(this IServiceProvider sp, Func<IServiceProvider, Task> action)
		{
			using (var s = sp.Resolve<IServiceScopeFactory>().CreateServiceScope())
				await action(s.ServiceProvider);
		}
		public static async Task<T> WithScope<T>(this IServiceProvider sp, Func<IServiceProvider, Task<T>> action)
		{
			using (var s = sp.Resolve<IServiceScopeFactory>().CreateServiceScope())
				return await action(s.ServiceProvider);
		}
		

		public static T WithScope<T>(this IServiceProvider sp, Func<IServiceProvider, T> action)
		{
			using (var s = sp.Resolve<IServiceScopeFactory>().CreateServiceScope())
				return action(s.ServiceProvider);
		}
		public static void WithScope(this IServiceProvider sp, Action<IServiceProvider> action)
		{
			using (var s = sp.Resolve<IServiceScopeFactory>().CreateServiceScope())
				action(s.ServiceProvider);
		}

	
		public static async Task<T> WithScopedServices<S, T>(this IServiceProvider sp, Func<S, Task<T>> action)
		{
			using (var s = sp.Resolve<IServiceScopeFactory>().CreateServiceScope())
				return await s.ServiceProvider.WithServices(action);
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
		static Func<Func<T, R>, IServiceResolver,long?,R> GetDelegateInvoker<T,R>()
		{
			var typeResolver = typeof(IServiceResolver);
			var ArgFunc= Expression.Parameter(typeof(Func<T, R>));
			var ArgServiceProvider = Expression.Parameter(typeResolver);
			var ArgScopeId = Expression.Parameter(typeof(long?));
			var MethodResolve = typeResolver.GetMethod(nameof(IServiceResolver.ResolveServiceByType), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Public);
			var typeArgument = typeof(T);
			var arg=
				typeArgument.IsGeneric() && typeArgument.GetGenericTypeDefinition().Name.StartsWith("ValueTuple`")?
				Expression.New(
					typeArgument.GetConstructor(typeArgument.GenericTypeArguments),
					typeArgument.GenericTypeArguments.Select(
						t=>ArgServiceProvider.CallMethod(
							MethodResolve,
							ArgScopeId,
							Expression.Constant(t),
							Expression.Constant(null, typeof(string))
							).To(t)
						)
					)
				:ArgServiceProvider.CallMethod(
					MethodResolve,
					ArgScopeId,
					Expression.Constant(typeArgument),
					Expression.Constant(null, typeof(string))
					).To(typeArgument)
				;
			var func = Expression.Lambda<Func<Func<T,R>,IServiceResolver,long?,R>>(
				Expression.Invoke(
					ArgFunc,
					arg
					),
				ArgFunc,
				ArgServiceProvider,
				ArgScopeId
				).Compile();
			return func;
		}
		static System.Collections.Concurrent.ConcurrentDictionary<(Type,Type), Lazy<Delegate>> Invokers = 
			new System.Collections.Concurrent.ConcurrentDictionary<(Type, Type), Lazy<Delegate>>();
		public static R WithServices<T,R>(this IServiceProvider ServiceProvider, Func<T,R> Func,long? ScopeId=null)
		{
			var key = (typeof(T), typeof(R));
			if (!Invokers.TryGetValue(key, out var l))
				l = Invokers.GetOrAdd(key, new Lazy<Delegate>(() => GetDelegateInvoker<T,R>()));
			var invoke=(Func< Func <T, R >, IServiceResolver, long ?, R>)l.Value;
			return invoke(Func, ServiceProvider.Resolver(),ScopeId);
		}
		public static void WithServices<T>(this IServiceProvider ServiceProvider, Action<T> Func, long? ScopeId)
		{
			ServiceProvider.WithServices((T a) =>
			{
				Func(a);
				return 0;
			},
			ScopeId);
		}
	}
}
