using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;

namespace SF.Core.ManagedServices.Runtime
{
	delegate object ServiceFactory(
		IServiceProvider ServiceProvider,
		IManagedServiceScope ServiceScope,
		IServiceCreateParameterTemplate ParameterTemplate
		);

	class ServiceFactoryBuilder
	{
		static ParameterExpression ParamServiceProvider = Expression.Parameter(typeof(IServiceProvider), "sp");
		static MethodInfo ServiceProviderResolve = typeof(IServiceProvider).GetMethod("GetService", BindingFlags.Public | BindingFlags.Instance);

		static ParameterExpression ParamManagedNamedServiceScope = Expression.Parameter(typeof(IManagedServiceScope), "mnss");
		static MethodInfo ManagedNamedServiceScopeResolve = typeof(IManagedServiceScope).GetMethod("Resolve", BindingFlags.Public | BindingFlags.Instance );

		static ParameterExpression ParamServiceCreateParameterProvider = Expression.Parameter(typeof(IServiceCreateParameterTemplate), "ap");
		static MethodInfo ServiceCreateParameterProviderGetParameter = typeof(IServiceCreateParameterTemplate).GetMethod("GetArgument", BindingFlags.Public | BindingFlags.Instance);
		static MethodInfo ServiceCreateParameterProviderGetServiceIdent = typeof(IServiceCreateParameterTemplate).GetMethod("GetServiceIdent", BindingFlags.Public | BindingFlags.Instance );
		static MethodInfo ServiceCreateParameterProviderGetServiceInstanceIdent = typeof(IServiceCreateParameterTemplate).GetMethod("GetServiceInstanceIdent", BindingFlags.Public | BindingFlags.Instance );

		static MethodInfo StringConcat = typeof(String).GetMethods().Single(m => m.Name == "Concat" && m.IsStatic && m.IsPublic && m.GetParameters().Length == 2 && m.GetParameters().All(pt=>pt.ParameterType== typeof(string)));
		static MethodInfo StringConcat4 = typeof(String).GetMethods().Single(m => m.Name == "Concat" && m.IsStatic && m.IsPublic && m.GetParameters().Length == 4 && m.GetParameters().All(pt => pt.ParameterType == typeof(string)));
		static MethodInfo Int32ToString = typeof(int).GetMethods().Single(m => m.Name == "ToString" && m.IsPublic && !m.IsStatic && m.GetParameters().Length == 0);
		enum ResolveType
		{
			Instance,
			Lazy,
			Func
		}
		static Expression ServiceProviderResolveExpression(Type Type,ResolveType resolveType) =>
			Expression.Convert(
				Expression.Call(
					ParamServiceProvider,
					ServiceProviderResolve,
					Expression.Constant(Type)
					),
				Type
				);


		static Lazy<T> CreateLazyedManagedServiceResolve<T>(
			IManagedServiceScope scope,
			IServiceCreateParameterTemplate CreateParameterTemplate,
			string Path
			)=>new Lazy<T>(() =>
				(T)scope.Resolve(typeof(T), CreateParameterTemplate.GetServiceIdent(Path))
			);

		static MethodInfo CreateLazyedManagedServiceResolveMethodInfo = typeof(ServiceFactoryBuilder).GetMethodExt(
			nameof(CreateLazyedManagedServiceResolve),
			typeof(IManagedServiceScope),
			typeof(IServiceCreateParameterTemplate),
			typeof(string)
			);
		static Func<T> CreateFuncManagedServiceResolve<T>(
			IManagedServiceScope scope,
			IServiceCreateParameterTemplate CreateParameterTemplate,
			string Path
			) => () =>
				  (T)scope.Resolve(typeof(T), CreateParameterTemplate.GetServiceIdent(Path))
			;

