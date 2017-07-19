using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;

namespace SF.Core.ServiceManagement.Internals
{
	delegate object ServiceCreator(
		IServiceResolver ServiceResolver,
		IServiceInstanceDescriptor ServiceInstanceDescriptor,
		IServiceCreateParameterTemplate ParameterTemplate
		);

	class ServiceCreatorBuilder
	{
		static ParameterExpression ParamServiceResolver = Expression.Parameter(typeof(IServiceResolver), "sr");

		static ParameterExpression ParamServiceCreateParameterProvider = Expression.Parameter(typeof(IServiceCreateParameterTemplate), "ap");
		static MethodInfo ServiceCreateParameterProviderGetParameter = typeof(IServiceCreateParameterTemplate).GetMethod("GetArgument", BindingFlags.Public | BindingFlags.Instance);
		static MethodInfo ServiceCreateParameterProviderGetServiceIdent = typeof(IServiceCreateParameterTemplate).GetMethod("GetServiceIdent", BindingFlags.Public | BindingFlags.Instance );

		static ParameterExpression ParamServiceInstanceDescriptor = Expression.Parameter(typeof(IServiceInstanceDescriptor), "dsp");


		static MethodInfo StringConcat = typeof(String).GetMethods().Single(m => m.Name == "Concat" && m.IsStatic && m.IsPublic && m.GetParameters().Length == 2 && m.GetParameters().All(pt=>pt.ParameterType== typeof(string)));
		static MethodInfo StringConcat4 = typeof(String).GetMethods().Single(m => m.Name == "Concat" && m.IsStatic && m.IsPublic && m.GetParameters().Length == 4 && m.GetParameters().All(pt => pt.ParameterType == typeof(string)));
		static MethodInfo Int32ToString = typeof(int).GetMethods().Single(m => m.Name == "ToString" && m.IsPublic && !m.IsStatic && m.GetParameters().Length == 0);
		enum ResolveType
		{
			Instance,
			Lazy,
			Func
		}
		
		public class MemberAttribute
		{
			public bool Optional { get; set; }
		}
		static object ResolveManagedService(
			IServiceResolver Resolver,
			IServiceCreateParameterTemplate CreateParameterTemplate,
			IServiceInstanceDescriptor ServiceInstanceDescriptor,
			string Path,
			Type InterfaceType,
			MemberAttribute MemberAttribute
			)
		{
			var si = CreateParameterTemplate.GetServiceIdent(Path);
			//if(pair.Key==null)

			//if (pair.Key == null)
			//	throw new NotImplementedException($"�Ҳ�������{InterfaceType}������·��:{Path}");
			//var ServiceType= MemberAttribute.ServiceType??InterfaceType;
			if (si!=null && si.ServiceType != null && si.ServiceType != InterfaceType)
				throw new NotSupportedException($"{InterfaceType}������·��:{Path}���صķ�������Ϊ{si.ServiceType}����{InterfaceType}");

			object instance = null;
			if (si?.InstanceId.HasValue ?? false)
			{
				if (si.InstanceId.Value > 0)
					instance = Resolver.ResolveServiceByIdent(
						si.InstanceId.Value,
						InterfaceType
						);
			}
			else
			{
				instance = Resolver.ResolveServiceByType(
					(ServiceInstanceDescriptor.InstanceId??-1)<0?null: ServiceInstanceDescriptor.InstanceId,
					InterfaceType,
					null
					);
			}

			if (instance == null && !MemberAttribute.Optional)
				throw new NotSupportedException($"�Ҳ�����������Ϊ{InterfaceType},IDΪ{si.InstanceId}�ķ���, ��ǰ����:{ServiceInstanceDescriptor.InstanceId}");
			return instance;
		}

		

