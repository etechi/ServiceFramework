using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq.Expressions;
namespace SF.Core.Interception
{
	public class ProxyInvokerFactory : IProxyInvokerFactory
	{
		static Lazy<Func<object,object[],object>> BuildMethodInvoker(MethodInfo Method)
		{
			return new Lazy<Func<object, object[], object>>(() =>
			{
				var argIns = Expression.Parameter(typeof(object), "ins");
				var argArgs = Expression.Parameter(typeof(object[]), "args");
				var ps = Method.GetParameters();
				return Expression.Lambda<Func<object, object[], object>>(
					Expression.Convert(
						Expression.Call(
							Expression.Convert(argIns, Method.DeclaringType),
							Method,
							ps.Select((p, i) =>
							{
								var v = Expression.ArrayIndex(
									argArgs,
									Expression.Constant(i)
									);
								if (p.ParameterType.IsValueType)
									return (Expression)Expression.Unbox(v, p.ParameterType);
								else
									return (Expression)Expression.Convert(v, p.ParameterType);
							})
						),
						typeof(object)
					),

					argIns,
					argArgs
				).Compile();
			});
		}
		static void CollectMethods(Type InterfaceType, HashSet<MethodInfo> methods)
		{
			foreach (var m in InterfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy))
				methods.Add(m);
			foreach (var it in InterfaceType.GetInterfaces())
				CollectMethods(it, methods);
		}
		class Behavior : IInterceptionBehavior
		{
			Func<object,IMethodInvocation, IMethodReturn> Invoker { get; }
			object Target { get; }
			public Behavior(object Target, Func<object,IMethodInvocation, IMethodReturn> Invoker)
			{
				this.Target = Target;
				this.Invoker = Invoker;
			}
			public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
			{
				return Invoker(Target, input);
			}
		}
		static System.Collections.Concurrent.ConcurrentDictionary<Type, Func<object, IMethodInvocation, IMethodReturn>> Invokers { get; } = new System.Collections.Concurrent.ConcurrentDictionary<Type, Func<object, IMethodInvocation, IMethodReturn>>();
		static Func<object,IMethodInvocation,IMethodReturn> Build(Type InterfaceType)
		{
			var methods = new HashSet<MethodInfo>();
			CollectMethods(InterfaceType, methods);
			var dic = (from m in methods
					   let invoker = BuildMethodInvoker(m)
					   select new { method = m, invoker = invoker }
					).ToDictionary(p => p.method, p => p.invoker);

			return (target, invocation) =>
			{
				var m = invocation.MethodBase as MethodInfo;
				if (m == null)
					throw new NotSupportedException("不支持方法:" + invocation.MethodBase);
				var invoker = dic.Get(m);
				if (invoker == null)
					throw new NotSupportedException("不支持方法:" + invocation.MethodBase);
				try
				{
					var re = invoker.Value(target, invocation.RawArguments);

					return invocation.CreateMethodReturn(re);
				}
				catch (Exception ex)
				{
					return invocation.CreateExceptionMethodReturn(ex);
				}
			};
		}
		static Func<Type, Func<object, IMethodInvocation, IMethodReturn>> Builder { get; } = Build;
		public IInterceptionBehavior Create(Type InterfaceType,object Target)
		{
			return new Behavior(
				Target,
				Invokers.GetOrAdd(InterfaceType, Builder)
				);
		}
	}


}