		static MethodInfo CreateFuncManagedServiceResolveMethodInfo = typeof(ServiceFactoryBuilder).GetMethodExt(
			nameof(CreateFuncManagedServiceResolve),
			typeof(IManagedServiceScope),
			typeof(IServiceCreateParameterTemplate),
			typeof(string)
			);
		static Expression ManagedServiceResolveExpression(Type Type, Expression PropPathExpr,ResolveType resolveType) =>
			resolveType==ResolveType.Lazy?
			(Expression)Expression.Call(
				CreateLazyedManagedServiceResolveMethodInfo.MakeGenericMethod(Type.GetGenericArgumentTypeAsLazy()),
				ParamManagedNamedServiceScope,
				ParamServiceCreateParameterProvider,
				PropPathExpr
				):
			resolveType==ResolveType.Func?
			(Expression)Expression.Call(
				CreateFuncManagedServiceResolveMethodInfo.MakeGenericMethod(Type.GetGenericArgumentTypeAsFunc()),
				ParamManagedNamedServiceScope,
				ParamServiceCreateParameterProvider,
				PropPathExpr
				) :
			Expression.Convert(
				Expression.Call(
					ParamManagedNamedServiceScope,
					ManagedNamedServiceScopeResolve,
					Expression.Constant(Type),
					Expression.Call(
						ParamServiceCreateParameterProvider,
						ServiceCreateParameterProviderGetServiceIdent,
						PropPathExpr
					)
				),
			Type
			);
		static Expression GetServiceInstanceIdentExpression() =>
			Expression.Call(
				ParamServiceCreateParameterProvider,
				ServiceCreateParameterProviderGetServiceInstanceIdent
			);

		static Expression StrConcat(Expression e1, Expression e2) =>
			Expression.Call(null, StringConcat, e1, e2);

		public static ConstructorInfo FindBestConstructorInfo(Type Type)
		{
			var constructors = Type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
				.ToArray();


			if (constructors.Length == 0)
				throw new InvalidOperationException($"找不到构造函数:{Type}");

			if (constructors.Length == 1)
				return constructors[0];

			Array.Sort(constructors,
				(a, b) => b.GetParameters().Length.CompareTo(a.GetParameters().Length));

			var bestConstructor = constructors[0];
			var bestConstructorParameterTypes = new HashSet<Type>(bestConstructor.GetParameters().Select(p => p.ParameterType));
			for (var i = 1; i < constructors.Length; i++)
			{
				var parameters = constructors[i].GetParameters();
				if (!bestConstructorParameterTypes.IsSupersetOf(parameters.Select(p => p.ParameterType)))
					throw new InvalidOperationException($"服务构造函数冲突:{bestConstructor.GetParameters()}和{constructors[i].GetParameters()}");
			}
			return bestConstructor;
		}


		class InitCopyRequiredPathsHelper
		{
			public HashSet<string> CopyRequiredPaths;
			public IServiceDetector ServiceDetector;
			public Type ImplementType;
			public ConstructorInfo constructorInfo;

			HashSet<Type> TypePath = new HashSet<Type>();
			List<string> PathList = new List<string>();
			void DetectCopyRequiredPath(Type ot)
			{
				if (!TypePath.Add(ot))
					throw new NotSupportedException($"配置类型循环依赖:根类型{ImplementType} 当期类型:{ot}");
				try
				{
					var t = ot;
					if (t.IsArray)
						t = t.GetElementType();
					else if (t.IsGeneric())
					{
						var gtd = t.GetGenericTypeDefinition();
						if (gtd == typeof(IEnumerable<>))
							t = t.GetGenericArguments()[0];
						else if (gtd == typeof(Dictionary<,>))
							t = t.GetGenericArguments()[1];
						else if (ServiceDetector.IsServiceType(gtd))
						{
							for (var i = 0; i < PathList.Count; i++)
								CopyRequiredPaths.Add(string.Join(".", PathList.Take(i + 1)));
							return;
						}
					}
					if (ServiceDetector.IsServiceType(t))
					{
						for (var i = 0; i < PathList.Count; i++)
							CopyRequiredPaths.Add(string.Join(".", PathList.Take(i + 1)));
						return;
					}
					if (t.IsPrimitiveType() || t.GetTypeCode()!=TypeCode.Object)
						return;

					foreach (var pi in t.GetProperties(BindingFlags.Public | BindingFlags.Instance ))
					{
						PathList.Add(pi.Name);
						DetectCopyRequiredPath(pi.PropertyType);
						PathList.RemoveAt(PathList.Count - 1);
					}
				}
				finally
				{
					TypePath.Remove(ot);
				}
			}
			public void InitCopyRequiredPaths()
			{
				var ps = constructorInfo.GetParameters();
				PathList.Add(null);
				for (var i = 0; i < ps.Length; i++)
				{
					PathList[0] = ps[i].Name;
					DetectCopyRequiredPath(ps[i].ParameterType);
				}
			}
		}

