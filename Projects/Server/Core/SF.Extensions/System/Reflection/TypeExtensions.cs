using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;

namespace System.Reflection
{
	public static class TypeExtension
	{
		static Dictionary<Type, TypeCode> codes { get; } = new Dictionary<Type, System.TypeCode>
		{
			{typeof(void),System.TypeCode.Empty},
			{typeof(bool),System.TypeCode.Boolean },
			{typeof(char),System.TypeCode.Char },
			{typeof(sbyte),System.TypeCode.SByte },
			{typeof(byte),System.TypeCode.Byte },
			{typeof(short),System.TypeCode.Int16 },
			{typeof(ushort),System.TypeCode.UInt16 },
			{typeof(int),System.TypeCode.Int32 },
			{typeof(uint),System.TypeCode.UInt32 },
			{typeof(long),System.TypeCode.Int64 },
			{typeof(ulong),System.TypeCode.UInt64 },
			{typeof(float),System.TypeCode.Single },
			{typeof(double),System.TypeCode.Double },
			{typeof(decimal),System.TypeCode.Decimal },
			{typeof(DateTime),System.TypeCode.DateTime },
			{typeof(string),System.TypeCode.String }
		};

		public static string GetFullName(this Type type)
		{
			if (!type.IsGenericType)
				return type.FullName;
			return type.GetGenericTypeDefinition().FullName + "<" +
				type.GetGenericArguments().Select(t=>t.GetFullName()).Join(",")+
				">";
		}
		public static IEnumerable<MemberInfo> GetInterfaceMembers(
			this Type type,
			BindingFlags flags=BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.InvokeMethod 
			)
		{
			if (type.IsInterfaceType())
			{
				foreach (var m in type.GetMembers(flags))
					yield return m;
			}
			foreach (var it in type.GetInterfaces())
				foreach (var m in it.GetInterfaceMembers(flags))
					yield return m;
		}
		public static TypeCode GetTypeCode(this Type type)
		{
#if NETCORE
			TypeCode tc;
			if (codes.TryGetValue(type??typeof(void), out tc))
				return tc;
			return System.TypeCode.Object;

#else
			return System.Type.GetTypeCode(type);
#endif

		}
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
		public static IEnumerable<Attribute> GetCustomAttributes(this Type type, bool inherit)
		{
			return type.GetTypeInfo().GetCustomAttributes(inherit).Cast<Attribute>();
		}
		public static IEnumerable<T> GetCustomAttributes<T>(this Type type, bool inherit)
		{
			return type.GetTypeInfo().GetCustomAttributes(inherit).Where(a => a is T).Cast<T>();
		}
		public static T GetCustomAttribute<T>(this Type type, bool inherit = true) where T : Attribute
		{
			return (T)type.GetTypeInfo().GetCustomAttributes(inherit).FirstOrDefault(a => a is T);
		}
		public static bool IsEnumType(this Type type)
		{
			return type.GetTypeInfo().IsEnum;
		}
		public static bool IsGeneric(this Type type)
		{
			return type.GetTypeInfo().IsGenericType;
		}
		public static bool IsClassType(this Type type)
		{
			return type.GetTypeInfo().IsClass;
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
		public static bool IsPrimitiveType(this Type type)
		{
			return type.GetTypeInfo().IsPrimitive;
		}
		static HashSet<Type> SysTypes { get; } = new HashSet<Type>
		{
			typeof(string),
			typeof(DateTime),
			typeof(TimeSpan),
		};
		public static bool IsSystemType(this Type type)
		{
			if (type.IsPrimitiveType())
				return true;

			return SysTypes.Contains(type);
		}

		public static Assembly GetAssembly(this Type type)
		{
			return type.GetTypeInfo().Assembly;
		}
		public static Type GetInterface(this Type type, string Name)
		{
			return type.GetTypeInfo().ImplementedInterfaces.SingleOrDefault(t => t.Name == Name);
			//.GetTypeInfo().GetInterface(Name);
		}

		static System.Collections.Concurrent.ConcurrentDictionary<Type, object> defaultValues { get; } = new Collections.Concurrent.ConcurrentDictionary<Type, object>();
		static object GetDefaultValue<T>()
		{
			return default(T);
		}
		static MethodInfo GetDefaultValueMethod { get; } = typeof(TypeExtension).GetMethodExt(
			nameof(GetDefaultValue)
			);
		public static object GetDefaultValue(this Type type)
		{
			return defaultValues.GetOrAdd(type, t =>
				GetDefaultValueMethod.
					MakeGenericMethod(t).
					Invoke(null, Array.Empty<object>())
			);
		}
		
		public static Reflection.GenericParameterAttributes GetGenericParameterAttributes(this Type Type)
		{
			return Type.GetTypeInfo().GenericParameterAttributes;
		}
#if NETCORE
		public static Type[] GetGenericParameterConstraints(this Type Type)
		{
			return Type.GetTypeInfo().GetGenericParameterConstraints();
		}

#endif
		public static TypeAttributes GetTypeAttributes(this Type Type)
		{
			return Type.GetTypeInfo().Attributes;
		}
		public static Type GetTypedGenericArgument(this Type Type, Type GenericDefination)
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
		public static Type GetGenericArgumentTypeAsOption(this Type type) =>
			GetTypedGenericArgument(type, typeof(Option<>));
		public static Type GetGenericArgumentTypeAsEnumerable(this Type type) =>
			GetTypedGenericArgument(type, typeof(IEnumerable<>));

		public static (Type,Type) GetGenericArgumentTypeAsFunc2(this Type type)
		{
			if (type.IsGeneric() && type.GetGenericTypeDefinition() == typeof(Func<,>))
			{
				var args = type.GetGenericArguments();
				return (args[0], args[1]);
			}
			return (null,null);
		}

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
			if (matchingMethod == null && thisType.IsInterfaceType())
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
															 bindingFlags ).Where(m=>m is MethodInfo))
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
			if (thisType.IsGeneric() && type.IsGeneric())
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
		static System.Collections.Concurrent.ConcurrentDictionary<MemberInfo, SF.Metadata.CommentAttribute> CommentDict { get; }
			= new Collections.Concurrent.ConcurrentDictionary<MemberInfo, SF.Metadata.CommentAttribute>();
		public static SF.Metadata.CommentAttribute Comment(this MemberInfo Info)
		{
			SF.Metadata.CommentAttribute attr;
			if (CommentDict.TryGetValue(Info, out attr)) return attr;
			return CommentDict.GetOrAdd(
				Info,
				Info.GetCustomAttribute<SF.Metadata.CommentAttribute>(true) ??
				new SF.Metadata.CommentAttribute(Info.Name)
				);
		}
		public static string FriendlyName(this MemberInfo Info)
		{
			return Info.Comment()?.Name ?? Info.Name;
		}
#if NETCOREAPP1_1
		public static SF.Metadata.CommentAttribute Comment(this Type Info)
			=> Comment(Info.GetTypeInfo());
#endif

