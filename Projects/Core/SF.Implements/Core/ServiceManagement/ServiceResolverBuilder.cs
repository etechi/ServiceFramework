using System;
using System.Collections.Generic;

using System.Linq;
using System.Collections;
using System.Reflection;
using SF.Core.ServiceManagement.Internals;

namespace SF.Core.ServiceManagement
{
	public class ServiceResolverBuilder : IServiceResolverBuilder
	{
		public virtual IServiceMetadata OnCreateServcieMetadata(IServiceCollection Services)
		{
			return new Internals.ServiceMetadata(Services);
		}
		public virtual Internals.IServiceFactoryManager OnCreateServcieFactoryManager(IServiceMetadata meta)
		{
			return new Internals.ServiceFactoryManager(meta);
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
			IServiceImplement Implement,
			IServiceDeclaration Service,
			IServiceMetadata Meta, 
			HashSet<Type> ValidatedServices,
			HashSet<Type> UsedServices
			)
		{
			if (!UsedServices.Add(Service.ServiceType))
				throw new InvalidOperationException(
					$"服务{Service.ServiceType}存在循环引用，来自{Implement.ImplementType}"
					);

			if (!ValidatedServices.Contains(Service.ServiceType))
			{
				(from impl in Service.Implements
				 where impl.ServiceImplementType == ServiceImplementType.Type
				 from arg in Internals.ServiceCreatorBuilder.FindBestConstructorInfo(impl.ImplementType).GetParameters()
				 let type = GetRealServiceType(arg.ParameterType)
				 where type.IsInterface
				 let svc = Meta.Services.Get(type)
				select new { impl, arg, svc, type }
				).ForEach(tuple => {
					if (tuple.svc == null)
						throw new InvalidOperationException(
							$"找不到{tuple.impl.ImplementType}的构造函数参数 {tuple.arg.Name}引用的服务{tuple.type}"
							);
					validate(tuple.impl, tuple.svc, Meta, ValidatedServices, UsedServices);
				}
				);

				ValidatedServices.Add(Service.ServiceType);
			}
			UsedServices.Remove(Service.ServiceType);
		}
		public virtual void OnValidate(IServiceMetadata Meta)
		{
			var validated = new HashSet<Type>();
			var used = new HashSet<Type>();
			Meta.Services.Values.ForEach(svc => validate(null, svc, Meta, validated, used));
		}
		static object NewLazy<T>(IServiceProvider sp) where T:class
		{
			return new Lazy<T>(() => sp.Resolve<T>());
		}
		static object NewCreator<T>(IServiceProvider sp) where T:class
		{
			return new Func<T>(() => sp.Resolve<T>());
		}

		static object NewInstanceCreator<T>(IServiceProvider sp) where T : class
		{
			return new Func<long,T>(id =>(T) ((IServiceResolver)sp).Resolve(typeof(T),id));
		}
		static object NewInstances<T>(IServiceProvider sp,int count) where T : class
		{
			return Enumerable.Range(0, count).Select(i =>
				(T)((IServiceResolver)sp).Resolve(typeof(T), -1-i)
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
						typeof(Func<>).MakeGenericType(svc.InterfaceType),
						newCreator,
						svc.Lifetime
						));


					var newInstanceCreator = NewInstanceCreatorMethodInfo
						.MakeGenericMethod(svc.InterfaceType)
						.CreateDelegate<Func<IServiceProvider, object>>();
					Services.Add(new ServiceDescriptor(
						typeof(Func<,>).MakeGenericType(typeof(long), svc.InterfaceType),
						newInstanceCreator,
						svc.Lifetime
						));
				});

			Services
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
		public IServiceResolver Build(IServiceCollection Services)
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
			manager = OnCreateServcieFactoryManager(meta);
			scopeFactory = new ServiceScopeFactory(manager);
			resolver = OnCreateServiceResolver(manager);
			return resolver;
		}
	}

}