		class Builder
		{
			public HashSet<string> CopyRequiredPaths;
			public IServiceDetector ServiceDetector;
			public Type ImplementType;
			public ConstructorInfo constructorInfo;

			Expression EnumerableCopyExpression(Expression src, Type ElementType, Expression PropPathExpr, string PropPath)
			{
				var listType = typeof(List<>).MakeGenericType(ElementType);
				var list = listType.AsVariable();
				var idx = typeof(int).AsVariable();
				var path = typeof(string).AsVariable();
				return Expression.Block(
					new  [] { list, idx,path }.Cast<ParameterExpression>(),
					list.Assign(listType.Create()),
					src.To(typeof(IEnumerable<>).MakeGenericType(ElementType))
						.ForEach(ElementType, item =>
						 Expression.Block(
							 Expression.Assign(
								path,
								Expression.Call(
									null,
									StringConcat4,
									PropPathExpr,
									Expression.Constant("["),
									idx.CallMethod(Int32ToString),
									Expression.Constant("]")
									)
								),
							list.CallMethod(
								"Add",
								CopyExpression(ElementType, item, path, PropPath)
								),
							Expression.AddAssign(idx, Expression.Constant(1))
							)),
					list
				);
			}
			Expression DictionaryCopyExpression(Expression src,Type KeyType, Type ElementType, Expression PropPathExpr, string PropPath)
			{
				var dictType = typeof(Dictionary<,>).MakeGenericType(KeyType,ElementType);
				var dict = dictType.AsVariable();
				var path = typeof(string).AsVariable();
				return Expression.Block(
					new[] { dict,  path }.Cast<ParameterExpression>(),
					dict.Assign(dictType.Create()),
					src.To(typeof(IEnumerable<>).MakeGenericType(typeof(KeyValuePair<,>).MakeGenericType(KeyType,ElementType)))
					.ForEach(typeof(KeyValuePair<,>).MakeGenericType(KeyType, ElementType), item =>
						 Expression.Block(
							 Expression.Assign(
								path,
								Expression.Call(
									null,
									StringConcat4,
									PropPathExpr,
									Expression.Constant("["),
									item.GetMember("Key"),
									Expression.Constant("]")
									)
								),
							dict.CallMethod(
								"Add",
								item.GetMember("Key"),
								CopyExpression(ElementType, item.GetMember("Value"), path, PropPath)
								)
							)),
					dict
				);
			}
			ServiceType GetServiceType(IServiceDetector ServiceDetector, Type Type, out ResolveType ResolveType)
			{
				var RealType = Type.GetGenericArgumentTypeAsLazy();
				if (RealType != null)
					ResolveType = ResolveType.Lazy;
				else
				{
					RealType = Type.GetGenericArgumentTypeAsFunc();
					if (RealType != null)
						ResolveType = ResolveType.Func;
					else
					{
						ResolveType = ResolveType.Instance;
						RealType = Type;
					}
				}
				return ServiceDetector.GetServiceType(RealType);
			}
			static Dictionary<K,T> CopyDirectionary<K,T>(Dictionary<K, T> dict)
			{
				return dict.ToDictionary(p => p.Key, p => p.Value);
			}
			Expression CopyExpression(Type Type, Expression src, Expression PropPathExpr, string PropPath)
			{
				if (!CopyRequiredPaths.Contains(PropPath))
					return src;

				if (Type.IsArray)
				{
					return Expression.Call(
						EnumerableCopyExpression(
							src,
							Type.GetElementType(),
							PropPathExpr,
							PropPath
						),
						typeof(List<>).MakeGenericType(Type.GetElementType()).GetMethod("ToArray", BindingFlags.Public | BindingFlags.Instance )
					);
				}
				else if(Type.IsGeneric() && Type.GetGenericTypeDefinition()==typeof(Dictionary<,>))
				{
					var gtype = Type.GetGenericArguments();
					return DictionaryCopyExpression(
							src,
							gtype[0],
							gtype[1],
							PropPathExpr,
							PropPath
						);
				}
				else if (Type.IsGeneric() && Type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				{
					return Expression.Convert(
						EnumerableCopyExpression(src, Type.GetGenericArguments()[0], PropPathExpr, PropPath),
						Type
						);
				}
				else
				{
					ResolveType rType;
					var svcType = GetServiceType(ServiceDetector,Type,out rType);
					if (svcType == Runtime.ServiceType.Managed)
						return ManagedServiceResolveExpression(Type, PropPathExpr, rType);
					else if (svcType == Runtime.ServiceType.Normal)
						return ServiceProviderResolveExpression(Type, rType);
					else if (Type == typeof(ManagedServices.IServiceInstanceIdent))
						return GetServiceInstanceIdentExpression();
				}

				return ObjectCreateExpression(
					Type,
					src,
					PropPathExpr,
					PropPath
					);
			}

			Expression PropCopyExpression(Type Type, Expression src, PropertyInfo pi, Expression PropPathExpr, string PropPath)
			{
				return CopyExpression(pi.PropertyType, Expression.Property(src, pi), StrConcat(PropPathExpr, Expression.Constant("." + pi.Name)), PropPath + "." + pi.Name);
			}
			IEnumerable<MemberBinding> MemberInitExpressions(Type Type, Expression src, Expression PropPathExpr, string PropPath)
			{
				return Type.GetProperties(BindingFlags.Public | BindingFlags.Instance| BindingFlags.FlattenHierarchy)
					.Select(pi => Expression.Bind(pi, PropCopyExpression(Type, src, pi, PropPathExpr, PropPath)));
			}
			Expression ObjectCreateExpression(Type Type, Expression Src, Expression PropPathExpr, string PropPath)
			{
				return Expression.MemberInit(Expression.New(Type), MemberInitExpressions(Type, Src, PropPathExpr, PropPath));
			}

			Expression BuildParamExpression(ParameterInfo pi, int Index)
			{
				ResolveType resolveType;
				var svcType = GetServiceType(ServiceDetector,pi.ParameterType,out resolveType);
				if (svcType == Runtime.ServiceType.Normal)
					return ServiceProviderResolveExpression(pi.ParameterType, resolveType);
				else if (svcType == Runtime.ServiceType.Managed)
					return ManagedServiceResolveExpression(pi.ParameterType, Expression.Constant(pi.Name), resolveType);
				else if (pi.ParameterType == typeof(ManagedServices.IServiceInstanceIdent))
					return GetServiceInstanceIdentExpression();
				else
				{
					if (!CopyRequiredPaths.Contains(pi.Name))
						return Expression.Convert(
							Expression.Call(
								ParamServiceCreateParameterProvider,
								ServiceCreateParameterProviderGetParameter,
								Expression.Constant(Index)
								),
							pi.ParameterType
							);

					var src = Expression.Parameter(pi.ParameterType);
					return Expression.Block(
						new[] { src },
						Expression.Assign(
							src,
							Expression.Convert(
								Expression.Call(
									ParamServiceCreateParameterProvider,
									ServiceCreateParameterProviderGetParameter,
									Expression.Constant(Index)
									),
								pi.ParameterType
							)
						),
						Expression.Convert(
							ObjectCreateExpression(
								pi.ParameterType,
								src,
								Expression.Constant(pi.Name),
								pi.Name
								),
							pi.ParameterType
							)
						);
				}
			}
			public ServiceFactory Build()
			{ 
				var args = constructorInfo.GetParameters().Select((pi, i) => BuildParamExpression(pi, i)).ToArray();
				return Expression.Lambda<ServiceFactory>(
					Expression.New(constructorInfo, args),
					ParamServiceProvider,
					ParamManagedNamedServiceScope,
					ParamServiceCreateParameterProvider
					).Compile();
			}
		}
		public static ServiceFactory Build(
			IServiceDetector ServiceDetector,
			Type ImplementType,
			ConstructorInfo constructorInfo
			)
		{
			var CopyRequiredPaths = new HashSet<string>();
			new InitCopyRequiredPathsHelper
			{
				constructorInfo = constructorInfo,
				CopyRequiredPaths = CopyRequiredPaths,
				ImplementType = ImplementType,
				ServiceDetector = ServiceDetector
			}.InitCopyRequiredPaths();

			return new Builder
			{
				constructorInfo = constructorInfo,
				CopyRequiredPaths = CopyRequiredPaths,
				ImplementType = ImplementType,
				ServiceDetector = ServiceDetector
			}.Build();
		}
	}
}
