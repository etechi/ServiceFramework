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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;
using SF.Sys.Reflection;
using SF.Sys.Linq.Expressions;
using SF.Sys.Linq;

namespace SF.Sys
{
	public  static partial class Poco
	{
		
		class Updater<T>
		{
			public static Func<T,T,T> Update { get; }

			static MethodInfo MethodClone { get; } =
				typeof(Poco).GetMethodExt(
					nameof(Clone),
					BindingFlags.Static | BindingFlags.Public,
					typeof(TypeExtension.GenericTypeArgument)
					);
			static Expression GetPropExpression(Type type, Expression value)
			{
				if (type.IsConstType())
					return value;
				if (typeof(Delegate).IsAssignableFrom(type))
					return value;
				return Expression.Call(
						MethodClone.MakeGenericMethod(type),
						value
					);

			}
			static Expression GetObjectUpdateExpression(Type type, Expression target, Expression source)
			{
				if (type.IsConstType())
					return source;

				var exp = Expression.Block(
					type.AllPublicInstanceProperties()
					.Where(p => p.CanRead && p.CanWrite)
					.Select(p =>
						target.CallMethod(
							p.GetSetMethod(),
							GetPropExpression(
								p.PropertyType,
								source.GetMember(p)
							)
						)
					).WithLast(target)
				);

				var defValue = Expression.Constant(type.GetDefaultValue(), type);
				return type.IsClass ?
					target.Equal(defValue).ForCondition(
						source,
						source.Equal(defValue).ForCondition(
							defValue,
							exp
							)
						) :
					exp;
			}
			static Updater()
			{
				var type = typeof(T);
				var argTarget = Expression.Parameter(type, "target");
				var argSource = Expression.Parameter(type, "source");
				Update= GetObjectUpdateExpression(type, argTarget, argSource)
					.Compile<Func<T, T, T>>(
						argTarget,
						argSource
						);
			}
		}
		public static T Update<T>(T target,T source)
		{
			return Updater<T>.Update(target, source);

		}
		static C CopyCollection<C, S, I>(S source)
				where C : ICollection<I>, new()
				where S : IEnumerable<I>
		{
			if (EqualityComparer<S>.Default.Equals(source, default(S)))
				return default(C);

			var collection = new C();
			foreach (var i in source)
				collection.Add(Clone(i));
			return collection;
		}
		static I[] CopyArray<I>(I[] source)
		{
			if (source == null)
				return null;

			var collection = new I[source.Length];
			for(var i=0;i<source.Length;i++)
				collection[i]=Clone(source[i]);
			return collection;
		}

		static KeyValuePair<K,V> CopyKeyValuePair<K,V>(KeyValuePair<K,V> pair)
		{
			return new KeyValuePair<K, V>(pair.Key, pair.Value);
		}
		class Cloner<T>
		{
			public static Func<T,T> Clone { get; }
			
			static MethodInfo MethodCopyCollection { get; } =
				typeof(Poco).GetMethodExt(
					nameof(CopyCollection),
					BindingFlags.Static | BindingFlags.NonPublic,
					typeof(TypeExtension.GenericTypeArgument)
					).IsNotNull();

			static MethodInfo MethodCopyKeyValuePair { get; } =
				typeof(Poco).GetMethodExt(
					nameof(CopyKeyValuePair),
					BindingFlags.Static | BindingFlags.NonPublic,
					typeof(KeyValuePair<TypeExtension.GenericTypeArgument, TypeExtension.GenericTypeArgument>)
					).IsNotNull();

			static MethodInfo MethodCopyArray { get; } =
				typeof(Poco).GetMethodExt(
					nameof(CopyArray),
					BindingFlags.Static | BindingFlags.NonPublic,
					typeof(TypeExtension.GenericTypeArgument).MakeArrayType()
					).IsNotNull();

			static Func<T,T> CreateCloner()
			{
				var type = typeof(T);
				if (type.IsArray)
					return MethodCopyArray.MakeGenericMethod(
						type.GetElementType()
						).CreateDelegate<Func<T, T>>();

				if (type.IsGenericTypeOf(typeof(List<>)) ||
					type.IsGenericTypeOf(typeof(IEnumerable<>)) ||
					type.IsGenericTypeOf(typeof(ICollection<>)) ||
					type.IsGenericTypeOf(typeof(IReadOnlyCollection<>)) ||
					type.IsGenericTypeOf(typeof(IList<>)) ||
					type.IsGenericTypeOf(typeof(IReadOnlyList<>))
					)
					return MethodCopyCollection.MakeGenericMethod(
						typeof(List<>).MakeGenericType(type.GenericTypeArguments[0]),
						type,
						type.GenericTypeArguments[0]
						).CreateDelegate<Func<T, T>>();

				if (type.IsGenericTypeOf(typeof(IReadOnlyDictionary<,>)) ||
					type.IsGenericTypeOf(typeof(IDictionary<,>))
					)
					return MethodCopyCollection.MakeGenericMethod(
						typeof(Dictionary<,>).MakeGenericType(
							type.GenericTypeArguments[0],
							type.GenericTypeArguments[1]
							),
						type,
						typeof(KeyValuePair<,>).MakeGenericType(
							type.GenericTypeArguments[0],
							type.GenericTypeArguments[1]
							)
						).CreateDelegate<Func<T, T>>();
				if (type.IsGenericTypeOf(typeof(KeyValuePair<,>)))
					return MethodCopyKeyValuePair.MakeGenericMethod(
							type.GenericTypeArguments[0],
							type.GenericTypeArguments[1]
						).CreateDelegate<Func<T, T>>();

				if (type.IsInterface)
					throw new NotSupportedException($"不支持克隆此接口:{type}");

				if (type.IsConstType())
					return (s) => s;

				if (type.IsGenericTypeOf(typeof(Nullable<>)))
					return (s) => s;


				var ctr = type.GetConstructor(
						BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.Instance,
						null,
						Array.Empty<Type>(),
						null
						);
				if (ctr == null)
					throw new NotSupportedException($"不支持克隆没有无参构造函数的对象:{type}");

				var collType = type.AllInterfaces().FirstOrDefault(i => i.IsGenericTypeOf(typeof(ICollection<>)));
				if (collType != null)
					return MethodCopyCollection.MakeGenericMethod(
						type,
						type,
						collType.GenericTypeArguments[0]
						).CreateDelegate<Func<T, T>>();

				var ctrFunc = Expression.New(ctr).Compile<Func<T>>();
				return (s) =>
				{
					var re = ctrFunc();
					return Poco.Update(re, s);
				};
			}
			static Cloner()
			{
				Clone = CreateCloner();
			}
		}
		public static T Clone<T>(T obj) 
		{
			return 	Cloner<T>.Clone(obj);
		}	
	
	}
}

