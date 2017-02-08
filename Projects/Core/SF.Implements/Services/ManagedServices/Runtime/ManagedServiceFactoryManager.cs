using System;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SF.Services.ManagedServices.Runtime
{
	class ManagedServiceFactoryManager : IManagedServiceFactoryManager,IManagedServiceConfigChangedNotifier
	{
		
		class ManagedServiceFactory : IManagedServiceFactory
		{
			public string Id { get; }
			public Type ServiceType { get; }
			public Type ImplementType { get; private set; }
			public bool IsScopedLifeTime { get; private set; }
			Runtime.IServiceCreateParameterTemplate _CreateParameterTemplate;
			Runtime.ServiceFactory _Factory;

			public ManagedServiceFactory(string Id,Type ServiceType)
			{
				this.Id = Id;
				this.ServiceType = ServiceType;
			}
			public void Ensure(
				IServiceProvider ServiceProvider, 
				Runtime.IServiceMetadata ServiceMetadata,
				HashSet<Tuple<Type,Type>> ScopedLifetimeServices
				)
			{
				if (_Factory != null)
					return;
				lock (this)
				{
					if (_Factory != null)
						return;

					var CfgLoader = (IServiceConfigLoader)ServiceProvider.GetService(typeof(IServiceConfigLoader));
					if(CfgLoader==null)
						throw new InvalidOperationException($"找不到服务{typeof(IServiceConfigLoader)}");


					var cfg = CfgLoader.GetConfig(Id);
					if (cfg.ServiceType != ServiceType.FullName)
						throw new InvalidOperationException($"服务配置({Id})返回类型({cfg.ServiceType})与实际所需类型({cfg.ServiceType})不符");

					var ImplementTypeResolver = (IServiceImplementTypeResolver)ServiceProvider.GetService(typeof(IServiceImplementTypeResolver));
					if(ImplementTypeResolver==null)
						throw new InvalidOperationException($"找不到服务{typeof(IServiceImplementTypeResolver)}");

					ImplementType = ImplementTypeResolver.Resolve(cfg.ImplementType);
					if(ImplementType==null)
						throw new InvalidOperationException($"找不到服务配置({Id})指定的服务实现类型({cfg.ServiceType}),服务:{ServiceType}");

					IsScopedLifeTime = ScopedLifetimeServices.Contains(Tuple.Create(ServiceType, ImplementType));
					var ci = Runtime.ServiceFactoryBuilder.FindBestConstructorInfo(ImplementType);

					_CreateParameterTemplate = Runtime.ServiceCreateParameterTemplate.Load(
						ci,
						Id,
						cfg.CreateArguments,
						ServiceMetadata
						);

					_Factory = Runtime.ServiceFactoryBuilder.Build(
						ServiceMetadata,
						ImplementType,
						ci
						);


				}
			}
			public object Create(IServiceProvider ServiceProvider, IManagedServiceScope ManagedServiceScope)
			{
				return _Factory(ServiceProvider, ManagedServiceScope, _CreateParameterTemplate);
			}
		}
		ConcurrentDictionary<string, string> TypeServiceMap { get; } = new ConcurrentDictionary<string, string>();
		ConcurrentDictionary<string, ManagedServiceFactory> FactoryMap { get; } = new ConcurrentDictionary<string, ManagedServiceFactory>();

		HashSet<Tuple<Type, Type>> ScopedLifeTimes { get; }

		Runtime.IServiceMetadata ServiceMetadata { get; }

		public ManagedServiceFactoryManager(
			Runtime.IServiceMetadata ServiceMetadata
			)
		{
			this.ServiceMetadata = ServiceMetadata;
			ScopedLifeTimes = new HashSet<Tuple<Type, Type>>(
				from pair in ServiceMetadata.ManagedServices
				from i in pair.Value
				where i.IsScopedLifeTime
				select Tuple.Create(pair.Key, i.Type)
				);
		}
		string ResolveDefaultService(IServiceProvider ServiceProvider, Type type, string Id)
		{
			if (Id != null)
				return Id;
			if (!TypeServiceMap.TryGetValue(type.FullName, out Id))
			{
				var DefaultServiceLocator = (IDefaultServiceLocator)ServiceProvider.GetService(typeof(IDefaultServiceLocator));
				if (DefaultServiceLocator == null)
					throw new InvalidOperationException($"找不到服务{typeof(IDefaultServiceLocator)}");
				Id = DefaultServiceLocator.Locate(type.FullName);
				if (Id == null)
					throw new ArgumentException($"找不到服务类型:{type.FullName}的默认服务");
				Id = TypeServiceMap.GetOrAdd(type.FullName, Id);
			}
			return Id;
		}
		public IManagedServiceFactory GetServiceFactory(IServiceProvider ServiceProvider, Type type, string Id)
		{
			Id = ResolveDefaultService(ServiceProvider, type, Id);

			ManagedServiceFactory entry;
			if (!FactoryMap.TryGetValue(Id, out entry))
				entry = FactoryMap.GetOrAdd(Id, new ManagedServiceFactory(Id, type));

			entry.Ensure(ServiceProvider, ServiceMetadata, ScopedLifeTimes);
			return entry;
		}


		void IManagedServiceConfigChangedNotifier.NotifyChanged(string Id)
		{
			ManagedServiceFactory v;
			FactoryMap.TryRemove(Id, out v);
		}

		void IManagedServiceConfigChangedNotifier.NotifyDefaultChanged(string Type)
		{
			string v;
			TypeServiceMap.TryRemove(Type, out v);
		}
	}

}
