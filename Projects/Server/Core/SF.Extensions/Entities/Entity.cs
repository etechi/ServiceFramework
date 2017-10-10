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
	{
		public static IReadOnlyList<PropertyInfo> KeyProperties { get; }
			= typeof(T).AllPublicInstanceProperties()
			.Where(p => p.IsDefined(typeof(KeyAttribute)))
			.ToArray();


		public static class KeyValidator<K>
		{
			public static bool HasSameKeys { get; }
			static KeyValidator()
			{
				var kks = Entity<K>.KeyProperties;
				HasSameKeys = KeyProperties.Count == kks.Count && 
					KeyProperties
					.Zip(kks, (x, y) => x.Name == y.Name && x.PropertyType == y.PropertyType)
					.All(a => a);
			}
		}
		public static bool HasSameKeys<K>() => KeyValidator<K>.HasSameKeys;
		public static void EnsureSameKeys<K>()
		{
			if (!HasSameKeys<K>())
				throw new NotSupportedException($"类型{typeof(T)}和类型{typeof(K)}主键不同");
		}
		public static ParameterExpression ArgModel { get; } = Expression.Parameter(typeof(T));

		static Lazy<Func<T, object[]>> LazyGetIdents { get; } = new Lazy<Func<T, object[]>>(() =>
			Expression.Lambda<Func<T, object[]>>(
				ArgModel.WithNullDefault(
					expr=>Expression.NewArrayInit(
						typeof(object),
						KeyProperties.Select(
							p => Expression.Convert(
								Expression.Property(expr, p),
								typeof(object)
								)
							)
						)
				),
				ArgModel
				).Compile()
		);

		public static object[] GetIdents(T e) => LazyGetIdents.Value(e);
		public static string GetIdentString(T e) => GetIdents(e)?.Join(",");



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
			{
				EnsureSameKeys<TKey>();
				return Expression.Lambda<Func<T, TKey>>(
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
				);
			});

			public static Lazy<Func<T, TKey>> Func { get; } = new Lazy<Func<T, TKey>>(() => Expr.Value.Compile());
		}
		public static Expression<Func<T, TKey>> KeySelector<TKey>()
			=> LazyKeySelector<TKey>.Expr.Value;
		public static TKey[] GetKeys<TKey>(T[] Models) => Models.Select(m => LazyKeySelector<TKey>.Func.Value(m)).ToArray();

		public static TKey[] GetSingleKeys<TKey>(T[] Models) => Models.Select(m => LazySingleKeySelector<TKey>.Func.Value(m)).ToArray();

		public static TKey GetKey<TKey>(T Model) => LazyKeySelector<TKey>.Func.Value(Model);

		static MethodInfo MethodGetSingleKeys { get; } =
			typeof(Entity<T>)
			.GetMethods("GetSingleKeys", BindingFlags.Public | BindingFlags.Static)
			.Single();


		static Type TypeExpression { get; } = typeof(Expression);
		static MethodInfo MethodConstant { get; } = TypeExpression.GetMethod(nameof(Expression.Constant), BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(object), typeof(Type) }, null);
		static MethodInfo MethodProperty { get; } = TypeExpression.GetMethod(nameof(Expression.Property), BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(Expression), typeof(PropertyInfo) }, null);
		static MethodInfo MethodAndAlso { get; } = TypeExpression.GetMethod(nameof(Expression.AndAlso), BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(Expression), typeof(Expression) }, null);
		static MethodInfo MethodEqual { get; } = TypeExpression.GetMethod(nameof(Expression.Equal), BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(Expression), typeof(Expression) }, null);
		static MethodInfo MethodLambda { get; } = TypeExpression.GetMethods(
			nameof(Expression.Lambda),
			BindingFlags.Public | BindingFlags.Static
			).Single(m =>
			{
				if (!m.IsGenericMethodDefinition)
					return false;
				var ps = m.GetParameters();
				return ps.Length == 2 && ps[0].ParameterType == typeof(Expression) && ps[1].ParameterType == typeof(IEnumerable<ParameterExpression>);
			});

		
		static class ObjectFilterCreator<TObject>
		{
			public static Lazy<Func<TObject, Expression<Func<T, bool>>>> Instance = new Lazy<Func<TObject, Expression<Func<T, bool>>>>(() =>
			{
				EnsureSameKeys<TObject>();
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
						MethodLambda.MakeGenericMethod(typeof(Func<T, bool>)),
						KeyProperties.Zip(objKeyProps, (kp, op) =>
							Expression.Call(
								null,
								MethodEqual,
								Expression.Call(
									null,
									MethodProperty,
									Expression.Constant(ArgModel),
									Expression.Constant(kp)
									),
								Expression.Call(
									null,
									MethodConstant,
									Expression.Convert(Expression.Property(argObj, op),typeof(object)),
									Expression.Constant(op.PropertyType)
									)
								)
							).Aggregate((x, y) =>
								Expression.Call(
									null,
									MethodAndAlso,
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

		static MethodInfo MethodObjectFilter { get; } = typeof(Entity<T>).GetMethods(
			nameof(Entity < T > .ObjectFilter),
			BindingFlags.Static | BindingFlags.Public
			).Single();

		public static MethodInfo MethodContains { get; } = 
			typeof(Enumerable)
			.GetMethods("Contains", BindingFlags.Public | BindingFlags.Static)
			.Single(m => m.IsGenericMethodDefinition && m.GetParameters().Length == 2);



		static class MultipleObjectFilterCreator<TObject>
		{
			public static Lazy<Func<TObject[], Expression<Func<T, bool>>>> Instance = new Lazy<Func<TObject[], Expression<Func<T, bool>>>>(() =>
			{
				EnsureSameKeys<TObject>();

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
							Entity<TObject>.MethodGetSingleKeys.MakeGenericMethod(keyType),
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

		static MethodInfo MethodWhere { get; } = typeof(ContextQueryable).GetMethodExt(
			nameof(ContextQueryable.Where),
			BindingFlags.Static | BindingFlags.Public,
			typeof(IContextQueryable<>).MakeGenericType<System.Reflection.TypeExtension.GenericTypeArgument>(),
			typeof(Expression<>).MakeGenericType(
				typeof(Func<,>).MakeGenericType(
					typeof(System.Reflection.TypeExtension.GenericTypeArgument),
					typeof(bool)
				)
			)
			);

		public static PropertyInfo GetQueryArgumentIdentProperty<TQueryArgument>()
		{
			return QueryArgumentIdent<TQueryArgument>.IdentProperty.Value;
		}
		static class QueryArgumentIdent<QueryArgument>
		{
			public static Lazy<PropertyInfo> IdentProperty { get; } = new Lazy<PropertyInfo>(() =>
			 {
				 var re = typeof(QueryArgument).AllPublicInstanceProperties().Where(
							  qap =>
							 {
								 if (!qap.PropertyType.IsClass)
									 return false;
								 var ks = qap.PropertyType.AllPublicInstanceProperties().Where(ip =>
								 ip.GetCustomAttribute<KeyAttribute>() != null
								 ).ToArray();
								 if (ks.Length != KeyProperties.Count) return false;
								 return ks.Zip(KeyProperties, (x, y) => x.Name == y.Name && x.PropertyType == y.PropertyType).All(a => a);
							 }).ToArray();
				 if (re.Length == 0)
					 return null;
				 return re.Length == 1 ? re[0] : re.FirstOrDefault(ip => ip.Name == "Id") ?? re[0];
			 });

			public static Lazy<Func<IContextQueryable<T>, QueryArgument, IContextQueryable<T>>> Func = new Lazy<Func<IContextQueryable<T>, QueryArgument, IContextQueryable<T>>>(() =>
			{
				var p = IdentProperty.Value;
				if (p == null)
					return (q, a) => q; 
				var ArgQueryable = Expression.Parameter(typeof(IContextQueryable<T>));
				var ArgQA = Expression.Parameter(typeof(QueryArgument));
				return Expression.Lambda<Func<IContextQueryable<T>, QueryArgument, IContextQueryable<T>>>(
					Expression.Condition(
						ArgQA.GetMember(p).Equal(Expression.Constant(null, p.PropertyType)),
						ArgQueryable,
						Expression.Call(
							null,
							MethodWhere.MakeGenericMethod(typeof(T)),
							ArgQueryable,
							Expression.Call(
								null,
								MethodObjectFilter.MakeGenericMethod(p.PropertyType),
								ArgQA.GetMember(p)
								)
						)
					),
					ArgQueryable,
					ArgQA
					).Compile();
			});
		}
		public static IContextQueryable<T> QueryIdentFilter<TQueryArgument>(IContextQueryable<T> q,TQueryArgument a)
		{
			return QueryArgumentIdent<TQueryArgument>.Func.Value(q, a);
		}

		//static MethodInfo MethodQueryIdentFilter { get; } = typeof(Entity<T>).GetMethods("QueryIdentFilter", BindingFlags.Static | BindingFlags.NonPublic).Single();

		//static class IdentFilter<TQueryArgument>
		//{
		//	public static Lazy<Func<IContextQueryable<T>, TQueryArgument, IContextQueryable<T>>> Instance { get; } = 
		//		new Lazy<Func<IContextQueryable<T>, TQueryArgument, IContextQueryable<T>>>(
		//		() =>
		//		{
		//			var iqa = typeof(TQueryArgument)
		//				.GetInterfaces()
		//				.FirstOrDefault(i => i.IsGeneric() && i.GetGenericTypeDefinition() == typeof(IQueryArgument<>));
		//			if (iqa == null)
		//				return (q, a) => q;
		//			if (iqa.GenericTypeArguments[0] != KeyProperties.Single().PropertyType)
		//				throw new InvalidOperationException($"指定查询参数{typeof(TQueryArgument)}的主键类型{iqa.GenericTypeArguments[0]}和模型{typeof(T)}主键{KeyProperties[0].Name}字段类型{KeyProperties[0].PropertyType}不符");
		//			var func = MethodQueryIdentFilter
		//				.MakeGenericMethod(iqa.GenericTypeArguments[0], typeof(TQueryArgument))
		//				.CreateDelegate<Func<IContextQueryable<T>, TQueryArgument, IContextQueryable<T>>>();
		//			return func;
		//		});
		//}
		//public static IContextQueryable<T> TryFilterIdent<TQueryArgument>(IContextQueryable<T> q,TQueryArgument arg)
		//{
		//	return IdentFilter<TQueryArgument>.Instance.Value(q, arg);
		//}


	}
}
