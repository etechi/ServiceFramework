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
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Linq;
using System.Collections.Concurrent;

namespace SF.Sys.Reflection
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

		static System.Collections.Concurrent.ConcurrentDictionary<(Type,bool), string> TypeNames { get; } = new ConcurrentDictionary<(Type,bool), string>();

		public static string GetTypeNamePostfix(this Type type,bool withNamespace)
		{
			if (type.IsGenericTypeDefinition)
			{
				return $"<{Enumerable.Repeat("", type.GetGenericArguments().Length ).Join(",")}>";
			}
			else if (type.IsGenericType)
			{
				var gas = type.GenericTypeArguments;
				if (type.IsGenericTypeOf(typeof(Nullable<>)))
					return "?";
				return $"<{gas.Select(at=>at.GetTypeName(withNamespace)).Join(",")}>";
			}
			else
				return string.Empty;
		}
		public static string GetFullName(this Type type) => type.GetTypeName(true);
		public static string GetTypeName(this Type type,bool withNamespace=false)
		{
			var key = (type, withNamespace);
			if (TypeNames.TryGetValue(key, out var name))
				return name;
			return TypeNames.GetOrAdd(key, k =>
			{
				var (t, ns) = k;
				if (t.IsArray)
					return t.GetElementType().GetTypeName(ns) + "[]";
				
				switch (Type.GetTypeCode(t))
				{
					case TypeCode.Boolean:return "bool";
					case TypeCode.Byte: return "byte";
					case TypeCode.Char: return "char";
					case TypeCode.DateTime: return "DateTime";
					case TypeCode.DBNull: return "null";
					case TypeCode.Decimal: return "decimal";
					case TypeCode.Double: return "double";
					case TypeCode.Empty: return "empty";
					case TypeCode.Int16: return "short";
					case TypeCode.Int32: return "int";
					case TypeCode.Int64: return "long";
					case TypeCode.SByte: return "sbyte";
					case TypeCode.Single: return "float";
					case TypeCode.String: return "string";
					case TypeCode.UInt16: return "ushort";
					case TypeCode.UInt32: return "uint";
					case TypeCode.UInt64: return "ulong";
				}
				if (t == typeof(object)) return "object";

				string tn;
				if (t.IsGenericDefinition())
				{
					var p = t.Name.LastSplit2('`');
					tn = $"{p.Item1}<{Enumerable.Repeat("", p.Item2.ToInt32()).Join(",")}>";
				}
				else if (t.IsGeneric())
				{
					var gtd = t.GetGenericTypeDefinition();
					var gas = t.GetGenericArguments();
					if (gtd == typeof(Nullable<>))
						return gas[0].GetTypeName(ns) + "?";

					var p = t.Name.LastSplit2('`');
					tn=p.Item1 + "<" +
						gas.Select(at => at.GetTypeName(ns)).Join(",") +
						">";
				}
				else
					tn = t.Name;
				if (ns)
					return (t.DeclaringType == null ? t.Namespace : t.DeclaringType.GetFullName()) + "." + tn;
				else
					return tn;
			});
			
		}
		public static string ShortName(this Type type) => type.GetTypeName(false);

		public static string ShortName(this MethodInfo method)
		{
			if (method.IsGenericMethodDefinition)
			{
				var p = method.Name.LastSplit2('`');
				return $"{p.Item1}<{Enumerable.Repeat("", p.Item2.ToInt32()).Join(",")}>";
			}
			else if (method.IsGenericMethod)
			{
				return method.Name.TrimEndTo("`") + "<" +
					method.GetGenericArguments().Select(t => t.GetFullName()).Join(",") +
					">";
			}
			return method.Name;
		}

		public static string ShortName(this MemberInfo Info)
		{
			if (Info is Type type)
				return type.ShortName();
			else if (Info is MethodInfo method)
				return method.ShortName();
			return Info.Name;
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
		//public static IEnumerable<Attribute> GetCustomAttributes(this Type type)
		//{
		//	return type.GetTypeInfo().GetCustomAttributes().Cast<Attribute>();
		//}
		//public static IEnumerable<T> GetCustomAttributes<T>(this Type type) where T:Attribute
		//{
		//	return type.GetTypeInfo().GetCustomAttributes<T>();
		//}
		//public static IEnumerable<Attribute> GetCustomAttributes(this Type type, bool inherit)
		//{
		//	return type.GetTypeInfo().GetCustomAttributes(inherit).Cast<Attribute>();
		//}
		//public static IEnumerable<T> GetCustomAttributes<T>(this Type type, bool inherit)
		//{
		//	return type.GetTypeInfo().GetCustomAttributes(inherit).Where(a => a is T).Cast<T>();
		//}
		//public static T GetCustomAttribute<T>(this Type type) where T : Attribute
		//{
		//	return (T)type.GetTypeInfo().GetCustomAttribute(typeof(T));
		//}
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
		static HashSet<Type> ConstTypes { get; } = new HashSet<Type>
		{
			typeof(string),
			typeof(DateTime),
			typeof(TimeSpan),
			typeof(Guid)
		};
		public static bool IsConstType(this Type type)
		{
			if (type.IsPrimitiveType())
				return true;

			if (type.IsEnumType())
				return true;

			return ConstTypes.Contains(type);
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

		static System.Collections.Concurrent.ConcurrentDictionary<Type, object> defaultValues { get; } = new ConcurrentDictionary<Type, object>();
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
		
		public static GenericParameterAttributes GetGenericParameterAttributes(this Type Type)
		{
			return Type.GetTypeInfo().GenericParameterAttributes;
		}

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
		public static Type GetGenericArgumentTypeAsNullable(this Type type) =>
			GetTypedGenericArgument(type, typeof(Nullable<>));
		public static Type GetGenericArgumentTypeAsFunc(this Type type) =>
			GetTypedGenericArgument(type, typeof(Func<>));
		//public static Type GetGenericArgumentTypeAsOption(this Type type) =>
		//	GetTypedGenericArgument(type, typeof(Option<>));
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

		public static IEnumerable<MethodInfo> GetMethods(this Type type,string Name,BindingFlags BindingFlags)
		{
			return type.GetMember(Name, BindingFlags | BindingFlags.InvokeMethod).Cast<MethodInfo>();
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
		public class GenericTypeArgument
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
			if (thisType == type || ((thisType.IsGenericParameter || thisType == typeof(GenericTypeArgument))
								 && (type.IsGenericParameter || type == typeof(GenericTypeArgument))))
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
		public static MethodInfo[] AllPublicInstanceMethods(this Type type)
			=> type.GetMethods(
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

		public static bool IsGenericTypeOf(this Type type, Type GenericType)
		{
			if (!GenericType.IsGenericDefinition())
				throw new ArgumentException();
			return type.IsGeneric() && type.GetGenericTypeDefinition() == GenericType;
		}
		public static IEnumerable<Type> BaseTypes(this Type type)
		{
			for (var t = type.BaseType; t != null; t = t.BaseType)
				yield return t;
		}
		public static bool IsAnyRelatedTypeDefined(this Type type,Type AttributeType)
		{
			if (type.IsInterface)
				return type.AllInterfaces().Any(i => i.IsDefined(AttributeType, true));
			else if (type.IsDefined(AttributeType, true))
				return true;
			else
				return type.AllInterfaces().Any(i => i.IsDefined(AttributeType, true));
		}
		public static IEnumerable<Type> AllRelatedTypes(this Type type)
		{
			if (type.IsInterface) return type.AllInterfaces();
			return EnumerableEx.From(type).Concat(type.BaseTypes()).Concat(type.AllInterfaces());
		}
		public static IEnumerable<Type> AllInterfaces(this Type type)
		{
			if(type.IsInterface)
				return SF.Sys.ADT.Tree.AsEnumerable(type,t=>t.GetInterfaces()).Distinct();
			else
				return SF.Sys.ADT.Tree.AsEnumerable(type.GetInterfaces(), t => t.GetInterfaces()).Distinct();
		}
		public static IEnumerable<Attribute> GetInterfaceAttributes(this Type type)
			=> from i in type.AllInterfaces()
			   from a in i.GetCustomAttributes()
			   select a;

		public static IEnumerable<T> GetInterfaceAttributes<T>(this Type type) where T : Attribute
			=> type.GetInterfaceAttributes().Where(a => a is T).Cast<T>();

		public static T GetFirstInterfaceAttribute<T>(this Type type) where T : Attribute
			=> (T)type.GetInterfaceAttributes().FirstOrDefault(a => a is T);

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
			return srcIsNumber && dstIsNumber;
		}

	}
}
