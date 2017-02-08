using SF.Core.DI;
using SF.Metadata;
using SF.Services.ManagedServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http.Dispatcher;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;

namespace SF.AspNet.NetworkService
{

	partial class ServiceActionDescriptor 
	{
		
		private sealed class ActionExecutor
		{
			private readonly Func<object, object[], Task<object>> _executor;

			private static MethodInfo _convertOfTMethod = typeof(ActionExecutor).GetMethod("Convert", BindingFlags.Static | BindingFlags.NonPublic);

			public ActionExecutor(MethodInfo methodInfo)
			{
				this._executor = ActionExecutor.GetExecutor(methodInfo);
			}

			public Task<object> Execute(object instance, object[] arguments)
			{
				return this._executor(instance, arguments);
			}

			private static async Task<object> Convert<T>(object taskAsObject)
			{
				return await (Task<T>)taskAsObject;
			}

			[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
			private static Func<object, Task<object>> CompileGenericTaskConversionDelegate(Type taskValueType)
			{
				return (Func<object, Task<object>>)Delegate.CreateDelegate(
					typeof(Func<object, Task<object>>),
					_convertOfTMethod.MakeGenericMethod(new Type[]
					{
						taskValueType
					}));
			}
			static readonly Type TaskGenericType = typeof(Task<>);
			static Type GetTaskInnerTypeOrNull(Type type)
			{
				if (type.IsGenericType && !type.IsGenericTypeDefinition)
				{
					Type genericTypeDefinition = type.GetGenericTypeDefinition();
					if (TaskGenericType == genericTypeDefinition)
					{
						return type.GetGenericArguments()[0];
					}
				}
				return null;
			}
			private static Func<object, object[], Task<object>> GetExecutor(MethodInfo methodInfo)
			{
				var argInstance = Expression.Parameter(typeof(object), "instance");
				var argParameters = Expression.Parameter(typeof(object[]), "parameters");
				var parameters = methodInfo.GetParameters();
				var args = parameters.Select((p, i) =>
					  Expression.Convert(
						  Expression.ArrayIndex(
							  argParameters,
							  Expression.Constant(i)
							  ),
						  p.ParameterType
						  )
					).ToArray();

				var instance = !methodInfo.IsStatic ? Expression.Convert(argInstance, methodInfo.ReflectedType) : null;
				var methodCallExpression = Expression.Call(instance, methodInfo, args);
				if (methodCallExpression.Type == typeof(void))
				{
					var voidExecutor = Expression.Lambda<Action<object, object[]>>(
							methodCallExpression,
							argInstance,
							argParameters
							).Compile();
					return (ins, methodParameters) =>
					{
						voidExecutor(instance, methodParameters);
						return Task.FromResult((object)null);
					};
				}
				var compiled = Expression.Lambda<Func<object, object[], object>>(
					Expression.Convert(methodCallExpression, typeof(object)),
					argInstance,
					argParameters
					)
					.Compile();
				if (methodCallExpression.Type == typeof(Task))
				{
					return async (ins, methodParameters) =>
					{
						var task = (Task)compiled(ins, methodParameters);
						ThrowIfWrappedTaskInstance(methodInfo, task.GetType());
						await task;
						return null;
					};
				}
				if (typeof(Task).IsAssignableFrom(methodCallExpression.Type))
				{
					Type taskInnerTypeOrNull = GetTaskInnerTypeOrNull(methodCallExpression.Type);
					var compiledConversion = CompileGenericTaskConversionDelegate(taskInnerTypeOrNull);
					return (ins, methodParameters) =>
					{
						object arg = compiled(ins, methodParameters);
						return compiledConversion(arg);
					};
				}
				return (ins, methodParameters) =>
				{
					var obj = compiled(ins, methodParameters);
					Task task = obj as Task;
					if (task != null)
						throw new InvalidOperationException();
					return Task.FromResult<object>(obj);
				};
			}

			private static void ThrowIfWrappedTaskInstance(MethodInfo method, Type type)
			{
				if (type != typeof(Task))
				{
					Type taskInnerTypeOrNull = GetTaskInnerTypeOrNull(type);
					if (taskInnerTypeOrNull != null && typeof(Task).IsAssignableFrom(taskInnerTypeOrNull))
						throw new InvalidOperationException();
				}
			}
		}
	}

}
