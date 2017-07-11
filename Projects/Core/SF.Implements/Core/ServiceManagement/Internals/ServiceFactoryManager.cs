using System;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SF.Core.ServiceManagement.Internals
{
	//struct ServiceKey
	//{
	//	public string ServiceType;
	//	public string Id;
	//}
	//class ServiceKeyComparer : IEqualityComparer<ServiceKey>
	//{
	//	public static ServiceKeyComparer Instance { get; } = new ServiceKeyComparer();

	//	public bool Equals(ServiceKey x, ServiceKey y)
	//	{
	//		return x.ServiceType == y.ServiceType &&
	//			x.Id == y.Id;
	//	}

	//	public int GetHashCode(ServiceKey obj)
	//	{
	//		var c = obj.Id.GetHashCode() ^
	//			obj.ServiceType.GetHashCode();
	//		return c;
	//	}
	//}
	class ServiceInterfaceFactorySet
	{
		static System.Collections.Concurrent.ConcurrentDictionary<(Type, Type), (ServiceCreator, System.Reflection.ConstructorInfo)> CreaterDict = new ConcurrentDictionary<(Type, Type), (ServiceCreator, System.Reflection.ConstructorInfo)>();
		static (ServiceCreator,System.Reflection.ConstructorInfo) GetServiceCreator(Type ServiceType,Type ImplementType,IServiceMetadata ServiceMetadata)
		{
			return CreaterDict.GetOrAdd((ImplementType, ServiceType), (ValueTuple<Type,Type> pair)=>
			{
				var it = pair.Item1;
				var st = pair.Item2;
				  var ci = Internals.ServiceCreatorBuilder
						  .FindBestConstructorInfo(it)
						  .AssertNotNull(
							  () => $"找不到服务实现类型{ImplementType}的构造函数"
							  );

				  var Creator = ServiceCreatorBuilder.Build(
					  ServiceMetadata,
					  st,
					  it,
					  ci
					  );
				  return (Creator, ci);
			  }
			 );
		}

		public AppServiceSet AppServiceSet { get; }
		public int AppId => AppServiceSet.AppId;
		public IServiceMetadata ServiceMetadata => AppServiceSet.ServiceMetadata;

		public long Id { get; }
		public IServiceImplement Implement { get; }
		public Lazy<IServiceFactory>[] Factories { get; }
		public IServiceCreateParameterTemplate Template { get; }
		
		public IServiceConfig Config { get; }
		public ServiceInterfaceFactorySet(
			long Id,
			IServiceImplement Implement,
			IServiceConfig Config,
			AppServiceSet AppServiceSet
			)
		{
			this.AppServiceSet = AppServiceSet;
			this.Id = Id;
			this.Implement = Implement;
			this.Config = Config;
			Factories = Implement.Interfaces.Select(i =>
				new Lazy<IServiceFactory>(() => CreateInterfaceFactory(i))
				).ToArray();
		}
		IServiceFactory CreateInterfaceFactory(IServiceInterface ServiceInterface)
		{
			ServiceCreator Creator;
			IServiceCreateParameterTemplate CreateParameterTemplate = null;
			switch (ServiceInterface.ServiceImplementType)
			{
				case ServiceImplementType.Creator:
					var func = ServiceInterface.ImplementCreator;
					Creator = (sp, ctr) => func(sp);
					break;
				case ServiceImplementType.Instance:
					var ins = ServiceInterface.ImplementInstance;
					Creator = (sp, ctr) => ins;
					break;
				case ServiceImplementType.Type:

					var implType = ServiceInterface.ImplementType.IsGenericTypeDefinition ?
						ServiceInterface.ImplementType.MakeGenericType(Implement.ServiceType.GetGenericArguments()) :
						ServiceInterface.ImplementType;
					System.Reflection.ConstructorInfo ci;
					(Creator,ci) = GetServiceCreator(Implement.ServiceType, implType, ServiceMetadata);
					var setting = Config.Settings.Get(ServiceInterface.ImplementType.FullName);
					CreateParameterTemplate = ServiceCreateParameterTemplate.Load(
						ci,
						AppId,
						Id,
						(setting?.ImplementType== ServiceInterface.ImplementType.FullName?setting?.Setting:null),
						ServiceMetadata
						);
					break;
				default:
					throw new NotSupportedException();
			}
			return null;
		}
		public IServiceFactory GetFactory(Type InterfaceType)
		{
			var count = Factories.Length;
			var iis = Implement.Interfaces;
			for (var i = 0; i < count; i++)
			{
				if (InterfaceType == iis[i].Type)
					return Factories[i].Value;
			}
			return null;
		}

	}

	class AppServiceSet
	{
		public int AppId { get; }
		public IServiceMetadata ServiceMetadata { get; }
		public ConcurrentDictionary<string, HashSet<string>> GenericTypes { get; } = new ConcurrentDictionary<string, HashSet<string>>();
		public ConcurrentDictionary<string, long> DefaultServiceImplementMap { get; } = new ConcurrentDictionary<string, long>();
		public ConcurrentDictionary<long, ServiceInterfaceFactorySet> FactoryMap { get; } = new ConcurrentDictionary<long, ServiceInterfaceFactorySet>();
		public AppServiceSet(int AppId, IServiceMetadata ServiceMetadata)
		{
			this.AppId = AppId;
			this.ServiceMetadata = ServiceMetadata;
		}
		(IServiceImplement, IServiceConfig) LoadImplementById(
			IServiceResolver ServiceResolver,
			Type ServiceType,
			long Id
		)
		{
			var (ImplementType, cfg) =
				ServiceResolver.WithScope(sp =>
				{
					var ImplementTypeResolver = sp.Resolve<IServiceImplementTypeResolver>();
					var icfg = sp.Resolve<IServiceConfigLoader>()
						.GetConfig(ServiceType.FullName, AppId, Id)
						.Assert(
						v => v?.ServiceType == ServiceType.FullName,
						v => $"服务配置({Id})返回类型({v?.ServiceType})与实际所需类型({ServiceType})不符"
						);

					var implType = ImplementTypeResolver
					.Resolve(icfg.Settings[icfg.ServiceType].ImplementType)
					.AssertNotNull(
						() => $"找不到服务配置({Id})指定的服务实现类型({icfg.Settings[icfg.ServiceType].ImplementType}),服务:{ServiceType}"
						);
					return (implType, icfg);
				});

			var setting = cfg.Settings;

			var impl = ServiceMetadata.Services
				.Get(ServiceType)?.Implements
				.SingleOrDefault(i => i.Interfaces.Single(ii => ii.Type == ServiceType).ImplementType == ImplementType)
				.AssertNotNull(
					() => $"找不到服务配置({Id})指定的服务实现类型({ImplementType}),服务:{ServiceType}"
					);
			return (impl, cfg);
		}
		ServiceInterfaceFactorySet CreateServiceFactorySet(
			IServiceResolver ServiceResolver,
			Type ServiceType,
			IServiceDeclaration ServiceDeclaration,
			long Id
			)
		{
			IServiceImplement ServiceImplement;
			IServiceConfig cfg = null;
			if (Id == 0)
				ServiceImplement = ServiceDeclaration.Implements.Last();
			else if (Id<0)
			{
				var idx = ServiceDeclaration.Implements.Count + Id;
				if (idx<0 || idx >= ServiceDeclaration.Implements.Count)
					throw new IndexOutOfRangeException($"超出服务索引限制{ServiceType}");
				ServiceImplement = ServiceDeclaration.Implements[(int)idx];
			}
			else
				(ServiceImplement, cfg) = LoadImplementById(ServiceResolver,ServiceType, Id);

			return new ServiceInterfaceFactorySet(
				Id,
				ServiceImplement,
				cfg,
				this
				);
		}

		long ResolveDefaultService(IServiceProvider ServiceProvider, Type ServiceType, long Id)
		{
			if (Id != 0)
				return Id;

			if (DefaultServiceImplementMap.TryGetValue(ServiceType.FullName, out Id))
				return Id;

			if (ServiceType.IsDefined(typeof(UnmanagedServiceAttribute), true))
				Id = 0;
			else
			{
				var DefaultServiceLocator = ServiceProvider
					.TryResolve<IDefaultServiceLocator>();
				if (DefaultServiceLocator == null)
					Id = 0;
				else
				{
					//.AssertNotNull(
					//	() => $"找不到服务{typeof(IDefaultServiceLocator)}"
					//	);

					var LocateResult = DefaultServiceLocator.Locate(ServiceType.FullName, AppId);
					if (LocateResult != 0)
						Id = LocateResult;
					else
					{
						//ServiceMetadata.Services.Get(ServiceType)
						//	.Implements
						//	.Error(
						//		v => v.Count == 0,
						//		v => $"没有服务实现"
						//	)
						//	.Error(
						//		v => v.Count > 1,
						//		v => $"找不到默认服务"
						//		);
						Id = 0;
					}
				}
			}
			Id = DefaultServiceImplementMap.GetOrAdd(ServiceType.FullName, Id);
			return Id;
		}

		public IServiceFactory GetServiceFactory(
			IServiceResolver ServiceResolver,
			Type ServiceType,
			long ServiceInstanceId,
			Type InterfaceType
			)
		{

			if (ServiceType == null)
				throw new ArgumentNullException();
			if (ServiceType.IsGenericTypeDefinition)
				throw new NotSupportedException();

			var RealServiceInstanceId = ResolveDefaultService(
				ServiceResolver,
				ServiceType,
				ServiceInstanceId
				);
			
			ServiceInterfaceFactorySet factorySet;
			if (FactoryMap.TryGetValue(RealServiceInstanceId, out factorySet))
				return factorySet.GetFactory(InterfaceType);
			var ServiceDeclare = ServiceMetadata.Services.Get(ServiceType);
			if (ServiceDeclare != null)
			{
				return FactoryMap.GetOrAdd(
					RealServiceInstanceId,
					(k) => CreateServiceFactorySet(
						ServiceResolver,
						ServiceType,
						ServiceDeclare,
						RealServiceInstanceId
						)
					).GetFactory(InterfaceType);
			}


			if (!ServiceType.IsGenericType)
				return null;

			var TypeArgs = ServiceType.GetGenericArguments();
			var ServiceTypeDef = ServiceType.GetGenericTypeDefinition();

			ServiceDeclare = ServiceMetadata.Services.Get(ServiceTypeDef);
			if (ServiceDeclare == null)
				return null;

			if (!FactoryMap.TryGetValue(RealServiceInstanceId, out factorySet))
				factorySet = FactoryMap.GetOrAdd(
					RealServiceInstanceId,
					(k) => {
						var types = GenericTypes.GetOrAdd(ServiceTypeDef.FullName, kk => new HashSet<string>());
						lock (types)
							types.Add(ServiceType.FullName);

						return CreateServiceFactorySet(
							ServiceResolver,
							ServiceType,
							ServiceDeclare,
							ServiceInstanceId
							);
					});
			return factorySet.GetFactory(InterfaceType);
		}

		public void NotifyChanged(string ServiceType, long Id)
		{
			ServiceInterfaceFactorySet v;
			
			FactoryMap.TryRemove(Id, out v);

			//清理泛型实例类的服务
			var types = GenericTypes.Get(ServiceType);
			if (types != null)
				lock (types)
				{
					types.ForEach(type =>
						FactoryMap.TryRemove(Id, out v)
						);
					types.Clear();
				}

		}

		public void NotifyDefaultChanged(string Type)
		{
			long id;
			if (DefaultServiceImplementMap.TryRemove(Type,out id))
				NotifyChanged(Type, id);
		}

	}

	class ServiceFactoryManager : IServiceFactoryManager,IServiceInstanceConfigChangedNotifier
	{
		
		Caching.ILocalCache<object> ServiceCache { get; }

		IServiceMetadata ServiceMetadata { get; }

		public ServiceFactoryManager(
			Caching.ILocalCache<object> ServiceCache,
			IServiceMetadata ServiceMetadata
			)
		{
			this.ServiceCache = ServiceCache;
			this.ServiceMetadata = ServiceMetadata;
		}
		AppServiceSet GetAppServiceSet(int AppId)
		{
			var key = AppId.ToString();
			var ass = (AppServiceSet)ServiceCache.Get(key);
			if (ass != null)
				return ass;
			ass = new AppServiceSet(AppId,ServiceMetadata);
			return (AppServiceSet)(ServiceCache.AddOrGetExisting(key, ass, TimeSpan.FromHours(2)) ?? ass);
		}

		
		public IServiceFactory GetServiceFactory(
			IServiceResolver ServiceResolver,
			int AppId,
			Type ServiceType, 
			long ServiceInstanceId,
			Type InterfaceType
			)
		{

			if (ServiceType == null)
				throw new ArgumentNullException();
			if (ServiceType.IsGenericTypeDefinition)
				throw new NotSupportedException();

			var appServiceSet = GetAppServiceSet(AppId);
			return appServiceSet.GetServiceFactory(ServiceResolver, ServiceType, ServiceInstanceId, InterfaceType);


		}


		void IServiceInstanceConfigChangedNotifier.NotifyChanged(string ServiceType,int AppId, long Id)
		{
			GetAppServiceSet(AppId).NotifyChanged(ServiceType, Id);
		}

		void IServiceInstanceConfigChangedNotifier.NotifyDefaultChanged(string Type, int AppId)
		{
			GetAppServiceSet(AppId).NotifyDefaultChanged(Type);
		}


	}

}
