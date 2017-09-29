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
	public interface IComparerCollection
	{
		IComparer<T> GetComparer<T>();
	}
	public  static partial class Poco
	{
		class InternalComparer<T> : IComparer<T>
		{
			public IComparerCollection ComparerCollection { get; }
			Lazy<Func<IComparerCollection, T, T, int>> Func { get; }
			static MethodInfo MethodGetComparer = typeof(IComparerCollection).GetMethodExt("GetComparer");

			static Expression GetBaseEqualExpression(Expression argColl, Expression x,Expression y,int level)
			{
				var comparableType = typeof(IComparable<>).MakeGenericType(x.Type);
				if(x.Type.GetInterfaces().Any(i=>i== comparableType))
				{
					var comparer=x.Type.GetMethod("CompareTo", BindingFlags.Public | BindingFlags.Instance, null, new[] { x.Type }, null);
					if(comparer!=null)
						return Expression.Call(x,comparer,y);
					else if(x.Type.IsNumberLikeType())
						throw new InvalidOperationException($"数据类型{x.Type}中找不到CompareTo方法");

					comparer = comparableType.GetMethod("CompareTo", BindingFlags.Public | BindingFlags.Instance, null, new[] { x.Type }, null);
					if(comparer!=null)
						return Expression.Call(
							Expression.Convert(x,comparableType), 
							comparer, 
							y
							);
					throw new InvalidOperationException($"类型{x.Type}中实现了IComparable<>但找不到CompareTo方法");
				}
				if(level==0)
					return Expression.Call(
						Expression.Call(
							argColl,
							MethodGetComparer.MakeGenericMethod(x.Type)
							),
						typeof(IComparer<>).MakeGenericType(x.Type).GetMethod("Compare"),
						x,
						y
						);

				return x.Type.AllPublicInstanceProperties().Select(
						p => GetEqualExpression(
							argColl,
							Expression.Property(x, p),
							Expression.Property(y, p),
							level-1
							)
						).Aggregate(
							(tx, ty) => Expression.Call(
								null,
								MethodMergeCompareResult,
								tx,
								ty
							)
						);
			}

			static Expression NullValue { get; } = Expression.Constant(null, typeof(object));
			static Expression GetEqualExpression(Expression argColl, Expression x, Expression y,int level)
			{
				if (x.Type != y.Type)
					throw new InvalidOperationException("类型不一致");

				if (x.Type.IsValue())
					return GetBaseEqualExpression(argColl, x, y, level);
				return Expression.Condition(
						Expression.And(
							Expression.NotEqual(x, NullValue),
							Expression.NotEqual(y, NullValue)
							),
						GetBaseEqualExpression(
							argColl,
							x,
							y,
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
			static int MergeCompareResult(int x,int y)
			{
				return x == 0 ? y : x;
			}
			static MethodInfo MethodMergeCompareResult = typeof(InternalComparer<T>).GetMethod("MergeCompareResult", BindingFlags.NonPublic | BindingFlags.Static);

			public InternalComparer(IComparerCollection ComparerCollection)
			{
				this.ComparerCollection = ComparerCollection;
				Func = new Lazy<Func<IComparerCollection, T, T, int>>(() =>
				{
					var argColl = Expression.Parameter(typeof(IComparerCollection));
					var argX = Expression.Parameter(typeof(T));
					var argY = Expression.Parameter(typeof(T));

					return Expression.Lambda<Func<IComparerCollection, T, T, int>>(
						GetEqualExpression(argColl,argX,argY,1),
						argColl,
						argX,
						argY
						).Compile();
					});
				}
			public int Compare(T x, T y)
			{
				return Func.Value(ComparerCollection, x, y);
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
					c = GetOrAdd(key, new InternalComparer<T>(this));
				return (IComparer<T>)c;
			}
		}
		public static IComparerCollection DeepComparers { get; } = new ComparerCollection();
		
		public static int DeepCompare<T>(T x, T y)
		{
			return DeepComparers.GetComparer<T>().Compare(x, y);
		}
		public static bool DeepEquals<T>(T x, T y)
			=> DeepCompare<T>(x, y) == 0;
	}
}

