﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel;

namespace SF.ADT
{
	public  static partial class Poco
	{
		public interface IMapper
		{
			Type SrcType { get; }
			Type DstType { get; }
		}

		public interface IMapper<S, T>: IMapper
		{
			Expression<Func<S, T>> Expr{ get; }
			T Map(S src);
			Func<S, T> Func { get; }
		}

		class InternalMapper<S, T> : IMapper<S, T>
		{

			
			static Y[] EnumerableToArray<Z, X, Y>(IMapper<X, Y> Mapper, Z src)
				where Z : IEnumerable<X>
				=> src.Select(i => Mapper.Map(i)).ToArray();
			static MethodInfo MethodEnumerableToArray = typeof(InternalMapper<S, T>).GetMethodExt(
				nameof(InternalMapper<S, T>.EnumerableToArray),
				BindingFlags.NonPublic | BindingFlags.Static,
				new[]
				{
					typeof(IMapper<TypeExtension.GenericTypeArgument,TypeExtension.GenericTypeArgument>),
					typeof(TypeExtension.GenericTypeArgument)
				});

			static List<Y> EnumerableToList<Z, X, Y>(IMapper<X, Y> Mapper, Z src)
				where Z : IEnumerable<X>
				=> src.Select(i => Mapper.Map(i)).ToList();
			static MethodInfo MethodEnumerableToList = typeof(InternalMapper<S, T>).GetMethodExt(
				nameof(InternalMapper<S, T>.EnumerableToList),
				BindingFlags.NonPublic | BindingFlags.Static,
				new[]
				{
					typeof(IMapper<TypeExtension.GenericTypeArgument,TypeExtension.GenericTypeArgument>),
					typeof(TypeExtension.GenericTypeArgument)
				});

			static Dictionary<KD, VD> EnumerableToDictionary<Z, KS, VS, KD, VD>(
				IMapper<KS, KD> KMapper, 
				IMapper<VS, VD> VMapper, 
				Z src
				)
				where Z : IEnumerable<KeyValuePair<KS, VS>>
			{
				return src.ToDictionary(p => KMapper.Map(p.Key), p => VMapper.Map(p.Value));
			}
			static MethodInfo MethodEnumerableToDictionary = typeof(InternalMapper<S, T>).GetMethodExt(
				nameof(InternalMapper<S, T>.EnumerableToDictionary),
				BindingFlags.NonPublic | BindingFlags.Static,
				new[]
				{
					typeof(IMapper<TypeExtension.GenericTypeArgument,TypeExtension.GenericTypeArgument>),
					typeof(IMapper<TypeExtension.GenericTypeArgument,TypeExtension.GenericTypeArgument>),
					typeof(TypeExtension.GenericTypeArgument)
				});



			static Expression CloneExpression(Type DstType,Expression src)
			{
				var srcType = src.Type;
				if (srcType==DstType)
				{
					if (srcType.IsConstType())
						return src;
				}
				else if (srcType.IsDefined(typeof(IConvertible)))
				{
					var method = typeof(IConvertible)
						.GetMethods(BindingFlags.Public | BindingFlags.Instance)
						.FirstOrDefault(m => m.ReturnType == DstType && m.Name != "ToType" && m.Name.StartsWith("To"));
					if (method != null)
					{
						var srcMethod = srcType.GetMethod(
							method.Name,
							BindingFlags.Public | BindingFlags.Instance,
							null,
							new[] { typeof(IFormatProvider) },
							null
							);
						return srcMethod == null ?
							Expression.Call(
								src.To(typeof(IConvertible)),
								method,
								Expression.Constant(null, typeof(IFormatProvider))
								) :
							Expression.Call(
								src,
								srcMethod,
								Expression.Constant(null, typeof(IFormatProvider))
								);
					}
				}

				//if (DstType!=srcType && DstType.IsAssignableFrom(srcType))
				//	return CloneExpression(srcType, src).To(DstType);

				//if(DstType.IsGeneric() && DstType.GetGenericTypeDefinition()==typeof(Nullable<>))


				//if (DstType.IsInterfaceType())
				//{
					

				//	if (!DstType.IsGeneric())
				//		throw new NotSupportedException($"不支持将{srcType}映射至{DstType}类型");

				//	var dtd = DstType.GetGenericTypeDefinition();
				//	if (dtd == typeof(IEnumerable<>))
				//	{

				//	}
				//	if (dtd == typeof(IList<>) || dtd==typeof(IReadOnlyList<>))
				//	{

				//	}
				//	else if(dtd==typeof(IDictionary<,>) || dtd==typeof(IReadOnlyDictionary<,>))
				//	{

				//	}




				//	DstType.AllInterfaces().Any(t=>t.IsGeneric() && t.GetGenericTypeDefinition()==typeof(IEnumerable<>))
				//	if (DstType.IsGeneric())
				//	{
				//		DstType.GetGenericTypeDefinition()
				//	}
				//}

				return Expression.MemberInit(
					Expression.New(DstType),
					(from dp in DstType.AllPublicInstanceProperties()
					 let sp = src.Type.GetProperty(dp.Name, BindingFlags.Public | BindingFlags.Instance)
					 where sp != null
					 select Expression.Bind(
						 dp,
						 //CloneExpression(
						//	 dp.PropertyType,
							 Expression.Property(
								 src,
								 sp
								 ).To(dp.PropertyType)
						//	 )
						 )
					)
				);
			}


			static Lazy<Expression<Func<S, T>>> LazyExpr { get; } = new Lazy<Expression<Func<S, T>>>(() =>
				{
					var arg = Expression.Parameter(typeof(S), "src");
					return Expression.Lambda<Func<S, T>>(
						CloneExpression(typeof(T),arg),
						arg
					  );
				});

			static Lazy<Func<S, T>> LazyFunc { get; } = new Lazy<Func<S, T>>(() =>
				LazyExpr.Value.Compile()
			);

			public Expression<Func<S, T>> Expr => LazyExpr.Value;

			public Func<S, T> Func => LazyFunc.Value;

			public Type SrcType { get; }= typeof(S);

			public Type DstType { get; }= typeof(T);

			public T Map(S src)
			{
				return LazyFunc.Value(src);
			}
		}

		static System.Collections.Concurrent.ConcurrentDictionary<(Type, Type), IMapper> Mappers { get; } 
			= new System.Collections.Concurrent.ConcurrentDictionary<(Type, Type), IMapper>();

		public static IMapper<S,T> Mapper<S, T>()
		{
			var key = (typeof(S), typeof(T));
			if (Mappers.TryGetValue(key, out var m)) return (IMapper < S,T > )m;
			return (IMapper<S, T>)Mappers.GetOrAdd(key, new InternalMapper<S, T>());

		}
		public static Expression<Func<S,T>> MapExpression<S, T>()
		{
			return Mapper<S, T>().Expr;
		}
		public static T Map<S, T>(S src)
		{
			return Mapper<S, T>().Map(src);
		}
	}
   
}