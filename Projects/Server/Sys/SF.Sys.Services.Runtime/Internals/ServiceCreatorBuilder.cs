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
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using SF.Sys.Reflection;
using SF.Sys.Linq.Expressions;
using SF.Sys.Annotations;
using SF.Sys.Linq;

namespace SF.Sys.Services.Internals
{
	delegate object ServiceCreator(
		IServiceResolver ServiceResolver,
		IServiceInstanceDescriptor ServiceInstanceDescriptor,
		IServiceCreateParameterTemplate ParameterTemplate
		);
	delegate KeyValuePair<string, object>[] ServiceConfigLoader(
		IServiceResolver ServiceResolver,
		IServiceInstanceDescriptor ServiceInstanceDescriptor,
		IServiceCreateParameterTemplate ParameterTemplate
		);
	class ServiceCreatorBuilder
	{
		static ParameterExpression ParamServiceResolver = Expression.Parameter(typeof(IServiceResolver), "sr");

		static ParameterExpression ParamServiceCreateParameterProvider = Expression.Parameter(typeof(IServiceCreateParameterTemplate), "ap");
		static MethodInfo ServiceCreateParameterProviderGetParameter = typeof(IServiceCreateParameterTemplate).GetMethod("GetArgument", BindingFlags.Public | BindingFlags.Instance);
		static MethodInfo ServiceCreateParameterProviderGetServiceIdent = typeof(IServiceCreateParameterTemplate).GetMethod("GetServiceIdent", BindingFlags.Public | BindingFlags.Instance);

		static ParameterExpression ParamServiceInstanceDescriptor = Expression.Parameter(typeof(IServiceInstanceDescriptor), "dsp");


		static MethodInfo StringConcat = typeof(String).GetMethods().Single(m => m.Name == "Concat" && m.IsStatic && m.IsPublic && m.GetParameters().Length == 2 && m.GetParameters().All(pt => pt.ParameterType == typeof(string)));
		static MethodInfo StringConcat4 = typeof(String).GetMethods().Single(m => m.Name == "Concat" && m.IsStatic && m.IsPublic && m.GetParameters().Length == 4 && m.GetParameters().All(pt => pt.ParameterType == typeof(string)));
		static MethodInfo Int32ToString = typeof(int).GetMethods().Single(m => m.Name == "ToString" && m.IsPublic && !m.IsStatic && m.GetParameters().Length == 0);

		static T NullVerify<T>(object obj, IServiceInstanceDescriptor Descriptor, string Path)
			where T : class
		{
			if (obj == null)
				throw new NullReferenceException($"在构造服务{Descriptor.ServiceDeclaration.ServiceType}的实现{Descriptor.ServiceImplement.ImplementType}时，参数{Path}为Null, 类型:{typeof(T)}");
			return (T)obj;
		}
		static MethodInfo NullVerifyMethod = typeof(ServiceCreatorBuilder).GetMethodExt(
			nameof(NullVerify),
			typeof(object),
			typeof(IServiceInstanceDescriptor),
			typeof(string)
			);

		static Expression NullVerify(Expression e, string Path)
			=> Expression.Call(
				NullVerifyMethod.MakeGenericMethod(e.Type),
				e,
				ParamServiceInstanceDescriptor,
				Expression.Constant(Path)
				);

		

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
			//	throw new NotImplementedException($"找不到服务{InterfaceType}，配置路径:{Path}");
			//var ServiceType= MemberAttribute.ServiceType??InterfaceType;
			if (si != null && si.ServiceType != null && si.ServiceType != InterfaceType)
				throw new NotSupportedException($"{InterfaceType}的配置路径:{Path}返回的服务类型为{si.ServiceType}不是{InterfaceType}");

