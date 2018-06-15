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
using System.Collections.Generic;

using System.Linq;
using System.Collections;
using System.Reflection;
using SF.Sys.Services.Internals;
using SF.Sys.Services;
using System.Threading.Tasks;
using SF.Sys.Reflection;
using SF.Sys.Linq;

namespace SF.Sys.Services
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
			HashSet<Type> UsedServiceImplements,
			string RefPath
			)
		{
			if (!UsedServiceImplements.Add(Implement.ImplementType))
				throw new InvalidOperationException(
					$"{RefPath} ����{Implement.ImplementType}����ѭ�����ã�����{CallImplement?.ImplementType}"
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
							$"{RefPath} �Ҳ���{Implement.ImplementType}�Ĺ��캯������ {tuple.arg.Name}���õķ���{tuple.type}"
							);
					foreach(var svc in tuple.svcs.Implements)
							validate(svc, Implement, Meta, ValidatedImplements, UsedServiceImplements,RefPath+svc.ImplementName+"/");
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
			validate(sif,null, Meta, validated, used,"");
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
			if (typeof(I).FullName.Contains("ActionDescriptorProvider"))
				scopeId = scopeId;
			var resolver = sp.Resolver();
			//var desc = (IServiceInstanceDescriptor)resolver.ResolveServiceByType(resolver.CurrentServiceId,typeof(IServiceInstanceDescriptor),null);
			foreach (var i in resolver.ResolveServices(scopeId, typeof(I), null))
				yield return (I)i;
		}
		static object NewEnumerable<I>(IServiceProvider sp) where I : class
		{
			return NewEnumerableReal<I>(sp, sp.Resolver().CurrentServiceId);
		}

		class Scoped<S> : IScoped<S> where S : class
		{
			IServiceScopeFactory ScopeFactory { get; }
			long? CurScopeId { get; }
			public Scoped(IServiceProvider ServiceProvider)
			{
				ScopeFactory = ServiceProvider.Resolve<IServiceScopeFactory>();
				CurScopeId = ServiceProvider.Resolver().CurrentServiceId;
			}
			public async Task<T> Use<T>(Func<S, Task<T>> Callback)
			{
				using (var ss = ScopeFactory.CreateServiceScope())
				{
					var isp = ss.ServiceProvider;
					var isr = isp.Resolver();
					using (isr.WithScopeService(CurScopeId))
						return await isp.WithServices<S, Task<T>>(Callback);
				}
			}
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

			Services.Add(typeof(IScoped<>), typeof(Scoped<>), ServiceImplementLifetime.Transient);


			meta = OnCreateServcieMetadata(Services);
			OnValidate(meta);
			manager = OnCreateServcieFactoryManager(
				meta, 
				AppServiceCache
				);
			provider = OnCreateServiceProvider(manager);
			scopeFactory = new ServiceScopeFactory(provider, manager);
			manager.BindServiceProvider(provider);
			return provider;
		}
	}

}