		static IServiceProvider CreateServiceProvider(
			IServiceResolver Resolver,
			IServiceInstanceDescriptor ServiceInstanceDescriptor
			)
		{
			return
				ServiceInstanceDescriptor.InstanceId.HasValue ?
				Resolver.CreateInternalServiceProvider(
				ServiceInstanceDescriptor.InstanceId.Value
				) : Resolver.Provider;
		}
		static MethodInfo CreateServiceProviderMethodInfo = typeof(ServiceCreatorBuilder).GetMethodExt(
			nameof(CreateServiceProvider),
			typeof(IServiceResolver),
			typeof(IServiceInstanceDescriptor)
			);

		static MethodInfo ResolveManagedServiceMethodInfo = typeof(ServiceCreatorBuilder).GetMethodExt(
			nameof(ResolveManagedService),
			typeof(IServiceResolver),
			typeof(IServiceCreateParameterTemplate),
			typeof(IServiceInstanceDescriptor),
			typeof(string),
			typeof(Type),
			typeof(MemberAttribute)
			);
		static Lazy<T> CreateLazyedManagedServiceResolve<T>(
			IServiceResolver ServiceResolver,
			IServiceCreateParameterTemplate CreateParameterTemplate,
			IServiceInstanceDescriptor ServiceInstanceDescriptor,
			string Path,
			MemberAttribute MemberAttribute
			) =>new Lazy<T>(() => (T)ResolveManagedService(
				ServiceResolver, 
				CreateParameterTemplate, 
				ServiceInstanceDescriptor,
				Path, 
				typeof(T), 
				MemberAttribute
				));

		static MethodInfo CreateLazyedManagedServiceResolveMethodInfo = typeof(ServiceCreatorBuilder).GetMethodExt(
			nameof(CreateLazyedManagedServiceResolve),
			typeof(IServiceResolver),
			typeof(IServiceCreateParameterTemplate),
			typeof(IServiceInstanceDescriptor),
			typeof(string),
			typeof(MemberAttribute)
			);

		static Func<T> CreateFuncManagedServiceResolve<T>(
			IServiceResolver resolver,
			IServiceCreateParameterTemplate CreateParameterTemplate,
			IServiceInstanceDescriptor ServiceInstanceDescriptor,
			string Path,
			MemberAttribute MemberAttribute
			) => () =>
				(T)ResolveManagedService(
					resolver,
					CreateParameterTemplate, 
					ServiceInstanceDescriptor,
					Path, 
					typeof(T), 
					MemberAttribute
					)
			;
		
		static MethodInfo CreateFuncManagedServiceResolveMethodInfo = typeof(ServiceCreatorBuilder).GetMethodExt(
			nameof(CreateFuncManagedServiceResolve),
			typeof(IServiceResolver),
			typeof(IServiceCreateParameterTemplate),
			typeof(IServiceInstanceDescriptor),
			typeof(string),
			typeof(MemberAttribute)
			);
		static Expression ManagedServiceResolveExpression(
			Type Type,
			Expression PropPathExpr,
			ResolveType resolveType,
			IServiceMetadata Metadata,
			MemberAttribute MemberAttribute
			)
		{
			switch (resolveType)
			{
				case ResolveType.Lazy:
					{
						var type = Type.GetGenericArgumentTypeAsLazy();
						return Expression.Call(
							CreateLazyedManagedServiceResolveMethodInfo.MakeGenericMethod(type),
							ParamServiceResolver,
							ParamServiceCreateParameterProvider,
							ParamServiceInstanceDescriptor,
							PropPathExpr,
							Expression.Constant(MemberAttribute)
							);
					}
				case ResolveType.Func:
					{
						var type = Type.GetGenericArgumentTypeAsFunc();
						return Expression.Call(
							CreateFuncManagedServiceResolveMethodInfo.MakeGenericMethod(type),
							ParamServiceResolver,
							ParamServiceCreateParameterProvider,
							ParamServiceInstanceDescriptor,
							PropPathExpr,
							Expression.Constant(MemberAttribute)
							);
					}
				default:
					return Expression.Convert(
						Expression.Call(
							ResolveManagedServiceMethodInfo,
							ParamServiceResolver,
							ParamServiceCreateParameterProvider,
							ParamServiceInstanceDescriptor,
							PropPathExpr,
							Expression.Constant(Type),
							Expression.Constant(MemberAttribute)
						),
						Type
					);
			}
		}
		static Expression GetServiceResolverExpression() =>
			ParamServiceResolver;
		static Expression GetServiceProviderExpression() =>
			Expression.Call(
				null,
				CreateServiceProviderMethodInfo,
				ParamServiceResolver,
				ParamServiceInstanceDescriptor
				);
		static Expression GetServiceInstanceDescriptorExpression() =>
			ParamServiceInstanceDescriptor;

