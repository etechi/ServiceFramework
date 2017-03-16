using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Data.Storage;
using System.Linq.Expressions;
using System.Reflection;
namespace SF.Data.Entity
{
	public static class EntityMapper
	{

		class Mapper<S, T>
		{
			public static Lazy<Expression<Func<S, T>>> Expr { get; } = new Lazy<Expression<Func<S, T>>>(() =>
				{
					var arg = Expression.Parameter(typeof(S), "src");
					var dstType = typeof(T);
					var srcType = typeof(S);
					return Expression.Lambda<Func<S, T>>(
						Expression.MemberInit(
						  Expression.New(typeof(T)),
						  (from dp in dstType.GetProperties(System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
						   let sp = srcType.GetProperty(dp.Name, System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
						   where sp != null
						   select Expression.Bind(
							   dp,
							   Expression.Property(
								   arg,
								   sp
								   )
							   )
						  )
						  ),
						arg
					  );
				});
			public static Lazy<Func<S, T>> Func { get; } = new Lazy<Func<S, T>>(() =>
				Expr.Value.Compile()
			);

		}
		public static Expression<Func<S,T>> Map<S, T>()
		{
			return Mapper<S, T>.Expr.Value;
		}
		public static T Map<S, T>(S src)
		{
			return Mapper<S, T>.Func.Value(src);
		}
	}
   
}