			object instance = null;
			if (si?.InstanceId.HasValue ?? false)
			{
				if (si.InstanceId.Value > 0)
				{
					instance = Resolver.ResolveServiceByIdent(
						si.InstanceId.Value,
						InterfaceType
						);
					if (instance == null && !MemberAttribute.Optional)
						throw new NotSupportedException($"找不到服务类型为{InterfaceType},ID为{si.InstanceId}的服务, 当前服务:{ServiceInstanceDescriptor.InstanceId}，配置路径:{Path},实现:{ServiceInstanceDescriptor.ServiceImplement.ImplementType}");
				}
				else
				{
					if (!MemberAttribute.Optional)
						throw new NotSupportedException($"当前未配置{InterfaceType}的服务, 当前服务:{ServiceInstanceDescriptor.InstanceId}，配置路径:{Path},实现:{ServiceInstanceDescriptor.ServiceImplement.ImplementType}");
				}
			}
			else
			{
				instance = Resolver.ResolveServiceByType(
					ServiceInstanceDescriptor.IsManaged ? (long?)ServiceInstanceDescriptor.InstanceId : null,
					InterfaceType,
					null
					);
				if (instance == null && !MemberAttribute.Optional)
					throw new NotSupportedException($"在当前服务下找不到服务类型为{InterfaceType}的服务, 当前服务:{ServiceInstanceDescriptor.InstanceId}，配置路径:{Path},实现:{ServiceInstanceDescriptor.ServiceImplement.ImplementType}");
			}

			return instance;
		}





