using System;
using System.Collections.Generic;

using System.Linq;
using System.Collections;
using System.Reflection;
using SF.Core.ServiceManagement.Internals;
using SF.Core.ServiceManagement;

namespace SF.Core.ServiceManagement
{
	public delegate T TypedInstanceResolver<T>(long Id);

	public class ServiceProviderBuilder : IServiceProviderBuilder
	{
		public virtual IServiceMetadata OnCreateServcieMetadata(IServiceCollection Services)
		{
			return new Internals.ServiceMetadata(Services);
		}
		
		public virtual Internals.IServiceFactoryManager OnCreateServcieFactoryManager(
			IServiceMetadata meta, 
			Caching.ILocalCache<IServiceEntry> AppServiceCache
			)
		{
			return new Internals.ServiceFactoryManager(
				AppServiceCache, 
				meta
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
		public virtual void PrepareServices(IServiceCollection Services)
		{
			
			
		}

		static object NewCreator<I>(IServiceProvider sp) where I : class
			=> new Func<I>(() => sp.Resolve<I>());
		static object NewLazy<I>(IServiceProvider sp) where I : class
			=> new Lazy<I>(() => sp.Resolve<I>());
		static object NewTypedInstanceResolver<I>(IServiceProvider sp) where I : class
			=> new TypedInstanceResolver<I>((id) => sp.Resolve<I>(id));

		static IEnumerable<I> NewEnumerableReal<I>(IServiceProvider sp) where I : class
		{
			var resolver = sp.NewResolver();
			var desc = (IServiceInstanceDescriptor)resolver.ResolveServiceByType(null,typeof(IServiceInstanceDescriptor),null);
			foreach (var i in resolver.ResolveServices(desc?.ParentInstanceId, typeof(I), null))
				yield return (I)i;
		}
		static object NewEnumerable<I>(IServiceProvider sp) where I : class
		{
			return NewEnumerableReal<I>(sp);
		}
		public IServiceProvider Build(
			IServiceCollection Services,
			Caching.ILocalCache<IServiceEntry> AppServiceCache=null
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
			Services.AddTransient(sp => resolver);
			Services.AddSingleton(sp => meta);
			Services.AddSingleton(sp => manager);
			Services.AddSingleton(sp => scopeFactory );
			Services.AddSingleton(sp => (IServiceInstanceConfigChangedNotifier)manager);
			//Services.AddTransient(typeof(IEnumerable<>), typeof(ManagedServices.Runtime.EnumerableService<>));
			Services.AddTransient(typeof(IEnumerable<>), typeof(ServiceProviderBuilder).GetMethodExt(nameof(NewEnumerable), typeof(IServiceProvider)));
			Services.AddTransient(typeof(Func<>), typeof(ServiceProviderBuilder).GetMethodExt(nameof(NewCreator), typeof(IServiceProvider)));
			Services.AddTransient(typeof(Lazy<>), typeof(ServiceProviderBuilder).GetMethodExt(nameof(NewLazy), typeof(IServiceProvider)));
			Services.AddTransient(typeof(TypedInstanceResolver<>), typeof(ServiceProviderBuilder).GetMethodExt(nameof(NewTypedInstanceResolver), typeof(IServiceProvider)));

			meta = OnCreateServcieMetadata(Services);
			OnValidate(meta);
			manager = OnCreateServcieFactoryManager(
				meta, 
				AppServiceCache
				);
			provider = OnCreateServiceProvider(manager);
			scopeFactory = new ServiceScopeFactory(provider, manager);
			return provider;
		}
	}

}
