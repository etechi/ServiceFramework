using System;
using System.Collections.Generic;

using System.Linq;
using System.Collections;
using System.Reflection;
using SF.Core.ServiceManagement.Internals;
using SF.Core.ServiceManagement;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement
{

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
					$"����{Implement.ImplementType}����ѭ�����ã�����{CallImplement?.ImplementType}"
					);

			if (!ValidatedImplements.Contains(Implement.ImplementType) && Implement.ServiceImplementType==ServiceImplementType.Type)
			{
				(from arg in Internals.ServiceCreatorBuilder.FindBestConstructorInfo(Implement.ImplementType, Meta).GetParameters()
				 let type = GetRealServiceType(arg.ParameterType)
				 where type.IsInterface && !type.IsDefined(typeof(AutoBindAttribute))
				 let svcs = Meta.FindServiceByType(type)
				select new { arg, svcs, type }
				).ForEach(tuple => {
					
					if (tuple.svcs == null )
						throw new InvalidOperationException(
							$"�Ҳ���{Implement.ImplementType}�Ĺ��캯������ {tuple.arg.Name}���õķ���{tuple.type}"
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
		{
			var sr = sp.Resolver();
			var curScopeId = sr.CurrentServiceId;
			return new Func<I>(() =>
			{
				using (sr.WithScopeService(curScopeId)) 
					return sp.Resolve<I>();
			});
		}
		static object NewLazy<I>(IServiceProvider sp) where I : class
		{
			var sr = sp.Resolver();
			var curScopeId = sr.CurrentServiceId;
			return new Lazy<I>(() =>
			{
				using (sr.WithScopeService(curScopeId)) 
					return sp.Resolve<I>();
			});
		}
		static object NewTypedInstanceResolver<I>(IServiceProvider sp) where I : class
		{
			var sr = sp.Resolver();
			var curScopeId = sr.CurrentServiceId;
			return new TypedInstanceResolver<I>((id) =>
			{
				using(sr.WithScopeService(curScopeId))
					return sp.Resolve<I>(id);
			});
		}
		static object NewNamedInstanceResolver<I>(IServiceProvider sp) where I : class
		{
			var sr = sp.Resolver();
			var curScopeId = sr.CurrentServiceId;
			return new NamedServiceResolver<I>((name) =>
			{
				using (sr.WithScopeService(curScopeId))
					return sp.TryResolve<I>(name);
			});
		}
		static IEnumerable<I> NewEnumerableReal<I>(IServiceProvider sp,long? scopeId) where I : class
		{
			var resolver = sp.Resolver();
			//var desc = (IServiceInstanceDescriptor)resolver.ResolveServiceByType(resolver.CurrentServiceId,typeof(IServiceInstanceDescriptor),null);
			foreach (var i in resolver.ResolveServices(scopeId, typeof(I), null))
				yield return (I)i;
		}
		static object NewEnumerable<I>(IServiceProvider sp) where I : class
		{
			return NewEnumerableReal<I>(sp, sp.Resolver().CurrentServiceId);
		}


		static WithNewScope<I1, I2, I3, I4, I5, I6, I7, I8, R> CreateWithNewScope8<I1, I2, I3, I4, I5, I6, I7, I8, R>(IServiceProvider ServiceProvider) 
		{
			return (callback)=>
				ServiceProvider.WithScope(async sp =>
					await sp.Invoke(callback)
				);
		}
		static WithNewScope<I1, I2, I3, I4, I5, I6, I7, R> CreateWithNewScope7<I1, I2, I3, I4, I5, I6, I7, R>(IServiceProvider ServiceProvider)
		{
			return (callback) =>
				ServiceProvider.WithScope(async sp =>
					await sp.Invoke(callback)
				);
		}
		static WithNewScope<I1, I2, I3, I4, I5, I6, R> CreateWithNewScope6<I1, I2, I3, I4, I5, I6, R>(IServiceProvider ServiceProvider)
		{
			return (callback) =>
				ServiceProvider.WithScope(async sp =>
					await sp.Invoke(callback)
				);
		}
		static WithNewScope<I1, I2, I3, I4, I5, R> CreateWithNewScope5<I1, I2, I3, I4, I5, R>(IServiceProvider ServiceProvider)
		{
			return (callback) =>
				ServiceProvider.WithScope(async sp =>
					await sp.Invoke(callback)
				);
		}
		static WithNewScope<I1, I2, I3, I4, R> CreateWithNewScope4<I1, I2, I3, I4, R>(IServiceProvider ServiceProvider)
		{
			return (callback) =>
				ServiceProvider.WithScope(async sp =>
					await sp.Invoke(callback)
				);
		}
		static WithNewScope<I1, I2, I3, R> CreateWithNewScope3<I1, I2, I3, R>(IServiceProvider ServiceProvider)
		{
			return (callback) =>
				ServiceProvider.WithScope(async sp =>
					await sp.Invoke(callback)
				);
		}
		static WithNewScope<I1, I2, R> CreateWithNewScope2<I1, I2, R>(IServiceProvider ServiceProvider)
		{
			return (callback) =>
				ServiceProvider.WithScope(async sp =>
					await sp.Invoke(callback)
				);
		}
		static WithNewScope<I1, R> CreateWithNewScope1<I1, R>(IServiceProvider ServiceProvider)
		{
			return (callback) =>
				ServiceProvider.WithScope(async sp =>
					await sp.Invoke(callback)
				);
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
			//Services.AddTransient(typeof(IEnumerable<>), typeof(ManagedServices.Runtime.EnumerableService<>));
			Services.AddTransient(typeof(IEnumerable<>), typeof(ServiceProviderBuilder).GetMethodExt(nameof(NewEnumerable), typeof(IServiceProvider)));
			Services.AddTransient(typeof(Func<>), typeof(ServiceProviderBuilder).GetMethodExt(nameof(NewCreator), typeof(IServiceProvider)));
			Services.AddTransient(typeof(Lazy<>), typeof(ServiceProviderBuilder).GetMethodExt(nameof(NewLazy), typeof(IServiceProvider)));
			Services.AddTransient(typeof(TypedInstanceResolver<>), typeof(ServiceProviderBuilder).GetMethodExt(nameof(NewTypedInstanceResolver), typeof(IServiceProvider)));
			Services.AddTransient(typeof(NamedServiceResolver<>), typeof(ServiceProviderBuilder).GetMethodExt(nameof(NewNamedInstanceResolver), typeof(IServiceProvider)));

			Services.AddTransient(typeof(WithNewScope<,,,,,,,,>), typeof(ServiceProviderBuilder).GetMethodExt(nameof(CreateWithNewScope8), typeof(IServiceProvider)));
			Services.AddTransient(typeof(WithNewScope<,,,,,,,>), typeof(ServiceProviderBuilder).GetMethodExt(nameof(CreateWithNewScope7), typeof(IServiceProvider)));
			Services.AddTransient(typeof(WithNewScope<,,,,,,>), typeof(ServiceProviderBuilder).GetMethodExt(nameof(CreateWithNewScope6), typeof(IServiceProvider)));
			Services.AddTransient(typeof(WithNewScope<,,,,,>), typeof(ServiceProviderBuilder).GetMethodExt(nameof(CreateWithNewScope5), typeof(IServiceProvider)));
			Services.AddTransient(typeof(WithNewScope<,,,,>), typeof(ServiceProviderBuilder).GetMethodExt(nameof(CreateWithNewScope4), typeof(IServiceProvider)));
			Services.AddTransient(typeof(WithNewScope<,,,>), typeof(ServiceProviderBuilder).GetMethodExt(nameof(CreateWithNewScope3), typeof(IServiceProvider)));
			Services.AddTransient(typeof(WithNewScope<,,>), typeof(ServiceProviderBuilder).GetMethodExt(nameof(CreateWithNewScope2), typeof(IServiceProvider)));
			Services.AddTransient(typeof(WithNewScope<,>), typeof(ServiceProviderBuilder).GetMethodExt(nameof(CreateWithNewScope1), typeof(IServiceProvider)));


			meta = OnCreateServcieMetadata(Services);
			OnValidate(meta);
			manager = OnCreateServcieFactoryManager(
				meta, 
				AppServiceCache
				);
			provider = OnCreateServiceProvider(manager);
			scopeFactory = new ServiceScopeFactory(provider, manager);
			manager.SubscribeEvents(provider);
			return provider;
		}
	}

}
