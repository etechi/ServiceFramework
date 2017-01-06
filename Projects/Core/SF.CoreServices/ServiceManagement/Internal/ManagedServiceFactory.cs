using System;

using System.Collections.Concurrent;

namespace SF.ServiceManagement.Internal
{
	class ManagedServiceFactory : IManagedServiceFactory,IManagedServiceConfigChangedNotifier
	{
		class ServiceImplementEntry
		{
			public string Id { get; }
			public Type ServiceType { get; }
			public Type ImplementType { get; private set; }
			Internal.IServiceCreateParameterTemplate _CreateParameterTemplate;
			Internal.ServiceFactory _Factory;

			public ServiceImplementEntry(string Id,Type ServiceType)
			{
				this.Id = Id;
				this.ServiceType = ServiceType;
			}
			public void Ensure(
				IServiceProvider ServiceProvider, 
				Internal.IServiceMetadata ServiceMetadata
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

					var ci = Internal.ServiceFactoryBuilder.FindBestConstructorInfo(ImplementType);

					_CreateParameterTemplate = Internal.ServiceCreateParameterTemplate.Load(
						ci,
						cfg.CreateArguments,
						ServiceMetadata
						);

					_Factory = Internal.ServiceFactoryBuilder.Build(
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
		ConcurrentDictionary<string, ServiceImplementEntry> IdentServiceMap { get; } = new ConcurrentDictionary<string, ServiceImplementEntry>();


		Internal.IServiceMetadata ServiceMetadata { get; }

		public ManagedServiceFactory(
			Internal.IServiceMetadata ServiceMetadata
			)
		{
			this.ServiceMetadata = ServiceMetadata;
		}
		public object Create(IServiceProvider ServiceProvider, IManagedServiceScope ManagedServiceScope, Type type, string Id)
		{
			if (Id == null)
			{
				if (!TypeServiceMap.TryGetValue(type.FullName, out var id))
				{
					var DefaultServiceLocator = (IDefaultServiceLocator)ServiceProvider.GetService(typeof(IDefaultServiceLocator));
					if (DefaultServiceLocator == null)
						throw new InvalidOperationException($"找不到服务{typeof(IDefaultServiceLocator)}");
					Id = DefaultServiceLocator.Locate(type.FullName);
					if (Id == null)
						throw new ArgumentException($"找不到服务类型:{type.FullName}的默认服务");
				}
			}

			if (!IdentServiceMap.TryGetValue(Id, out var entry))
				entry=IdentServiceMap.GetOrAdd(Id, new ServiceImplementEntry(Id,type));

			entry.Ensure(ServiceProvider, ServiceMetadata);
			return entry.Create(ServiceProvider, ManagedServiceScope);
		}


		void IManagedServiceConfigChangedNotifier.NotifyChanged(string Id)
		{
			IdentServiceMap.TryRemove(Id, out var v);
		}

		void IManagedServiceConfigChangedNotifier.NotifyDefaultChanged(string Type)
		{
			TypeServiceMap.TryRemove(Type, out var v);
		}
	}

}
