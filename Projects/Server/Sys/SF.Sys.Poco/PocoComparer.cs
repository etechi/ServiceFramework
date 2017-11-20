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

namespace SF.Sys
{
	public interface IComparerCollection
	{
		IComparer<T> GetComparer<T>();
		IEqualityComparer<T> GetEqualityComparer<T>();
	}
	public  static partial class Poco
	{
		
	

		class ComparerFuncBuilder<T>
		{
			static int CompareEnumerable<X>(IEnumerable<X> xs, IEnumerable<X> ys)
			{
				if (xs == ys)
					return 0;
				if (xs != null)
				{
					if (ys == null)
						return 1;
				}
				else if (ys != null)
					return -1;

				var comparer = DeepComparers.GetComparer<X>();
				using (var ex = xs.GetEnumerator())
				using (var ey = ys.GetEnumerator())
				{
					while (ex.MoveNext())
					{
						if (ey.MoveNext())
						{
							var re = comparer.Compare(ex.Current, ey.Current);
							if (re == 0)
								continue;
							return re;
						}
						return 1;
					}
					if (ey.MoveNext())
						return -1;
					return 0;
				}
			}
			static MethodInfo MethodCompareEnumerable { get; } = typeof(ComparerFuncBuilder<T>).GetMethodExt(
				nameof(ComparerFuncBuilder<T>.CompareEnumerable),
				BindingFlags.NonPublic | BindingFlags.Static,
				typeof(IEnumerable<>).MakeGenericType<TypeExtension.GenericTypeArgument>(),
				typeof(IEnumerable<>).MakeGenericType<TypeExtension.GenericTypeArgument>()
				).IsNotNull();
			static MethodInfo MethodGetComparer = typeof(IComparerCollection).GetMethodExt(nameof(IComparerCollection.GetComparer));
			static Expression UseFuncEqual(Expression argColl, Expression x, Expression y)
			{
				return Expression.Call(
						Expression.Call(
							argColl,
							MethodGetComparer.MakeGenericMethod(x.Type)
							),
						typeof(IComparer<>).MakeGenericType(x.Type).GetMethod(nameof(IComparer<int>.Compare)),
						x,
						y
						);
			}

