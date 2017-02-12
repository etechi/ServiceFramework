using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;

namespace System.Reflection
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
		public static Type GetTypedGenericArgument(this Type Type,Type GenericDefination)
		{
			if (Type.IsGeneric() && Type.GetGenericTypeDefinition() == GenericDefination)
				return Type.GetGenericArguments()[0];
			return null;
		}
		public static Type GetGenericArgumentTypeAsTask(this Type type) =>
			GetTypedGenericArgument(type, typeof(Task<>));
		public static Type GetGenericArgumentTypeAsLazy(this Type type) =>
			GetTypedGenericArgument(type, typeof(Lazy<>));
		public static Type GetGenericArgumentTypeAsFunc(this Type type) =>
			GetTypedGenericArgument(type, typeof(Func<>));

		/// <summary>
		/// Search for a method by name and parameter types.  
		/// Unlike GetMethod(), does 'loose' matching on generic
		/// parameter types, and searches base interfaces.
		/// </summary>
		/// <exception cref="AmbiguousMatchException"/>
		public static MethodInfo GetMethodExt(this Type thisType,
												string name,
												params Type[] parameterTypes)
		{
			return GetMethodExt(thisType,
								name,
								BindingFlags.Instance
								| BindingFlags.Static
								| BindingFlags.Public
								| BindingFlags.NonPublic
								| BindingFlags.FlattenHierarchy,
								parameterTypes);
		}

		/// <summary>
		/// Search for a method by name, parameter types, and binding flags.  
		/// Unlike GetMethod(), does 'loose' matching on generic
		/// parameter types, and searches base interfaces.
		/// </summary>
		/// <exception cref="AmbiguousMatchException"/>
		public static MethodInfo GetMethodExt(this Type thisType,
												string name,
												BindingFlags bindingFlags,
												params Type[] parameterTypes)
		{
			MethodInfo matchingMethod = null;

			// Check all methods with the specified name, including in base classes
			GetMethodExt(ref matchingMethod, thisType, name, bindingFlags, parameterTypes);

			// If we're searching an interface, we have to manually search base interfaces
			if (matchingMethod == null && thisType.IsInterface)
			{
				foreach (Type interfaceType in thisType.GetInterfaces())
					GetMethodExt(ref matchingMethod,
								 interfaceType,
								 name,
								 bindingFlags,
								 parameterTypes);
			}

			return matchingMethod;
		}

		private static void GetMethodExt(ref MethodInfo matchingMethod,
											Type type,
											string name,
											BindingFlags bindingFlags,
											params Type[] parameterTypes)
		{
			// Check all methods with the specified name, including in base classes
			foreach (MethodInfo methodInfo in type.GetMember(name,
															 MemberTypes.Method,
															 bindingFlags))
			{
				// Check that the parameter counts and types match, 
				// with 'loose' matching on generic parameters
				ParameterInfo[] parameterInfos = methodInfo.GetParameters();
				if (parameterInfos.Length == parameterTypes.Length)
				{
					int i = 0;
					for (; i < parameterInfos.Length; ++i)
					{
						if (!parameterInfos[i].ParameterType
											  .IsSimilarType(parameterTypes[i]))
							break;
					}
					if (i == parameterInfos.Length)
					{
						if (matchingMethod == null)
							matchingMethod = methodInfo;
						else
							throw new AmbiguousMatchException(
								   "More than one matching method found!");
					}
				}
			}
		}

		/// <summary>
		/// Special type used to match any generic parameter type in GetMethodExt().
		/// </summary>
		public class T
		{ }

		/// <summary>
		/// Determines if the two types are either identical, or are both generic 
		/// parameters or generic types with generic parameters in the same
		///  locations (generic parameters match any other generic paramter,
		/// but NOT concrete types).
		/// </summary>
		private static bool IsSimilarType(this Type thisType, Type type)
		{
			// Ignore any 'ref' types
			if (thisType.IsByRef)
				thisType = thisType.GetElementType();
			if (type.IsByRef)
				type = type.GetElementType();

			// Handle array types
			if (thisType.IsArray && type.IsArray)
				return thisType.GetElementType().IsSimilarType(type.GetElementType());

			// If the types are identical, or they're both generic parameters 
			// or the special 'T' type, treat as a match
			if (thisType == type || ((thisType.IsGenericParameter || thisType == typeof(T))
								 && (type.IsGenericParameter || type == typeof(T))))
				return true;

			// Handle any generic arguments
			if (thisType.IsGenericType && type.IsGenericType)
			{
				Type[] thisArguments = thisType.GetGenericArguments();
				Type[] arguments = type.GetGenericArguments();
				if (thisArguments.Length == arguments.Length)
				{
					for (int i = 0; i < thisArguments.Length; ++i)
					{
						if (!thisArguments[i].IsSimilarType(arguments[i]))
							return false;
					}
					return true;
				}
			}

			return false;
		}
	}
}
