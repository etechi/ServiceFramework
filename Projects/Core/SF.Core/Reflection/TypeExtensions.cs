using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
namespace SF.Reflection
{
	public static class TypeExtensions
	{
		public static bool IsInterfaceType(this Type Type)
		{
			return Type.GetTypeInfo().IsInterface;
		}
		//public static bool IsAssignableFrom(this Type dst, Type src)
		//{
		//	return dst.GetTypeInfo().IsAssignableFrom(src.GetTypeInfo());
		//}
		//public static MethodInfo GetMethod(this Type type,string Name,BindingFlags Flags)
		//{
		//	return type.GetTypeInfo().GetMethod(Name, Flags);
		//}
		//public static PropertyInfo GetProperty(this Type type, string Name)
		//{
		//	return type.GetTypeInfo().GetProperty(Name);
		//}
		public static bool IsValue(this Type Type)
		{
			return Type.GetTypeInfo().IsValueType;
		}

#if NETCORE
		public static T GetCustomAttribute<T>(this Type type)
			where T:Attribute
		{
			var ti = type.GetTypeInfo();
			return ti.GetCustomAttribute<T>();
		}
#endif
		public static IEnumerable<Attribute> GetCustomAttributes(this Type type,bool inherit)
		{
			return type.GetTypeInfo().GetCustomAttributes(inherit).Cast<Attribute>();
		}
		public static IEnumerable<T> GetCustomAttributes<T>(this Type type, bool inherit)
		{
			return type.GetTypeInfo().GetCustomAttributes(inherit).Where(a=>a is T).Cast<T>();
		}
		public static T GetCustomAttribute<T>(this Type type, bool inherit=true) where T:Attribute
		{
			return (T)type.GetTypeInfo().GetCustomAttributes(inherit).FirstOrDefault(a=>a is T);
		}
		public static bool IsEnumType(this Type type)
		{
			return type.GetTypeInfo().IsEnum;
		}
		public static bool IsGeneric(this Type type)
		{
			return type.GetTypeInfo().IsGenericType;
		}
		public static bool IsGenericDefinition(this Type type)
		{
			return type.GetTypeInfo().IsGenericTypeDefinition;
		}
		public static Type GetBaseType(this Type type)
		{
			return type.GetTypeInfo().BaseType;
		}
		public static bool IsAbstractType(this Type type)
		{
			return type.GetTypeInfo().IsAbstract;
		}
		public static bool IsPublicType(this Type type)
		{
			return type.GetTypeInfo().IsPublic;
		}

		public static Assembly GetAssembly(this Type type)
		{
			return type.GetTypeInfo().Assembly;
		}
		public static Type GetInterface(this Type type,string Name)
		{
			return type.GetTypeInfo().GetInterface(Name);
		}
	}
}