			static Expression GetBaseEqualExpression(Expression argColl, Expression x, Expression y, Expression t, int level)
			{
				var comparableType = typeof(IComparable<>).MakeGenericType(x.Type);
				if (x.Type.GetInterfaces().Any(i => i == comparableType))
				{
					var comparer = x.Type.GetMethod(nameof(IComparable.CompareTo), BindingFlags.Public | BindingFlags.Instance, null, new[] { x.Type }, null);
					if (comparer != null)
						return Expression.Call(x, comparer, y);
					else if (x.Type.IsNumberLikeType())
						throw new InvalidOperationException($"数据类型{x.Type}中找不到CompareTo方法");

					comparer = comparableType.GetMethod(nameof(IComparable.CompareTo), BindingFlags.Public | BindingFlags.Instance, null, new[] { x.Type }, null);
					if (comparer != null)
						return Expression.Call(
							Expression.Convert(x, comparableType),
							comparer,
							y
							);
					throw new InvalidOperationException($"类型{x.Type}中实现了IComparable<>但找不到CompareTo方法");
				}
				if (level == 0)
					return UseFuncEqual(argColl, x, y);


				var props = x.Type.AllPublicInstanceProperties()
					.Where(p=>p.GetGetMethod().GetParameters().Length==0)
					.ToArray();

				var expr =
					props.Length == 0 ? null :
					props.Length == 1 ? GetEqualExpression(
							argColl,
							Expression.Property(x, props[0]),
							Expression.Property(y, props[0]),
							t,
							level - 1
							) :
					props.Select(
						p => GetEqualExpression(
							argColl,
							Expression.Property(x, p),
							Expression.Property(y, p),
							t,
							level - 1
							)
						)
						.Reverse()
						.Aggregate(
							(tx, ty) => Expression.Condition(
								Expression.NotEqual(
									Expression.Assign(
										t,
										ty
									),
									Expression.Constant(0)
									),
								t,
								tx
							)
						);



				var interfaces = x.Type.GetInterfaces();
				var enumerable = interfaces.FirstOrDefault(i => i.IsGeneric() && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
				if (enumerable != null)
				{
					var itemType = enumerable.GenericTypeArguments[0];
					var enumCompare = Expression.Call(
						null,
						MethodCompareEnumerable.MakeGenericMethod(itemType),
						x.To(typeof(IEnumerable<>).MakeGenericType(itemType)),
						y.To(typeof(IEnumerable<>).MakeGenericType(itemType))
						);
					expr = expr == null ?
						(Expression)enumCompare :
						Expression.Condition(
							Expression.NotEqual(
								Expression.Assign(
									t,
									expr
								),
								Expression.Constant(0)
								),
							t,
							enumCompare
						);
				}

				return expr ?? Expression.Constant(0);

			}

			static Expression NullValue { get; } = Expression.Constant(null, typeof(object));
			static Expression GetEqualExpression(Expression argColl, Expression x, Expression y, Expression t, int level)
			{
				if (x.Type != y.Type)
					throw new InvalidOperationException("类型不一致");


				if (x.Type.IsValue())
				{
					if(x.Type.IsGenericTypeOf(typeof(Nullable<>)))
					{
						return Expression.Condition(
							Expression.And(
								Expression.IsTrue(x.GetMember("HasValue")),
								Expression.IsTrue(y.GetMember("HasValue"))
								),
							GetBaseEqualExpression(argColl,x,y,t,level),
							Expression.Condition(
								Expression.And(
									Expression.IsFalse(x.GetMember("HasValue")),
									Expression.IsFalse(y.GetMember("HasValue"))
									),
								Expression.Constant(0),
								Expression.Condition(
									Expression.And(
										Expression.IsTrue(x.GetMember("HasValue")),
										Expression.IsFalse(y.GetMember("HasValue"))
										),
									Expression.Constant(1),
									Expression.Constant(-1)
								)
							)
						);
					}
					return GetBaseEqualExpression(argColl, x, y, t, level);
				}
				return Expression.Condition(
						Expression.And(
							Expression.NotEqual(x, NullValue),
							Expression.NotEqual(y, NullValue)
							),
						GetBaseEqualExpression(
							argColl,
							x,
							y,
							t,
							level
							),
						Expression.Condition(
							Expression.And(
								Expression.Equal(x, NullValue),
								Expression.Equal(y, NullValue)
								),
							Expression.Constant(0),
							Expression.Condition(
								Expression.And(
									Expression.NotEqual(x, NullValue),
									Expression.Equal(y, NullValue)
									),
								Expression.Constant(1),
								Expression.Constant(-1)
							)
					)
				);
			}
			public static Func<IComparerCollection, T, T, int> Build()
			{
				var type = typeof(T);
				var argColl = Expression.Parameter(typeof(IComparerCollection));
				var argX = Expression.Parameter(type);
				var argY = Expression.Parameter(type);
				var varT = Expression.Variable(typeof(int));
				return Expression.Lambda<Func<IComparerCollection, T, T, int>>(
					Expression.Block(
						new[] { varT},
							GetEqualExpression(argColl, argX, argY, varT,1)
						),
						argColl,
						argX,
						argY
						).Compile();
			}
		}


		class GetHashCodeFuncBuilder<T>
		{
			static int GetEnumerableHashCode<X>(IEnumerable<X> xs)
			{
				var comparer = DeepComparers.GetEqualityComparer<X>();
				int r = 0;
				foreach (var x in xs)
					r ^= comparer.GetHashCode(x);
				return r;
			}
			static MethodInfo MethodGetEnumerableHashCode { get; } = typeof(GetHashCodeFuncBuilder<T>).GetMethodExt(
				nameof(GetHashCodeFuncBuilder<T>.GetEnumerableHashCode),
				BindingFlags.NonPublic | BindingFlags.Static,
				typeof(IEnumerable<>).MakeGenericType<TypeExtension.GenericTypeArgument>()
				).IsNotNull();

			static MethodInfo MethodGetEqualityComparer = typeof(IComparerCollection).GetMethodExt(nameof(IComparerCollection.GetEqualityComparer));
			static Expression UseFuncGetHashCode(Expression argColl, Expression x)
			{
				return Expression.Call(
						Expression.Call(
							argColl,
							MethodGetEqualityComparer.MakeGenericMethod(x.Type)
							),
						typeof(IEqualityComparer<>).MakeGenericType(x.Type).GetMethod(
							nameof(IEqualityComparer<int>.GetHashCode),
							new[] { x.Type }
							),
						x
						);
			}