		static Expression StrConcat(Expression e1, Expression e2) =>
			Expression.Call(null, StringConcat, e1, e2);

		public static ConstructorInfo FindBestConstructorInfo(Type Type)
		{
			var constructors = Type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
				.ToArray();


			if (constructors.Length == 0)
				throw new InvalidOperationException($"�Ҳ������캯��:{Type}");

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
					throw new InvalidOperationException($"�����캯����ͻ:{bestConstructor.GetParameters()}��{constructors[i].GetParameters()}");
			}
			return bestConstructor;
		}


		class InitCopyRequiredPathsHelper
		{
			public HashSet<string> CopyRequiredPaths;
			public IServiceMetadata ServiceMetadata;
			public Type ImplementType;
			public ConstructorInfo constructorInfo;

			HashSet<Type> TypePath = new HashSet<Type>();
			List<string> PathList = new List<string>();
			void DetectCopyRequiredPath(Type ot)
			{
				if (ot.IsDefined(typeof(AutoBindAttribute)))
					return;
				if (!TypePath.Add(ot))
					throw new NotSupportedException($"��������ѭ������:������{ImplementType} ��������:{ot}");
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
						else if (ServiceMetadata.IsService(gtd))
						{
							for (var i = 0; i < PathList.Count; i++)
								CopyRequiredPaths.Add(string.Join(".", PathList.Take(i + 1)));
							return;
						}
					}
					if (ServiceMetadata.IsService(t))
					{
						for (var i = 0; i < PathList.Count; i++)
							CopyRequiredPaths.Add(string.Join(".", PathList.Take(i + 1)));
						return;
					}
					if (t.IsPrimitiveType() || t.GetTypeCode()!=TypeCode.Object || t.IsInterfaceType())
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
			public IServiceMetadata ServiceMetadata;
			public Type ServiceType;
			public Type ImplementType;
			public ConstructorInfo constructorInfo;

