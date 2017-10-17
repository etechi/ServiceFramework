﻿#region Apache License Version 2.0
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
using System.Reactive.Linq;
using System.Linq.Expressions;
using System.Reflection;
namespace SF.ADT
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
		
		class Cloner<T>
		{
			public static Func<T,T> Clone { get; }
			static C CopyCollection<C,S,I>(S source)
				where C : ICollection<I>, new()
				where S: IEnumerable<I>
			{
				if (EqualityComparer<S>.Default.Equals(source, default(S)))
					return default(C);

				var collection = new C();
				foreach (var i in source)
					collection.Add(i);
				return collection;
			}
			static MethodInfo MethodCopyCollection { get; } =
				typeof(Cloner<T>).GetMethodExt(
					nameof(CopyCollection),
					BindingFlags.Static | BindingFlags.NonPublic,
					typeof(TypeExtension.GenericTypeArgument)
					);
			static Cloner()
			{
				var type = typeof(T);
				if (type.IsInterfaceType())
				{
					if (type.IsGenericTypeOf(typeof(IEnumerable<>)) ||
						type.IsGenericTypeOf(typeof(ICollection<>)) ||
						type.IsGenericTypeOf(typeof(IReadOnlyCollection<>)) ||
						type.IsGenericTypeOf(typeof(IList<>)) ||
						type.IsGenericTypeOf(typeof(IReadOnlyList<>))
						)
					{
						Clone = MethodCopyCollection.MakeGenericMethod(
							typeof(List<>).MakeGenericType(type.GenericTypeArguments[0]),
							type,
							type.GenericTypeArguments[0]
							).CreateDelegate<Func<T, T>>();
						return;
					}
					if (type.IsGenericTypeOf(typeof(IReadOnlyDictionary<,>)) ||
						type.IsGenericTypeOf(typeof(IDictionary<,>))
						)
					{
						Clone = MethodCopyCollection.MakeGenericMethod(
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
						return;
					}

					throw new NotSupportedException($"不支持克隆此接口:{type}");
				}

				if(type.IsConstType())
				{
					Clone = (s) => s;
					return;
				}
				if(type.IsGenericTypeOf(typeof(Nullable<>)))
				{
					Clone = (s) => s;
					return;
				}

				var ctr = type.GetConstructor(
						BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.Instance,
						null,
						Array.Empty<Type>(),
						null
						);
				if (ctr == null)
					throw new NotSupportedException($"不支持克隆没有无参构造函数的对象:{type}");

				if (type.AllInterfaces().Any(i => i.IsGenericTypeOf(typeof(ICollection<>))))
				{
					Clone = MethodCopyCollection.MakeGenericMethod(
						type,
						type,
						type.GenericTypeArguments[0]
						).CreateDelegate<Func<T, T>>();
					return;
				}
				var ctrFunc = Expression.New(ctr).Compile<Func<T>>();
				Clone = (s) =>
				{
					var re = ctrFunc();
					return Poco.Update(re, s);
				};
			}
		}
		public static T Clone<T>(T obj) 
		{
			return 	Cloner<T>.Clone(obj);
		}	
	
	}
}

