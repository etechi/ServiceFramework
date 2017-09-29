using System.Threading.Tasks;
using System;
using SF.Core.ServiceManagement;
using System.Linq;
using SF.Data;
using System.Reflection;
using System.Linq.Expressions;
using System.Data.Common;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SF.Entities
{
	public static class Entity<T>
		where T : class
	{

		public static IReadOnlyList<PropertyInfo> KeyProperties { get; }
			= typeof(T).AllPublicInstanceProperties()
			.Where(p => p.IsDefined(typeof(KeyAttribute)))
			.ToArray();

		public static ParameterExpression ArgModel { get; } = Expression.Parameter(typeof(T));

		static Lazy<Func<T, object[]>> LazyGetIdents { get; } = new Lazy<Func<T, object[]>>(() =>
			Expression.Lambda<Func<T, object[]>>(
				Expression.NewArrayInit(
					typeof(object),
					KeyProperties.Select(
						p => Expression.Convert(
							Expression.Property(ArgModel, p),
							typeof(object)
							)
						)
					),
				ArgModel
				).Compile()
		);

		public static object[] GetIdents(T e) => LazyGetIdents.Value(e);
		public static string GetIdentString(T e) => GetIdents(e).Join(",");



		static class LazySingleKeyEntityBuilder<TKey>
		{
			public static Lazy<Func<TKey, T>> Func { get; } = new Lazy<Func< TKey,T>>(() =>
			{
				if (KeyProperties.Count != 1)
					throw new NotSupportedException($"实体{typeof(T)}有多个主键");
				var arg = Expression.Parameter(typeof(TKey));
				return Expression.Lambda<Func<TKey, T>>(
					Expression.MemberInit(
						Expression.New(typeof(T)),
						Expression.Bind(
							KeyProperties[0],
							arg
							)
						),
					arg
					).Compile();
			});
		}
		public static T WithKey<TKey>(TKey id) => LazySingleKeyEntityBuilder<TKey>.Func.Value(id);


		static class LazySingleKeySelector<TKey>
		{
			public static Lazy<Expression<Func<T, TKey>>> Expr { get; } = new Lazy<Expression<Func<T, TKey>>>(() =>
				Expression.Lambda<Func<T, TKey>>(
					Expression.Property(
						ArgModel,
						KeyProperties.Single()
						.Assert(p => p.PropertyType == typeof(TKey), p => $"主键类型不匹配，模型主键类型：{p.PropertyType},指定类型:{typeof(TKey)}")
						),
					ArgModel
				)
			);
			public static Lazy<Func<T, TKey>> Func { get; } = new Lazy<Func<T, TKey>>(() => Expr.Value.Compile());
		}
		public static Expression<Func<T, TKey>> SingleKeySelector<TKey>()
			=> LazySingleKeySelector<TKey>.Expr.Value;

		public static TKey GetSingleKey<TKey>(T Model)
			=> LazySingleKeySelector<TKey>.Func.Value(Model);

		static class LazyKeySelector<TKey>
		{
			public static Lazy<Expression<Func<T, TKey>>> Expr { get; } = new Lazy<Expression<Func<T, TKey>>>(() =>
				Expression.Lambda<Func<T, TKey>>(
					Expression.MemberInit(
						Expression.New(typeof(TKey)),
						KeyProperties.Select(p =>
							Expression.Bind(
								typeof(TKey).GetProperty(p.Name)
									.IsNotNull(() => $"{typeof(TKey)}中找不到实体{typeof(T)}的主键字段{p.Name}")
									.Assert(
										np => np.PropertyType == p.PropertyType,
										np => $"{typeof(TKey)}的字段{np.Name}的类型为{np.PropertyType}和实体{typeof(T)}主键字段{p.Name}的类型{p.PropertyType}不同"
									),
								Expression.Property(ArgModel, p)
								)
							)
					),
					ArgModel
				)
			);

			public static Lazy<Func<T, TKey>> Func { get; } = new Lazy<Func<T, TKey>>(() => Expr.Value.Compile());
		}
		public static Expression<Func<T, TKey>> KeySelector<TKey>()
			=> LazyKeySelector<TKey>.Expr.Value;
		public static TKey[] GetKeys<TKey>(T[] Models) => Models.Select(m => LazyKeySelector<TKey>.Func.Value(m)).ToArray();

		public static TKey[] GetSingleKeys<TKey>(T[] Models) => Models.Select(m => LazySingleKeySelector<TKey>.Func.Value(m)).ToArray();

		static MethodInfo MethodGetSingleKeys { get; } =
			typeof(Entity<T>)
			.GetMethods("GetSingleKeys", BindingFlags.Public | BindingFlags.Static)
			.Single();


		static Type TypeExpression { get; } = typeof(Expression);
		static MethodInfo MethodConstant { get; } = TypeExpression.GetMethod(nameof(Expression.Constant), BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(object), typeof(Type) }, null);
		static MethodInfo MethodProperty { get; } = TypeExpression.GetMethod(nameof(Expression.Property), BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(Expression), typeof(PropertyInfo) }, null);
		static MethodInfo MethodAnd { get; } = TypeExpression.GetMethod(nameof(Expression.And), BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(Expression), typeof(Expression) }, null);
		static MethodInfo MethodEquals { get; } = TypeExpression.GetMethod(nameof(Expression.Equals), BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(Expression), typeof(Expression) }, null);
		static MethodInfo MethodLambda { get; } = TypeExpression.GetMethods(nameof(Expression.Lambda), BindingFlags.Public | BindingFlags.Static).Single(m => m.IsGenericMethodDefinition);

		static class ObjectFilterCreator<TObject>
		{
			public static Lazy<Func<TObject, Expression<Func<T, bool>>>> Instance = new Lazy<Func<TObject, Expression<Func<T, bool>>>>(() =>
			{
				var objType = typeof(TObject);
				var objKeyProps = KeyProperties.Select(p =>
				{
					var op = objType.GetProperty(p.Name);
					if (op == null)
						throw new InvalidOperationException($"对象{objType}中没有实体{typeof(T)}的主键字段{p.Name}");
					if (op.PropertyType != p.PropertyType)
						throw new InvalidOperationException($"对象{objType}和实体{typeof(T)}的主键字段{p.Name}的类型不同，分别为{op.PropertyType}和{p.PropertyType}");
					return op;
				});

				//x=>x.Id1==o.Id1 && x.Id2==o.Id2

				var argObj = Expression.Parameter(typeof(TObject));
				var typeExpression = typeof(Expression);
				return Expression.Lambda<Func<TObject, Expression<Func<T, bool>>>>(
					Expression.Call(
						null,
						MethodLambda.MakeGenericMethod(typeof(Expression<Func<T, bool>>)),
						KeyProperties.Zip(objKeyProps, (kp, op) =>
							Expression.Call(
								null,
								MethodEquals,
								Expression.Call(
									null,
									MethodProperty,
									Expression.Constant(ArgModel),
									Expression.Constant(kp.PropertyType)
									),
								Expression.Call(
									null,
									MethodConstant,
									Expression.Property(argObj, op),
									Expression.Constant(op.PropertyType)
									)
								)
							).Aggregate((x, y) =>
								Expression.Call(
									null,
									MethodAnd,
									x,
									y
									)
								),
							Expression.NewArrayInit(
								typeof(ParameterExpression),
								Expression.Constant(ArgModel)
								)
							),
					argObj
					).Compile();
			});
		} 
		public static Expression<Func<T,bool>> ObjectFilter<TObj>(TObj Obj)
			=>ObjectFilterCreator<TObj>.Instance.Value(Obj);

		public static MethodInfo MethodContains { get; } = 
			typeof(IEnumerable<>)
			.GetMethods("Contains", BindingFlags.Public | BindingFlags.Static)
			.Single(m => m.IsGenericMethodDefinition && m.GetParameters().Length == 1);



		static class MultipleObjectFilterCreator<TObject>
		{
			public static Lazy<Func<TObject[], Expression<Func<T, bool>>>> Instance = new Lazy<Func<TObject[], Expression<Func<T, bool>>>>(() =>
			{
				if (KeyProperties.Count != 1)
					throw new NotSupportedException($"对象{typeof(T)}有多个主键，不支持批量主键查询");

				var p = KeyProperties[0];
				var keyType = p.PropertyType;
				var objType = typeof(TObject);
				var op = objType.GetProperty(p.Name);
				if (op == null)
					throw new InvalidOperationException($"对象{objType}中没有实体{typeof(T)}的主键字段{p.Name}");
				if (op.PropertyType != p.PropertyType)
					throw new InvalidOperationException($"对象{objType}和实体{typeof(T)}的主键字段{p.Name}的类型不同，分别为{op.PropertyType}和{p.PropertyType}");

				//x=>Ids.Contains(x.Id)

				var argObjs = Expression.Parameter(typeof(TObject).MakeArrayType());
				return Expression.Lambda<Func<TObject[], Expression<Func<T, bool>>>>(
					Expression.Call(
						null,
						MethodMultipleKeyFilter.MakeGenericMethod(keyType),
						Expression.Call(
							null,
							MethodGetSingleKeys.MakeGenericMethod(keyType),
							argObjs
							)
						),
					argObjs
				).Compile();
			});
		}
		public static Expression<Func<T, bool>> MultipleObjectFilter<TObject>(TObject[] Keys)
			=> MultipleObjectFilterCreator<TObject>.Instance.Value(Keys);

		static MethodInfo MethodMultipleKeyFilter = typeof(Entity<T>).GetMethods("MultipleKeyFilter", BindingFlags.Public | BindingFlags.Static).Single();
		public static Expression<Func<T,bool>> MultipleKeyFilter<TKey>(TKey[] Keys) 
			where TKey : IEquatable<TKey>
			=> Expression.Lambda<Func<T, bool>>(
				Expression.Call(
					null,
					MethodContains.MakeGenericMethod(typeof(TKey)),
					Expression.Constant(Keys),
					Expression.Property(
						ArgModel,
						KeyProperties.Single()
						.Assert(p => p.PropertyType == typeof(TKey), p => $"主键类型不匹配，模型主键类型：{p.PropertyType},指定类型:{typeof(TKey)}")
						)
				),
				ArgModel
				);

		public static Expression<Func<T, bool>> KeyFilter<TKey>(TKey Id)  where TKey:IEquatable<TKey>
			=> Expression.Lambda<Func<T, bool>>(
				Expression.Equal(
					Expression.Constant(Id),
					Expression.Property(
						ArgModel,
						KeyProperties.Single()
						.Assert(p => p.PropertyType == typeof(TKey), p => $"主键类型不匹配，模型主键类型：{p.PropertyType},指定类型:{typeof(TKey)}")
						)
				),
				ArgModel
				);

		static IContextQueryable<T> QueryIdentFilter<TKey, TQueryArgument>(IContextQueryable<T> q,TQueryArgument a)
			where TQueryArgument:IQueryArgument<TKey>
			where TKey:IEquatable<TKey>
		{
			if (a.Id.HasValue)
				return q.Where(KeyFilter(a.Id.Value));
			return q;
		}

		static MethodInfo MethodQueryIdentFilter { get; } = typeof(Entity<T>).GetMethods("QueryIdentFilter", BindingFlags.Static | BindingFlags.NonPublic).Single();

		static class IdentFilter<TQueryArgument>
		{
			public static Lazy<Func<IContextQueryable<T>, TQueryArgument, IContextQueryable<T>>> Instance { get; } = 
				new Lazy<Func<IContextQueryable<T>, TQueryArgument, IContextQueryable<T>>>(
				() =>
				{
					var iqa = typeof(TQueryArgument)
						.GetInterfaces()
						.FirstOrDefault(i => i.IsGeneric() && i.GetGenericTypeDefinition() == typeof(IQueryArgument<>));
					if (iqa == null)
						return (q, a) => q;
					if (iqa.GenericTypeArguments[0] != KeyProperties.Single().PropertyType)
						throw new InvalidOperationException($"指定查询参数{typeof(TQueryArgument)}的主键类型{iqa.GenericTypeArguments[0]}和模型{typeof(T)}主键{KeyProperties[0].Name}字段类型{KeyProperties[0].PropertyType}不符");
					var func = MethodQueryIdentFilter
						.MakeGenericMethod(iqa.GenericTypeArguments[0], typeof(TQueryArgument))
						.CreateDelegate<Func<IContextQueryable<T>, TQueryArgument, IContextQueryable<T>>>();
					return func;
				});
		}
		public static IContextQueryable<T> TryFilterIdent<TQueryArgument>(IContextQueryable<T> q,TQueryArgument arg)
		{
			return IdentFilter<TQueryArgument>.Instance.Value(q, arg);
		}


	}
}