			Expression EnumerableCopyExpression(MemberAttribute MemberAttribute, Expression src, Type ElementType, Expression PropPathExpr, string PropPath)
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
								CopyExpression(ElementType, MemberAttribute, item, path, PropPath)
								),
							Expression.AddAssign(idx, Expression.Constant(1))
							)),
					list
				);
			}
			Expression DictionaryCopyExpression(MemberAttribute MemberAttribute, Expression src,Type KeyType, Type ElementType, Expression PropPathExpr, string PropPath)
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
								CopyExpression(ElementType, MemberAttribute, item.GetMember("Value"), path, PropPath)
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


					ResolveType = ResolveType.Instance;
					RealType = Type;

				} while (false);
				var re= ServiceDetector.IsService(RealType);
				if (re || !RealType.IsGenericType) return re;
				return ServiceDetector.IsService(RealType.GetGenericTypeDefinition());
			}
			static Dictionary<K,T> CopyDirectionary<K,T>(Dictionary<K, T> dict)
			{
				return dict.ToDictionary(p => p.Key, p => p.Value);
			}
			Expression CopyExpression(Type Type, MemberAttribute MemberAttribute, Expression src, Expression PropPathExpr, string PropPath)
			{
				if (!CopyRequiredPaths.Contains(PropPath))
					return src;

				if (Type == typeof(IServiceInstanceDescriptor))
					return GetServiceInstanceDescriptorExpression();

				if (Type == typeof(IServiceResolver))
					return GetServiceResolverExpression();
				if (Type == typeof(IServiceProvider))
					return GetServiceProviderExpression();


				if (Type.IsArray)
				{
					return Expression.Call(
						EnumerableCopyExpression(
							MemberAttribute,
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
							MemberAttribute,
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
						EnumerableCopyExpression(MemberAttribute, src, Type.GetGenericArguments()[0], PropPathExpr, PropPath),
						Type
						);
				}
				else
				{
					ResolveType rType;
					var isSvcType = IsServiceType(ServiceMetadata,Type,out rType);
					if (isSvcType)
						return ManagedServiceResolveExpression(
							Type, 
							PropPathExpr,
							rType, 
							ServiceMetadata,
							MemberAttribute
							);
					//else if (isSvcType == Internals.ServiceType.Normal)
					//	return ServiceResolverResolveExpression(Type, rType);
					
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
				return CopyExpression(pi.PropertyType, TryGetAttributes(pi), Expression.Property(src, pi), StrConcat(PropPathExpr, Expression.Constant("." + pi.Name)), PropPath + "." + pi.Name);
			}
			
			MemberAttribute TryGetAttributes(MemberInfo pi)
			{
				return new MemberAttribute
				{
					//ServiceType = pi.GetCustomAttribute<FromServiceAttribute>(true)?.ServiceType ??
					//			 (pi.IsDefined(typeof(LocalAttribute),true)?ServiceType:null),
					Optional=pi.IsDefined(typeof(OptionalAttribute),true),
				};
			}
			MemberAttribute TryGetAttributes(ParameterInfo pi)
			{
				return new MemberAttribute
				{
					//ServiceType = pi.GetCustomAttribute<FromServiceAttribute>(true)?.ServiceType ??
					//			 (pi.IsDefined(typeof(LocalAttribute), true) ? ServiceType : null),
					Optional = pi.IsDefined(typeof(OptionalAttribute), true),
				};
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
				var isSvcType = IsServiceType(ServiceMetadata,pi.ParameterType,out resolveType);
				//if (isSvcType == Internals.ServiceType.Normal)
				//	return ServiceResolverResolveExpression(pi.ParameterType, resolveType);
				if (pi.ParameterType == typeof(IServiceInstanceDescriptor))
					return GetServiceInstanceDescriptorExpression();
				else if (pi.ParameterType == typeof(IServiceResolver))
					return GetServiceResolverExpression();
				else if (pi.ParameterType == typeof(IServiceProvider))
					return GetServiceProviderExpression();
				else if (isSvcType)
					return ManagedServiceResolveExpression(
						pi.ParameterType, 
						Expression.Constant(pi.Name), 
						resolveType,
						ServiceMetadata,
						TryGetAttributes(pi)
						);
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
				var args = constructorInfo
					.GetParameters()
					.Select((pi, i) => BuildParamExpression(pi, i))
					.ToArray();
				return Expression.Lambda<ServiceCreator>(
					Expression.New(constructorInfo, args),
					ParamServiceResolver,
					ParamServiceInstanceDescriptor,
					ParamServiceCreateParameterProvider
					).Compile();
			}
		}
		public static ServiceCreator Build(
			IServiceMetadata ServiceMetadata,
			Type ServiceType,
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
				ServiceMetadata = ServiceMetadata
			}.InitCopyRequiredPaths();

			return new Builder
			{
				constructorInfo = constructorInfo,
				CopyRequiredPaths = CopyRequiredPaths,
				ImplementType = ImplementType,
				ServiceMetadata = ServiceMetadata,
				ServiceType= ServiceType,
			}.Build();
		}
	}
}
