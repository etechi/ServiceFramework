using System;
using System.Collections.Generic;

using System.Linq;
using System.Collections;
using System.Reflection;
using SF.Core.ServiceManagement.Internals;
using SF.Core.ServiceManagement;

namespace SF.Core.ServiceManagement
{
	public class ServiceProviderBuilder : IServiceProviderBuilder
	{
		public virtual IServiceMetadata OnCreateServcieMetadata(IServiceCollection Services)
		{
			return new Internals.ServiceMetadata(Services);
		}
		public virtual IServiceDeclarationTypeResolver OnCreateServiceDeclarationTypeResolver(IServiceMetadata meta)
		{
			return new DefaultServiceDeclarationTypeResolver(meta);
		}
		public virtual IServiceImplementTypeResolver OnCreateImplementDeclarationTypeResolver(IServiceMetadata meta)
		{
			return new DefaultServiceImplementTypeResolver(meta);
		}
		public virtual Internals.IServiceFactoryManager OnCreateServcieFactoryManager(
			IServiceMetadata meta, 
			Caching.ILocalCache<IServiceEntry> AppServiceCache,
			IServiceDeclarationTypeResolver ServiceDeclarationTypeResolver,
			IServiceImplementTypeResolver ServiceImplementTypeResolver
			)
		{
			return new Internals.ServiceFactoryManager(
				AppServiceCache, 
				meta,
				ServiceDeclarationTypeResolver?? OnCreateServiceDeclarationTypeResolver(meta),
				ServiceImplementTypeResolver?? OnCreateImplementDeclarationTypeResolver(meta)
				);
		}
		public virtual IServiceProvider OnCreateServiceProvider(Internals.IServiceFactoryManager Manager)
		{
			return new ServiceProvider(Manager);
		}
		public virtual Type GetRealServiceType(Type type)
		{
			var (argType,retType) = type.GetGenericArgumentTypeAsFunc2();
			if (argType == typeof(long))
				return retType;
			return type.GetGenericArgumentTypeAsFunc() ??
					type.GetGenericArgumentTypeAsLazy() ??
					type;
		}
		void validate(
			IServiceImplement Implement,
			IServiceImplement CallImplement,
			IServiceMetadata Meta, 
			HashSet<Type> ValidatedImplements,
			HashSet<Type> UsedServiceImplements
			)
		{
			if (!UsedServiceImplements.Add(Implement.ImplementType))
				throw new InvalidOperationException(
					$"服务{Implement.ImplementType}存在循环引用，来自{CallImplement?.ImplementType}"
					);

			if (!ValidatedImplements.Contains(Implement.ImplementType) && Implement.ServiceImplementType==ServiceImplementType.Type)
			{
				(from arg in Internals.ServiceCreatorBuilder.FindBestConstructorInfo(Implement.ImplementType).GetParameters()
				 let type = GetRealServiceType(arg.ParameterType)
				 where type.IsInterface && !type.IsDefined(typeof(AutoBindAttribute))
				 let svcs = Meta.Services.Get(type) ?? (type.IsGenericType?Meta.Services.Get(type.GetGenericTypeDefinition()):null)
				select new { arg, svcs, type }
				).ForEach(tuple => {
					
					if (tuple.svcs == null )
						throw new InvalidOperationException(
							$"找不到{Implement.ImplementType}的构造函数参数 {tuple.arg.Name}引用的服务{tuple.type}"
							);
					foreach(var svc in tuple.svcs.Implements)
							validate(svc, Implement, Meta, ValidatedImplements, UsedServiceImplements);
				}
				);

				ValidatedImplements.Add(Implement.ImplementType);
			}
			UsedServiceImplements.Remove(Implement.ImplementType);
		}
		public virtual void OnValidate(IServiceMetadata Meta)
		{
			var validated = new HashSet<Type>();
			var used = new HashSet<Type>();
			foreach(var sif in (from svc in Meta.Services.Values
			 from impl in svc.Implements
				select impl
			 ))
			validate(sif,null, Meta, validated, used);
		}
		static object NewLazy<I>(IServiceProvider sp)  where I : class
		{
			return new Lazy<I>(() => sp.Resolve<I>());
		}
		static object NewCreator<I>(IServiceProvider sp)  where I : class
		{
			return new Func<I>(() => sp.Resolve<I>());
		}