			static Expression BaseGetHashCodeExpression(Expression argColl, Expression x, int level)
			{
				var GetHashCodeMethod = x.Type.GetMethod(nameof(object.GetHashCode), BindingFlags.Public | BindingFlags.Instance);
				if(GetHashCodeMethod.DeclaringType!=typeof(object))
					return Expression.Call(
						x,
						GetHashCodeMethod
						);
				if (level == 0)
					return UseFuncGetHashCode(argColl, x);


				var props = x.Type.AllPublicInstanceProperties().ToArray();
				var expr =
					props.Length == 0 ? null :
					props.Length == 1 ? GetHashCodeExpression(
							argColl,
							Expression.Property(x, props[0]),
							level - 1
							) :
					props.Select(
						p => GetHashCodeExpression(
							argColl,
							Expression.Property(x, p),
							level - 1
							)
						)
						.Aggregate((tx, ty) => Expression.ExclusiveOr(tx,ty)
						);


				var interfaces = x.Type.GetInterfaces();
				var enumerable = interfaces.FirstOrDefault(i => i.IsGeneric() && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
				if (enumerable != null)
				{
					var itemType = enumerable.GenericTypeArguments[0];
					var enumHashCode = Expression.Call(
						null,
						MethodGetEnumerableHashCode.MakeGenericMethod(itemType),
						x.To(typeof(IEnumerable<>).MakeGenericType(itemType))
						);
					expr = expr == null ?
						(Expression)enumHashCode :
						Expression.ExclusiveOr(expr, enumHashCode);
				}

				var typeHashCode = (Expression)Expression.Constant(x.Type.GetHashCode());
				expr = expr == null ?
						typeHashCode :
						Expression.ExclusiveOr(expr, typeHashCode);

				return expr ;

			}

			static Expression NullValue { get; } = Expression.Constant(null, typeof(object));
			static Expression GetHashCodeExpression(Expression argColl, Expression x, int level)
			{
				if (x.Type.IsValue())
					return BaseGetHashCodeExpression(argColl, x, level);
				return Expression.Condition(
						Expression.NotEqual(x, NullValue),
						BaseGetHashCodeExpression(
							argColl,
							x,
							level
							),
						Expression.Constant(0)
					);
			}
			public static Func<IComparerCollection, T, int> Build()
			{
				var argColl = Expression.Parameter(typeof(IComparerCollection));
				var argX = Expression.Parameter(typeof(T));
				return Expression.Lambda<Func<IComparerCollection, T,int>>(
						GetHashCodeExpression(argColl, argX, 1),
						argColl,
						argX
						).Compile();
			}
		}
		class InternalComparer<T> : IComparer<T>,IEqualityComparer<T>
		{
			public IComparerCollection ComparerCollection { get; }
			Lazy<Func<IComparerCollection, T, T, int>> FuncComparer { get; }
			Lazy<Func<IComparerCollection, T, int>> FuncGetHashCode { get; }

			public InternalComparer(IComparerCollection ComparerCollection)
			{
				this.ComparerCollection = ComparerCollection;
				FuncComparer = new Lazy<Func<IComparerCollection, T, T, int>>(
					() => ComparerFuncBuilder<T>.Build()
					);
				FuncGetHashCode = new Lazy<Func<IComparerCollection, T, int>>(
					() => GetHashCodeFuncBuilder<T>.Build()
					);
			}
			public int Compare(T x, T y)
			{
				return FuncComparer.Value(ComparerCollection, x, y);
			}

			public bool Equals(T x, T y)
			{
				return Compare(x, y) == 0;
			}

			public int GetHashCode(T obj)
			{
				return FuncGetHashCode.Value(ComparerCollection,obj);
			}
		}
		class ComparerCollection :
			System.Collections.Concurrent.ConcurrentDictionary<Type, object>,
			IComparerCollection
		{
			public IComparer<T> GetComparer<T>()
			{
				var key = typeof(T);
				if (!TryGetValue(key, out var c))
				{
					c = GetOrAdd(key, new InternalComparer<T>(this));
				}
				return (IComparer<T>)c;
			}
			public IEqualityComparer<T> GetEqualityComparer<T>()
			{
				var key = typeof(T);
				if (!TryGetValue(key, out var c))
				{
					c = GetOrAdd(key, new InternalComparer<T>(this));
				}
				return (IEqualityComparer<T>)c;
			}
		}
		public static IComparerCollection DeepComparers { get; } = new ComparerCollection();
		public static IComparer<T> DeepComparer<T>()
		{
			return DeepComparers.GetComparer<T>();
		}
		public static IEqualityComparer<T> DeepEqualityComparer<T>()
		{
			return DeepComparers.GetEqualityComparer<T>();
		}
		public static int DeepCompare<T>(T x, T y)
		{
			return DeepComparers.GetComparer<T>().Compare(x, y);
		}
		public static bool DeepEquals<T>(T x, T y)
			=> DeepCompare<T>(x, y) == 0;

		public static int GetDeepHashCode<T>(T x)
		{
			return DeepComparers.GetEqualityComparer<T>().GetHashCode(x);
		}
	}
}

