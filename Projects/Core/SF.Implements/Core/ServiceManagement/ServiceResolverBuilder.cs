using System;
using System.Collections.Generic;

using System.Linq;
using System.Collections;
using System.Reflection;
using SF.Core.ServiceManagement.Internals;
using SF.Core.ServiceManagement;

namespace SF.Core.ServiceManagement
{
	public class ServiceResolverBuilder : IServiceResolverBuilder
	{
		public virtual IServiceMetadata OnCreateServcieMetadata(IServiceCollection Services)
		{
			return new Internals.ServiceMetadata(Services);
		}
		public virtual Internals.IServiceFactoryManager OnCreateServcieFactoryManager(IServiceMetadata meta, Caching.ILocalCache<object> AppServiceCache)
		{
			return new Internals.ServiceFactoryManager(AppServiceCache, meta);
		}
		public virtual IServiceResolver OnCreateServiceResolver(Internals.IServiceFactoryManager Manager)
		{
			return new ServiceResolver(Manager);
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
			IServiceInterface Interface,
			IServiceInterface CallInterface,
			IServiceMetadata Meta, 
			HashSet<Type> ValidatedInterfaces,
			HashSet<Type> UsedServiceInterfaces
			)
		{
			if (!UsedServiceInterfaces.Add(Interface.Type))
				throw new InvalidOperationException(
					$"服务{Interface.Type}存在循环引用，来自{CallInterface?.ImplementType}"
					);

			if (!ValidatedInterfaces.Contains(Interface.Type) && Interface.ServiceImplementType==ServiceImplementType.Type)
			{
				(from arg in Internals.ServiceCreatorBuilder.FindBestConstructorInfo(Interface.ImplementType).GetParameters()
				 let type = GetRealServiceType(arg.ParameterType)
				 where type.IsInterface
				 let svcs = Meta.Services.Get(type)
				select new { arg, svcs, type }
				).ForEach(tuple => {
					if (tuple.svcs == null )
						throw new InvalidOperationException(
							$"找不到{Interface.ImplementType}的构造函数参数 {tuple.arg.Name}引用的服务{tuple.type}"
							);
					foreach(var svc in tuple.svcs.Implements)
						foreach(var isf in svc.Interfaces)
							validate(isf,Interface, Meta, ValidatedInterfaces, UsedServiceInterfaces);
				}
				);

				ValidatedInterfaces.Add(Interface.Type);
			}
			UsedServiceInterfaces.Remove(Interface.Type);
		}
		public virtual void OnValidate(IServiceMetadata Meta)
		{
			var validated = new HashSet<Type>();
			var used = new HashSet<Type>();
			foreach(var sif in (from svc in Meta.Services.Values
			 from impl in svc.Implements
			 from sif in impl.Interfaces
				select sif

			 ))
			validate(sif,null, Meta, validated, used);
		}
		static object NewLazy<S,I>(IServiceProvider sp) where S:class where I : class
		{
			return new Lazy<I>(() => sp.Resolve<S,I>());
		}
		static object NewCreator<S,I>(IServiceProvider sp) where S:class where I : class
		{
			return new Func<I>(() => sp.Resolve<S, I>());
		}


		static object NewInstanceCreator<S,I>(IServiceProvider sp) where S : class where I : class
		{
			return new Func<long, I>(id => sp.Resolve<S, I>(id));
		}
		static object NewInstances<T>(IServiceProvider sp,int count) where T : class
		{
			return Enumerable.Range(0, count).Select(i =>
				(T)((IServiceResolver)sp).Resolve(0,typeof(T), -1-i, typeof(T))
				);
		}


		static MethodInfo NewLazyMethodInfo = typeof(ServiceResolverBuilder)
				.GetMethodExt(nameof(NewLazy), typeof(IServiceProvider));
		static MethodInfo NewCreatorMethodInfo = typeof(ServiceResolverBuilder)
			.GetMethodExt(nameof(NewCreator), typeof(IServiceProvider));


		static MethodInfo NewInstanceCreatorMethodInfo = typeof(ServiceResolverBuilder)
			.GetMethodExt(nameof(NewInstanceCreator), typeof(IServiceProvider));

		static MethodInfo NewInstancesMethodInfo = typeof(ServiceResolverBuilder)
			.GetMethodExt(nameof(NewInstances), typeof(IServiceProvider),typeof(int));

		public virtual void PrepareServices(IServiceCollection Services)
		{
			Services.SelectMany(svc=>svc.Interfaces,(svc,i)=>new { svc, i })
				.Where(svc => svc.i.ServiceImplementType == ServiceImplementType.Type)
				.Distinct()
				.ToArray()
				.ForEach(svc =>
				{
					var newLazy = NewLazyMethodInfo
						.MakeGenericMethod(svc.svc.ServiceType, svc.i.InterfaceType)
						.CreateDelegate<Func<IServiceProvider, object>>();
					Services.Add(new ServiceDescriptor(
						typeof(Lazy<>).MakeGenericType(svc.i.InterfaceType),
						newLazy,
						svc.i.Lifetime
						));


					var newCreator = NewCreatorMethodInfo
						.MakeGenericMethod(svc.svc.ServiceType, svc.i.InterfaceType)
						.CreateDelegate<Func<IServiceProvider, object>>();
					Services.Add(new ServiceDescriptor(
						typeof(Func<>).MakeGenericType( svc.i.InterfaceType),
						newCreator,
						svc.i.Lifetime
						));


					var newInstanceCreatorSimple = NewInstanceCreatorMethodInfo
						.MakeGenericMethod(svc.svc.ServiceType,svc.i.InterfaceType)
						.CreateDelegate<Func<IServiceProvider, object>>();
					Services.Add(new ServiceDescriptor(
						typeof(Func<,>).MakeGenericType(typeof(long), svc.i.InterfaceType),
						newInstanceCreatorSimple,
						svc.i.Lifetime
						));

				});

			Services.SelectMany(s=>s.Interfaces)
				.GroupBy(s => s.InterfaceType)
				.Select(g=>new { Key = g.Key, Count = g.Count() })
				.ToArray()
				.ForEach(g =>
				{
					var newInstances = NewInstancesMethodInfo
							.MakeGenericMethod(g.Key)
							.CreateDelegate<Func<IServiceProvider, int, object>>();
					Services.Add(
						new ServiceDescriptor(
							typeof(IEnumerable<>).MakeGenericType(g.Key),
							sp=>newInstances(sp,g.Count),
							ServiceImplementLifetime.Transient
							)
						);
				});
		}
		public IServiceResolver Build(IServiceCollection Services,Caching.ILocalCache<object> AppServiceCache)
		{

			PrepareServices(Services);

			IServiceResolver resolver = null;
			IServiceMetadata meta = null;
			Internals.IServiceFactoryManager manager = null;
			IServiceScopeFactory scopeFactory = null;

			Services.AddSingleton(sp => resolver);
			Services.AddSingleton(sp => (IServiceProvider)resolver);
			Services.AddSingleton(sp => meta);
			Services.AddSingleton(sp => manager);
			Services.AddSingleton(sp => scopeFactory );
			Services.AddSingleton(sp => (IServiceInstanceConfigChangedNotifier)manager);

			meta = OnCreateServcieMetadata(Services);
			OnValidate(meta);
			manager = OnCreateServcieFactoryManager(meta, AppServiceCache);
			scopeFactory = new ServiceScopeFactory(manager);
			resolver = OnCreateServiceResolver(manager);
			return resolver;
		}
	}

}
