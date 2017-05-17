using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;

namespace SF.Core.ServiceManagement.Internals
{
	delegate object ServiceCreator(
		IServiceResolver ServiceProvider,
		IServiceCreateParameterTemplate ParameterTemplate
		);

	class ServiceCreatorBuilder
	{
		static ParameterExpression ParamServiceResolver = Expression.Parameter(typeof(IServiceResolver), "sr");
		static MethodInfo ServiceResolve = typeof(IServiceResolver).GetMethod("Resolve", BindingFlags.Public | BindingFlags.Instance);


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
			Func,
			Option
		}
		static Expression ServiceProviderResolveExpression(Type Type,ResolveType resolveType) =>
			Expression.Convert(
				Expression.Call(
					ParamServiceResolver,
					ServiceResolve,
					Expression.Constant(Type),
					Expression.Constant(0L)
					),
				Type
				);

		static object ResolveManagedService(
			IServiceResolver Resolver,
			IServiceCreateParameterTemplate CreateParameterTemplate,
			string Path,
			Type InterfaceType
			)
		{
			var pair = CreateParameterTemplate.GetServiceIdent(Path);
			//if(pair.Key==null)

			//if (pair.Key == null)
			//	throw new NotImplementedException($"找不到服务{InterfaceType}，配置路径:{Path}");

			if (pair.Key!=null && pair.Key != InterfaceType.FullName)
				throw new NotSupportedException($"找不到服务{InterfaceType}的配置路径:{Path}返回的服务类型为{pair.Key}不是{InterfaceType}");
			var re= Resolver.Resolve(InterfaceType, pair.Value);
			if (re == null)
				throw new NotSupportedException($"找不到服务类型为{InterfaceType},ID为{pair.Value}的服务");
			return re;
		}
		static MethodInfo ResolveManagedServiceMethodInfo = typeof(ServiceCreatorBuilder).GetMethodExt(
			nameof(ResolveManagedService),
			typeof(IServiceResolver),
			typeof(IServiceCreateParameterTemplate),
			typeof(string),
			typeof(Type)
			);
		static Lazy<T> CreateLazyedManagedServiceResolve<T>(
			IServiceResolver Resolver,
			IServiceCreateParameterTemplate CreateParameterTemplate,
			string Path
			)=>new Lazy<T>(() => (T)ResolveManagedService(Resolver, CreateParameterTemplate,Path,typeof(T)));

		static MethodInfo CreateLazyedManagedServiceResolveMethodInfo = typeof(ServiceCreatorBuilder).GetMethodExt(
			nameof(CreateLazyedManagedServiceResolve),
			typeof(IServiceResolver),
			typeof(IServiceCreateParameterTemplate),
			typeof(string)
			);
		
		static Func<T> CreateFuncManagedServiceResolve<T>(
			IServiceResolver resolver,
			IServiceCreateParameterTemplate CreateParameterTemplate,
			string Path
			) => () =>
				(T)ResolveManagedService(resolver,CreateParameterTemplate,Path,typeof(T))
			;

		static MethodInfo CreateFuncManagedServiceResolveMethodInfo = typeof(ServiceCreatorBuilder).GetMethodExt(
			nameof(CreateFuncManagedServiceResolve),
			typeof(IServiceResolver),
			typeof(IServiceCreateParameterTemplate),
			typeof(string)
			);
		static Expression ManagedServiceResolveExpression(Type Type, Expression PropPathExpr,ResolveType resolveType) =>
			resolveType==ResolveType.Lazy?
			(Expression)Expression.Call(
				CreateLazyedManagedServiceResolveMethodInfo.MakeGenericMethod(Type.GetGenericArgumentTypeAsLazy()),
				ParamServiceResolver,
				ParamServiceCreateParameterProvider,
				PropPathExpr
				):
			resolveType==ResolveType.Func?
			(Expression)Expression.Call(
				CreateFuncManagedServiceResolveMethodInfo.MakeGenericMethod(Type.GetGenericArgumentTypeAsFunc()),
				ParamServiceResolver,
				ParamServiceCreateParameterProvider,
				PropPathExpr
				) :
			Expression.Convert(
				Expression.Call(
					ResolveManagedServiceMethodInfo,
					ParamServiceResolver,
					ParamServiceCreateParameterProvider,
					PropPathExpr,
					Expression.Constant(Type)
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
						else if (ServiceDetector.IsService(gtd))
						{
							for (var i = 0; i < PathList.Count; i++)
								CopyRequiredPaths.Add(string.Join(".", PathList.Take(i + 1)));
							return;
						}
					}
					if (ServiceDetector.IsService(t))
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
			bool IsServiceType(IServiceDetector ServiceDetector, Type Type, out ResolveType ResolveType)
			{
				Type RealType = null;
				do
				{
					RealType = Type.GetGenericArgumentTypeAsLazy();
					if (RealType != null)
					{
						ResolveType = ResolveType.Lazy;
						break;
					}

					RealType = Type.GetGenericArgumentTypeAsFunc();
					if (RealType != null)
					{
						ResolveType = ResolveType.Func;
						break;
					}

					RealType = Type.GetGenericArgumentTypeAsOption();
					if (RealType != null)
					{
						ResolveType = ResolveType.Option;
						break;
					}


					ResolveType = ResolveType.Instance;
					RealType = Type;

				} while (false);
				return ServiceDetector.IsService(RealType);
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
					var isSvcType = IsServiceType(ServiceDetector,Type,out rType);
					if (isSvcType)
						return ManagedServiceResolveExpression(Type, PropPathExpr, rType);
					//else if (isSvcType == Internals.ServiceType.Normal)
					//	return ServiceProviderResolveExpression(Type, rType);
					else if (Type == typeof(ServiceManagement.IServiceInstanceMeta))
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
				var isSvcType = IsServiceType(ServiceDetector,pi.ParameterType,out resolveType);
				//if (isSvcType == Internals.ServiceType.Normal)
				//	return ServiceProviderResolveExpression(pi.ParameterType, resolveType);
				if (isSvcType)
					return ManagedServiceResolveExpression(pi.ParameterType, Expression.Constant(pi.Name), resolveType);
				else if (pi.ParameterType == typeof(ServiceManagement.IServiceInstanceMeta))
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
			public ServiceCreator Build()
			{ 
				var args = constructorInfo.GetParameters().Select((pi, i) => BuildParamExpression(pi, i)).ToArray();
				return Expression.Lambda<ServiceCreator>(
					Expression.New(constructorInfo, args),
					ParamServiceResolver,
					ParamServiceCreateParameterProvider
					).Compile();
			}
		}
		public static ServiceCreator Build(
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
