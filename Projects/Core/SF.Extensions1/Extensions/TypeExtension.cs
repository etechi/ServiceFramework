using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SF
{
	public static class TypeExtension
	{
		static System.Collections.Concurrent.ConcurrentDictionary<string, Func<object[],object>> Creators { get; } 
			= new System.Collections.Concurrent.ConcurrentDictionary<string, Func<object[], object>>();
		static string GetCreatorKey (Type type,params Type[] arg_types)
		{
			return type.FullName + ":" + arg_types.Select(at => at.FullName).Join(":");
		}
		static Func<object[],object> BuildCreator(Type type, params Type[] argTypes)
		{
			var args = Expression.Parameter(typeof(object[]));
			var constructor=type.GetConstructor(
				System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance, 
				null, 
				argTypes,
				null
				);
			if (constructor == null)
				throw new ArgumentException($"找不到{type}的构造函数，参数：{argTypes.Select(t => t.FullName).Join(",")}");
			return Expression.Lambda<Func<object[], object>>(
				Expression.New(
					constructor,
					argTypes.Select((t, i) =>
						{
							var arg = Expression.ArrayAccess(args, Expression.Constant(i));
							if (t.IsValueType)
								return Expression.Unbox(arg, t);
							else
								return Expression.Convert(arg, t);
						}
					)
				),
				args
				).Compile();
		}
		public static object CreateInstance(this Type type)
		{
			var key = GetCreatorKey(type);
			var creator=Creators.GetOrAdd(key, k => BuildCreator(type,Empty.Array<Type>()));
			return creator(Empty.Array<object>());
		}
		public static object CreateInstance(this Type type, Type[] argTypes,object[] args)
		{
			var key = GetCreatorKey(type, argTypes);
			var creator = Creators.GetOrAdd(key, k => BuildCreator(type, argTypes));
			return creator(args);
		}
		public static object CreateInstance<A1>(this Type type, A1 arg1)
		{
			return type.CreateInstance(new[] { typeof(A1) }, new object[] { arg1 });
		}
		public static object CreateInstance<A1,A2>(this Type type, A1 arg1, A2 arg2)
		{
			return type.CreateInstance(new[] { typeof(A1), typeof(A2) }, new object[] { arg1 ,arg2});
		}
		public static object CreateInstance<A1, A2, A3>(this Type type, A1 arg1, A2 arg2, A3 arg3)
		{
			return type.CreateInstance(new[] { typeof(A1), typeof(A2), typeof(A3) }, new object[] { arg1, arg2, arg3});
		}
		public static object CreateInstance<A1, A2, A3,A4>(this Type type, A1 arg1, A2 arg2, A3 arg3, A4 arg4)
		{
			return type.CreateInstance(new[] { typeof(A1), typeof(A2), typeof(A3), typeof(A4) }, new object[] { arg1, arg2, arg3, arg4});
		}
	}
}
