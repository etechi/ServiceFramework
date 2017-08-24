using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Data;
using System.Linq.Expressions;
using System.Reflection;
namespace SF.Entities
{
	public static class EntityMapper
	{

		class Mapper<S, T>
		{
			static Type SrcType { get; } = typeof(S);
			static Type DstType { get; } = typeof(T);
			static PropertyInfo[] dstTypeProps { get; } =
				DstType.GetProperties(System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
			static Dictionary<string, PropertyInfo> srcTypeProps { get; } =
				SrcType.GetProperties(System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
				.ToDictionary(p => p.Name);

			public static Lazy<Expression<Func<S, T>>> Expr { get; } = new Lazy<Expression<Func<S, T>>>(() =>
				{
					var arg = Expression.Parameter(typeof(S), "src");
					return Expression.Lambda<Func<S, T>>(
						Expression.MemberInit(
						  Expression.New(DstType),
						  (from dp in dstTypeProps
						   let sp = srcTypeProps.Get(dp.Name)
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