		//static IServiceProvider CreateServiceProvider(
		//	IServiceResolver Resolver,
		//	IServiceInstanceDescriptor ServiceInstanceDescriptor
		//	)
		//{
		//	return
		//		//ServiceInstanceDescriptor.IsManaged?
		//		Resolver.CreateInternalServiceProvider(ServiceInstanceDescriptor);//: 
		//		//Resolver.Provider;
		//}
		//static MethodInfo CreateServiceProviderMethodInfo = typeof(ServiceCreatorBuilder).GetMethodExt(
		//	nameof(CreateServiceProvider),
		//	typeof(IServiceResolver),
		//	typeof(IServiceInstanceDescriptor)
		//	);

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
			){
				var scopeId = ServiceResolver.CurrentServiceId;
				return new Lazy<T>(() =>
				{
					using (ServiceResolver.WithScopeService(scopeId))
					{
						var re = ResolveManagedService(
							ServiceResolver,
							CreateParameterTemplate,
							ServiceInstanceDescriptor,
							Path,
							typeof(T),
							MemberAttribute
							);
						if (re == null)
							throw new InvalidOperationException($"找不到服务{typeof(T)},上下文:{ServiceInstanceDescriptor.InstanceId}");
						return (T)re;
					}
				});
			}

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
			) {
			var scopeId = resolver.CurrentServiceId;

			return () =>
			{
				using (resolver.WithScopeService(scopeId))
				{
					var re = ResolveManagedService(
						resolver,
						CreateParameterTemplate,
						ServiceInstanceDescriptor,
						Path,
						typeof(T),
						MemberAttribute
						);
					if (re == null)
						throw new InvalidOperationException($"找不到服务{typeof(T)},上下文:{ServiceInstanceDescriptor.InstanceId}");
					return (T)re;
				}
			};
			}
		
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
			ServiceResolveType resolveType,
			IServiceMetadata Metadata,
			MemberAttribute MemberAttribute
			)
		{
			switch (resolveType)
			{
				case ServiceResolveType.Lazy:
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
				case ServiceResolveType.Func:
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

		static Func<T> FuncWrapper<T>(object instance) =>
			new Func<T>(() => (T)instance);
		static Lazy<T> LazyWrapper<T>(object instance) =>
			new Lazy<T>(() => (T)instance);

		static MethodInfo FuncWrapperMethod= typeof(ServiceCreatorBuilder).GetMethodExt(
			nameof(FuncWrapper),
			typeof(object)
			);
		static MethodInfo LazyWrapperMethod = typeof(ServiceCreatorBuilder).GetMethodExt(
			nameof(LazyWrapper),
			typeof(object)
			);

		static Expression WrapInstance(ServiceResolveType type, Expression expr) =>
			type == ServiceResolveType.Func ?
			Expression.Call(
				FuncWrapperMethod.MakeGenericMethod(expr.Type),
				expr
				) :
			type == ServiceResolveType.Lazy ?
			Expression.Call(
				LazyWrapperMethod.MakeGenericMethod(expr.Type),
				expr
				) :
			throw new NotSupportedException();

		static Expression GetServiceResolverExpression(ServiceResolveType type) =>
			type == ServiceResolveType.Direct ? ParamServiceResolver:
			WrapInstance(type, GetServiceResolverExpression(ServiceResolveType.Direct));

		static Expression GetServiceProviderExpression(ServiceResolveType type) =>
			type == ServiceResolveType.Direct ?
			//Expression.Call(
			//	null,
			//	CreateServiceProviderMethodInfo,
			//	ParamServiceResolver,
			//	ParamServiceInstanceDescriptor
			//	):
			Expression.Property(ParamServiceResolver,"Provider"):
			WrapInstance(type, GetServiceProviderExpression(ServiceResolveType.Direct));

		static Expression GetServiceInstanceDescriptorExpression(ServiceResolveType type) =>
			type == ServiceResolveType.Direct ? ParamServiceInstanceDescriptor :
			WrapInstance(type, GetServiceInstanceDescriptorExpression(ServiceResolveType.Direct));

		static Expression StrConcat(Expression e1, Expression e2) =>
			Expression.Call(null, StringConcat, e1, e2);



		public static ConstructorInfo FindBestConstructorInfo(Type Type,IServiceMetadata Meta)
		{
			var constructors = Type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
				.ToArray();


			if (constructors.Length == 0)
				throw new InvalidOperationException($"找不到构造函数:{Type}");

			if (constructors.Length == 1)
				return constructors[0];

			Array.Sort(constructors,
				(a, b) => b.GetParameters().Length.CompareTo(a.GetParameters().Length));

			ConstructorInfo bestConstructor =null;
			HashSet<Type> bestConstructorParameterTypes = null;
			for (var i = 0; i < constructors.Length; i++)
			{
				var c = constructors[i];
				if (c.GetParameters().Any(p => Meta.DetectService(p.ParameterType) == ServiceResolveType.None))
					continue;
				if (bestConstructor == null)
				{
					bestConstructor = c;
				}
				else if (bestConstructorParameterTypes == null)
				{
					bestConstructorParameterTypes = new HashSet<Type>(bestConstructor.GetParameters().Select(p => p.ParameterType));
				}
				else
				{
					var parameters = constructors[i].GetParameters();
					if (!bestConstructorParameterTypes.IsSupersetOf(parameters.Select(p => p.ParameterType)))
						throw new InvalidOperationException($"服务{Type}构造函数冲突:{bestConstructor.GetParameters().Select(p => p.Name).Join(",")}和{constructors[i].GetParameters().Select(p => p.Name).Join(",")}");
				}
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
					throw new NotSupportedException($"配置类型循环依赖:根类型{ImplementType} 当期类型:{ot}");
				try
				{
					var t = ot;
					if (t.IsArray)
						t = t.GetElementType();
					else if (t.IsGeneric() && t.GetGenericTypeDefinition() == typeof(Dictionary<,>))
						t = t.GetGenericArguments()[1];

					if (ServiceMetadata.DetectService(t)!=ServiceResolveType.None)
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
			
			static Dictionary<K,T> CopyDirectionary<K,T>(Dictionary<K, T> dict)
			{
				return dict.ToDictionary(p => p.Key, p => p.Value);
			}
			Expression CopyExpression(Type Type, MemberAttribute MemberAttribute, Expression src, Expression PropPathExpr, string PropPath)
				=> CopyExpression(Type, MemberAttribute, src, PropPathExpr, PropPath, out var isSvcType);

			Expression CopyExpression(Type Type, MemberAttribute MemberAttribute, Expression src, Expression PropPathExpr, string PropPath, out bool isSvcType)
			{
				isSvcType = false;
				if (CopyRequiredPaths==null || !CopyRequiredPaths.Contains(PropPath))
					return src;

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
				//else if (Type.IsGeneric() && Type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				//{
				//	var elementType = Type.GetGenericArguments()[0];
				//	if (ServiceMetadata.IsService(elementType))
				//		return ManagedServiceResolveExpression(
				//			Type,
				//			PropPathExpr,
				//			ResolveType.Instance,
				//			ServiceMetadata,
				//			MemberAttribute
				//			);

				//	return Expression.Convert(
				//		EnumerableCopyExpression(MemberAttribute, src, elementType, PropPathExpr, PropPath),
				//		Type
				//		);
				//}
				else
				{
					Type RealType;
					var rType= ServiceMetadata.DetectService(Type,out RealType);
					isSvcType = rType != ServiceResolveType.None;
					if (RealType == typeof(IServiceInstanceDescriptor))
						return GetServiceInstanceDescriptorExpression(rType);
					if (RealType == typeof(IServiceResolver))
						return GetServiceResolverExpression(rType);
					if (RealType == typeof(IServiceProvider))
						return GetServiceProviderExpression(rType);

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
				var prop = Expression.Property(src, pi);
				var copied= CopyExpression(
					pi.PropertyType, 
					TryGetAttributes(pi), 
					prop, 
					StrConcat(PropPathExpr, Expression.Constant("." + pi.Name)), 
					PropPath + "." + pi.Name,
					out var isSvcType
					);
				if (!pi.PropertyType.IsClassType() || isSvcType)
					return copied;

				return Expression.Condition(Expression.Equal(prop, Expression.Constant(null, pi.PropertyType)), Expression.Constant(null, pi.PropertyType), copied);

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
				var srcVar = Expression.Variable(Src.Type);
				return Expression.Block(
					new[] { srcVar },
					Expression.Assign(srcVar, NullVerify(Src, PropPath)),
					Expression.MemberInit(Expression.New(Type), MemberInitExpressions(Type, srcVar, PropPathExpr, PropPath))
					);
			}

			Expression BuildParamExpression(ParameterInfo pi, int Index)
			{
				Type RealType;
				var resolveType = ServiceMetadata.DetectService(pi.ParameterType,out RealType);
				//if (isSvcType == Internals.ServiceType.Normal)
				//	return ServiceResolverResolveExpression(pi.ParameterType, resolveType);
				if (RealType == typeof(IServiceInstanceDescriptor))
					return GetServiceInstanceDescriptorExpression(resolveType);
				else if (RealType == typeof(IServiceResolver))
					return GetServiceResolverExpression(resolveType);
				else if (RealType == typeof(IServiceProvider))
					return GetServiceProviderExpression(resolveType);
				else if (resolveType!=ServiceResolveType.None)
					return ManagedServiceResolveExpression(
						pi.ParameterType, 
						Expression.Constant(pi.Name), 
						resolveType,
						ServiceMetadata,
						TryGetAttributes(pi)
						);
				else
				{
					if (CopyRequiredPaths==null || !CopyRequiredPaths.Contains(pi.Name))
						return Expression.Convert(
							Expression.Call(
								ParamServiceCreateParameterProvider,
								ServiceCreateParameterProviderGetParameter,
								Expression.Constant(Index)
								),
							pi.ParameterType
							);

					var src = Expression.Variable(pi.ParameterType);
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
			public (ServiceCreator, ServiceConfigLoader) Build()
			{ 
				var args = constructorInfo
					.GetParameters()
					.Select((pi, i) => BuildParamExpression(pi, i))
					.ToArray();
				var pairConstructor = typeof(KeyValuePair<string, object>).GetConstructor(new[] { typeof(string), typeof(object) });
				return (
					Expression.Lambda<ServiceCreator>(
						Expression.New(constructorInfo, args),
						ParamServiceResolver,
						ParamServiceInstanceDescriptor,
						ParamServiceCreateParameterProvider
						).Compile(),
					Expression.Lambda<ServiceConfigLoader>(
						Expression.NewArrayInit(
							typeof(KeyValuePair<string, object>),
							constructorInfo.GetParameters().Zip(args, (p, a) =>
								 Expression.New(
									 pairConstructor,
									 Expression.Constant(p.Name),
									 Expression.Convert(a, typeof(object))
									 )
								)
							),
						ParamServiceResolver,
						ParamServiceInstanceDescriptor,
						ParamServiceCreateParameterProvider
						).Compile()
					);
			}
		}
		public static (ServiceCreator creator, ServiceConfigLoader loader) Build(
			IServiceMetadata ServiceMetadata,
			Type ServiceType,
			Type ImplementType,
			ConstructorInfo constructorInfo,
			bool IsManagedService
			)
		{
			HashSet<string> CopyRequiredPaths = null;
			if (IsManagedService)
			{
				var helper = new InitCopyRequiredPathsHelper
				{
					constructorInfo = constructorInfo,
					CopyRequiredPaths = new HashSet<string>(),
					ImplementType = ImplementType,
					ServiceMetadata = ServiceMetadata
				};
				helper.InitCopyRequiredPaths();
				if (helper.CopyRequiredPaths.Count > 0)
					CopyRequiredPaths = helper.CopyRequiredPaths;
			}
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
