using System;
using System.Collections.Generic;
using System.Reflection;
namespace SF.Reflection
{
	public static class TypeExtensions
	{
		public static bool IsInterfaceType(this Type Type)
		{
			return Type.GetTypeInfo().IsInterface;
		}
		public static bool IsAssignableFrom(this Type dst, Type src)
		{
			return dst.GetTypeInfo().IsAssignableFrom(src.GetTypeInfo());
		}
		public static T GetCustomAttribute<T>(this Type type)
			where T:Attribute
		{
			return type.GetTypeInfo().GetCustomAttribute<T>();
		}
		public static bool IsValue(this Type Type)
		{
			return Type.GetTypeInfo().IsValueType;
		}
		public static IEnumerable<Attribute> GetCustomAttributes(this Type type,bool inherit)
		{
			return type.GetTypeInfo().GetCustomAttributes(inherit);
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

	}
}