		static object NewInstanceCreator<I>(IServiceProvider sp) where I : class
		{
			return new Func<long, I>(id => sp.Resolve<I>(id));
		}
		static object NewInstances<T>(IServiceProvider sp,int count) where T : class
		{
			return Enumerable.Range(0, count).Select(i =>
				(T)sp.Resolve<T>(-1-i)
				);
		}


		static MethodInfo NewLazyMethodInfo = typeof(ServiceProviderBuilder)
				.GetMethodExt(nameof(NewLazy), typeof(IServiceProvider));
		static MethodInfo NewCreatorMethodInfo = typeof(ServiceProviderBuilder)
			.GetMethodExt(nameof(NewCreator), typeof(IServiceProvider));


		static MethodInfo NewInstanceCreatorMethodInfo = typeof(ServiceProviderBuilder)
			.GetMethodExt(nameof(NewInstanceCreator), typeof(IServiceProvider));

		static MethodInfo NewInstancesMethodInfo = typeof(ServiceProviderBuilder)
			.GetMethodExt(nameof(NewInstances), typeof(IServiceProvider),typeof(int));

		public virtual void PrepareServices(IServiceCollection Services)
		{
			Services
				.Where(svc => svc.ServiceImplementType == ServiceImplementType.Type)
				.Distinct()
				.ToArray()
				.ForEach(svc =>
				{
					var newLazy = NewLazyMethodInfo
						.MakeGenericMethod(svc.InterfaceType)
						.CreateDelegate<Func<IServiceProvider, object>>();
					Services.Add(new ServiceDescriptor(
						typeof(Lazy<>).MakeGenericType(svc.InterfaceType),
						newLazy,
						svc.Lifetime
						));


					var newCreator = NewCreatorMethodInfo
						.MakeGenericMethod(svc.InterfaceType)
						.CreateDelegate<Func<IServiceProvider, object>>();
					Services.Add(new ServiceDescriptor(
						typeof(Func<>).MakeGenericType( svc.InterfaceType),
						newCreator,
						svc.Lifetime
						));


					var newInstanceCreatorSimple = NewInstanceCreatorMethodInfo
						.MakeGenericMethod(svc.InterfaceType)
						.CreateDelegate<Func<IServiceProvider, object>>();
					Services.Add(new ServiceDescriptor(
						typeof(Func<,>).MakeGenericType(typeof(long), svc.InterfaceType),
						newInstanceCreatorSimple,
						svc.Lifetime
						));

				});

			
		}
		public IServiceProvider Build(
			IServiceCollection Services,
			Caching.ILocalCache<IServiceEntry> AppServiceCache=null,
			IServiceDeclarationTypeResolver ServiceDeclarationTypeResolver=null,
			IServiceImplementTypeResolver ServiceImplementTypeResolver=null
			)
		{

			PrepareServices(Services);

			IServiceProvider provider = null;
			IServiceResolver resolver = null;
			IServiceMetadata meta = null;
			Internals.IServiceFactoryManager manager = null;
			IServiceScopeFactory scopeFactory = null;

			//Services.AddSingleton(sp => provider.Resolve<IServiceResolver>());
			Services.AddSingleton(sp => provider);
			Services.AddSingleton(sp => resolver);
			Services.AddSingleton(sp => meta);
			Services.AddSingleton(sp => manager);
			Services.AddSingleton(sp => scopeFactory );
			Services.AddSingleton(sp => (IServiceInstanceConfigChangedNotifier)manager);
			Services.AddTransient(typeof(IEnumerable<>), typeof(ManagedServices.Runtime.EnumerableService<>));
			meta = OnCreateServcieMetadata(Services);
			OnValidate(meta);
			manager = OnCreateServcieFactoryManager(
				meta, 
				AppServiceCache,
				ServiceDeclarationTypeResolver,
				ServiceImplementTypeResolver
				);
			provider = OnCreateServiceProvider(manager);
			scopeFactory = new ServiceScopeFactory(provider, manager);
			resolver = (IServiceResolver)provider;
			return provider;
		}
	}

}