		public static int GetInheritLevel(this Type type)
		{
			var l = 0;
			while (type != null)
			{
				type = type.BaseType;
				l = l + 1;
			}
			return l;
		}
		public static PropertyInfo[] AllPublicInstanceProperties(this Type type)
			=> type.GetProperties(
				System.Reflection.BindingFlags.Public |
				System.Reflection.BindingFlags.Instance
			);

			//=> WithHiddenProperty ?
			//	type.GetProperties(
			//	System.Reflection.BindingFlags.FlattenHierarchy |
			//	System.Reflection.BindingFlags.Public |
			//	System.Reflection.BindingFlags.Instance
			//	) :
			//(from p in type.AllPublicInstanceProperties(true).Select((p, idx) => (idx: idx, prop: p))
			//group p by p.prop.Name into g
			//let p = (from gi in g
			//		let lev= gi.prop.DeclaringType.GetInheritLevel()
			//		orderby lev descending
			//		select gi
			//		).First()
			//orderby p.idx
			//select p.prop
			//).ToArray();


		static IEnumerable<Type> _AllInterfaces(Type type)
		{
			yield return type;
			foreach (var i in type.GetInterfaces())
				foreach (var ii in _AllInterfaces(i))
					yield return ii;
		}
		public static IEnumerable<Type> AllInterfaces(this Type type)
		{
			return _AllInterfaces(type).Distinct();
		}

		public static bool IsNumberLikeType(this Type type)
		{
			switch (type.GetTypeCode())
			{
				case TypeCode.Boolean:
				case TypeCode.Byte:
				case TypeCode.Char:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Single:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return true;
			}
			if (type.IsEnumType())
				return true;
			return false;
		}
		public static bool CanSimpleConvertTo(this Type src,Type dest)
		{
			if (src == dest) return true;
			if (dest.IsAssignableFrom(src)) return true;
			var srcIsNumber = src.IsNumberLikeType();
			var dstIsNumber = dest.IsNumberLikeType();
			return srcIsNumber == dstIsNumber;
		}

	}
}
