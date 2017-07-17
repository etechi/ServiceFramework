using System;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
	//class ServiceInterfaceFactorySet
	//{
	//	static System.Collections.Concurrent.ConcurrentDictionary<(Type, Type), (ServiceCreator, System.Reflection.ConstructorInfo)> CreaterDict = new ConcurrentDictionary<(Type, Type), (ServiceCreator, System.Reflection.ConstructorInfo)>();
	//	static (ServiceCreator, System.Reflection.ConstructorInfo) GetServiceCreator(Type ServiceType, Type ImplementType, IServiceMetadata ServiceMetadata)
	//	{
	//		return CreaterDict.GetOrAdd((ImplementType, ServiceType), (ValueTuple<Type, Type> pair) =>
	//		{
	//			var it = pair.Item1;
	//			var st = pair.Item2;
	//			var ci = Internals.ServiceCreatorBuilder
	//					.FindBestConstructorInfo(it)
	//					.AssertNotNull(
	//						() => $"找不到服务实现类型{ImplementType}的构造函数"
	//						);

	//			var Creator = ServiceCreatorBuilder.Build(
	//				ServiceMetadata,
	//				st,
	//				it,
	//				ci
	//				);
	//			return (Creator, ci);
	//		}
	//		 );
	//	}

	//	public AppServiceSet AppServiceSet { get; }
	//	public int AppId => AppServiceSet.AppId;
	//	public IServiceMetadata ServiceMetadata => AppServiceSet.ServiceMetadata;

	//	public long Id { get; }
	//	public IServiceImplement Implement { get; }
	//	public Lazy<IServiceFactory>[] Factories { get; }
	//	public IServiceCreateParameterTemplate Template { get; }

	//	public IServiceConfig Config { get; }
	//	public ServiceInterfaceFactorySet(
	//		long Id,
	//		IServiceImplement Implement,
	//		IServiceConfig Config,
	//		AppServiceSet AppServiceSet
	//		)
	//	{
	//		this.AppServiceSet = AppServiceSet;
	//		this.Id = Id;
	//		this.Implement = Implement;
	//		this.Config = Config;
	//		Factories = Implement.Interfaces.Select(i =>
	//			new Lazy<IServiceFactory>(() => CreateInterfaceFactory(i))
	//			).ToArray();
	//	}
	//	IServiceFactory CreateInterfaceFactory(IServiceInterface ServiceInterface)
	//	{
	//		ServiceCreator Creator;
	//		IServiceCreateParameterTemplate CreateParameterTemplate = null;
	//		switch (ServiceInterface.ServiceImplementType)
	//		{
	//			case ServiceImplementType.Creator:
	//				var func = ServiceInterface.ImplementCreator;
	//				Creator = (sp, ctr) => func(sp);
	//				break;
	//			case ServiceImplementType.Instance:
	//				var ins = ServiceInterface.ImplementInstance;
	//				Creator = (sp, ctr) => ins;
	//				break;
	//			case ServiceImplementType.Type:

	//				var implType = ServiceInterface.ImplementType.IsGenericTypeDefinition ?
	//					ServiceInterface.ImplementType.MakeGenericType(Implement.ServiceType.GetGenericArguments()) :
	//					ServiceInterface.ImplementType;
	//				System.Reflection.ConstructorInfo ci;
	//				(Creator, ci) = GetServiceCreator(Implement.ServiceType, implType, ServiceMetadata);
	//				var setting = Config.Settings.Get(ServiceInterface.ImplementType.FullName);
	//				CreateParameterTemplate = ServiceCreateParameterTemplate.Load(
	//					ci,
	//					AppId,
	//					Id,
	//					(setting?.ImplementType == ServiceInterface.ImplementType.FullName ? setting?.Setting : null),
	//					ServiceMetadata
	//					);
	//				break;
	//			default:
	//				throw new NotSupportedException();
	//		}
	//		return null;
	//	}
	//	public IServiceFactory GetFactory(Type InterfaceType)
	//	{
	//		var count = Factories.Length;
	//		var iis = Implement.Interfaces;
	//		for (var i = 0; i < count; i++)
	//		{
	//			if (InterfaceType == iis[i].Type)
	//				return Factories[i].Value;
	//		}
	//		return null;
	//	}

	//}

	//class AppServiceSet
	//{
	//	public int AppId { get; }
	//	public IServiceMetadata ServiceMetadata { get; }
	//	public ConcurrentDictionary<string, HashSet<string>> GenericTypes { get; } = new ConcurrentDictionary<string, HashSet<string>>();
	//	public ConcurrentDictionary<string, long> DefaultServiceImplementMap { get; } = new ConcurrentDictionary<string, long>();
	//	public ConcurrentDictionary<long, ServiceInterfaceFactorySet> FactoryMap { get; } = new ConcurrentDictionary<long, ServiceInterfaceFactorySet>();
	//	public AppServiceSet(int AppId, IServiceMetadata ServiceMetadata)
	//	{
	//		this.AppId = AppId;
	//		this.ServiceMetadata = ServiceMetadata;
	//	}
	//	(IServiceImplement, IServiceConfig) LoadImplementById(
	//		IServiceResolver ServiceResolver,
	//		long Id
	//	)
	//	{
	//		var (ImplementType, cfg) =
	//			ServiceResolver.WithScope(sp =>
	//			{
	//				var ImplementTypeResolver = sp.Resolve<IServiceImplementTypeResolver>();
	//				var icfg = sp.Resolve<IServiceConfigLoader>()
	//					.GetConfig(ServiceType.FullName, AppId, Id)
	//					.Assert(
	//					v => v?.ServiceType == ServiceType.FullName,
	//					v => $"服务配置({Id})返回类型({v?.ServiceType})与实际所需类型({ServiceType})不符"
	//					);

	//				var implType = ImplementTypeResolver
	//				.Resolve(icfg.Settings[icfg.ServiceType].ImplementType)
	//				.AssertNotNull(
	//					() => $"找不到服务配置({Id})指定的服务实现类型({icfg.Settings[icfg.ServiceType].ImplementType}),服务:{ServiceType}"
	//					);
	//				return (implType, icfg);
	//			});

	//		var setting = cfg.Settings;

	//		var impl = ServiceMetadata.Services
	//			.Get(ServiceType)?.Implements
	//			.SingleOrDefault(i => i.Interfaces.Single(ii => ii.Type == ServiceType).ImplementType == ImplementType)
	//			.AssertNotNull(
	//				() => $"找不到服务配置({Id})指定的服务实现类型({ImplementType}),服务:{ServiceType}"
	//				);
	//		return (impl, cfg);
	//	}
	//	ServiceInterfaceFactorySet CreateServiceFactorySet(
	//		IServiceResolver ServiceResolver,
	//		Type ServiceType,
	//		IServiceDeclaration ServiceDeclaration,
	//		long Id
	//		)
	//	{
	//		IServiceImplement ServiceImplement;
	//		IServiceConfig cfg = null;
	//		if (Id == 0)
	//			ServiceImplement = ServiceDeclaration.Implements.Last();
	//		else if (Id < 0)
	//		{
	//			var idx = ServiceDeclaration.Implements.Count + Id;
	//			if (idx < 0 || idx >= ServiceDeclaration.Implements.Count)
	//				throw new IndexOutOfRangeException($"超出服务索引限制{ServiceType}");
	//			ServiceImplement = ServiceDeclaration.Implements[(int)idx];
	//		}
	//		else
	//			(ServiceImplement, cfg) = LoadImplementById(ServiceResolver, ServiceType, Id);

	//		return new ServiceInterfaceFactorySet(
	//			Id,
	//			ServiceImplement,
	//			cfg,
	//			this
	//			);
	//	}

	//	long ResolveDefaultService(IServiceProvider ServiceProvider, Type ServiceType, long Id)
	//	{
	//		if (Id != 0)
	//			return Id;

	//		if (DefaultServiceImplementMap.TryGetValue(ServiceType.FullName, out Id))
	//			return Id;

	//		if (ServiceType.IsDefined(typeof(UnmanagedServiceAttribute), true))
	//			Id = 0;
	//		else
	//		{
	//			var DefaultServiceLocator = ServiceProvider
	//				.TryResolve<IDefaultServiceLocator>();
	//			if (DefaultServiceLocator == null)
	//				Id = 0;
	//			else
	//			{
	//				//.AssertNotNull(
	//				//	() => $"找不到服务{typeof(IDefaultServiceLocator)}"
	//				//	);

	//				var LocateResult = DefaultServiceLocator.Locate(ServiceType.FullName, AppId);
	//				if (LocateResult != 0)
	//					Id = LocateResult;
	//				else
	//				{
	//					//ServiceMetadata.Services.Get(ServiceType)
	//					//	.Implements
	//					//	.Error(
	//					//		v => v.Count == 0,
	//					//		v => $"没有服务实现"
	//					//	)
	//					//	.Error(
	//					//		v => v.Count > 1,
	//					//		v => $"找不到默认服务"
	//					//		);
	//					Id = 0;
	//				}
	//			}
	//		}
	//		Id = DefaultServiceImplementMap.GetOrAdd(ServiceType.FullName, Id);
	//		return Id;
	//	}

	//	public IServiceFactory GetServiceFactory(
	//		IServiceResolver ServiceResolver,
	//		Type ServiceType,
	//		long ServiceInstanceId,
	//		Type InterfaceType
	//		)
	//	{

	//		if (ServiceType == null)
	//			throw new ArgumentNullException();
	//		if (ServiceType.IsGenericTypeDefinition)
	//			throw new NotSupportedException();

	//		var RealServiceInstanceId = ResolveDefaultService(
	//			ServiceResolver,
	//			ServiceType,
	//			ServiceInstanceId
	//			);

	//		ServiceInterfaceFactorySet factorySet;
	//		if (FactoryMap.TryGetValue(RealServiceInstanceId, out factorySet))
	//			return factorySet.GetFactory(InterfaceType);
	//		var ServiceDeclare = ServiceMetadata.Services.Get(ServiceType);
	//		if (ServiceDeclare != null)
	//		{
	//			return FactoryMap.GetOrAdd(
	//				RealServiceInstanceId,
	//				(k) => CreateServiceFactorySet(
	//					ServiceResolver,
	//					ServiceType,
	//					ServiceDeclare,
	//					RealServiceInstanceId
	//					)
	//				).GetFactory(InterfaceType);
	//		}


	//		if (!ServiceType.IsGenericType)
	//			return null;

	//		var TypeArgs = ServiceType.GetGenericArguments();
	//		var ServiceTypeDef = ServiceType.GetGenericTypeDefinition();

	//		ServiceDeclare = ServiceMetadata.Services.Get(ServiceTypeDef);
	//		if (ServiceDeclare == null)
	//			return null;

	//		if (!FactoryMap.TryGetValue(RealServiceInstanceId, out factorySet))
	//			factorySet = FactoryMap.GetOrAdd(
	//				RealServiceInstanceId,
	//				(k) => {
	//					var types = GenericTypes.GetOrAdd(ServiceTypeDef.FullName, kk => new HashSet<string>());
	//					lock (types)
	//						types.Add(ServiceType.FullName);

	//					return CreateServiceFactorySet(
	//						ServiceResolver,
	//						ServiceType,
	//						ServiceDeclare,
	//						ServiceInstanceId
	//						);
	//				});
	//		return factorySet.GetFactory(InterfaceType);
	//	}

	//	public void NotifyChanged(string ServiceType, long Id)
	//	{
	//		ServiceInterfaceFactorySet v;

	//		FactoryMap.TryRemove(Id, out v);

	//		//清理泛型实例类的服务
	//		var types = GenericTypes.Get(ServiceType);
	//		if (types != null)
	//			lock (types)
	//			{
	//				types.ForEach(type =>
	//					FactoryMap.TryRemove(Id, out v)
	//					);
	//				types.Clear();
	//			}

	//	}

	//	public void NotifyDefaultChanged(string Type)
	//	{
	//		long id;
	//		if (DefaultServiceImplementMap.TryRemove(Type, out id))
	//			NotifyChanged(Type, id);
	//	}
	//}

	//class Utils
	//{
	//	public static (IServiceImplement, IServiceConfig) LoadImplementById(
	//		IServiceProvider ServiceProvider,
	//		Type ServiceType,
	//		IServiceMetadata ServiceMetadata,
	//		long Id
	//		)
	//	{
	//		var (ImplementType, cfg) =
	//			ServiceProvider.WithScope(sp =>
	//			{
	//				var icfg = sp.Resolve<IServiceConfigLoader>()
	//					.GetConfig(Id)
	//					.Assert(
	//					v => v?.ServiceType == ServiceType.FullName,
	//					v => $"服务配置({Id})返回类型({v?.ServiceType})与实际所需类型({ServiceType})不符"
	//					);

	//				var implType = sp.Resolve<IServiceImplementTypeResolver>()
	//					.Resolve(icfg.ImplementType)
	//					.AssertNotNull(
	//						() => $"找不到服务配置({Id})指定的服务实现类型({icfg.ImplementType}),服务:{ServiceType}"
	//						);
	//				return (implType, icfg);
	//			});

	//		var setting = cfg.Setting;

	//		var impl = ServiceMetadata.Services
	//			.Get(ServiceType)?.Implements
	//			.SingleOrDefault(i => i.ImplementType == ImplementType)
	//			.AssertNotNull(
	//				() => $"找不到服务配置({Id})指定的服务实现类型({ImplementType}),服务:{ServiceType}"
	//				);
	//		return (impl, cfg);
	//	}
	//}
	
	class ServiceFactoryManager : IServiceFactoryManager,IServiceInstanceConfigChangedNotifier
	{
		abstract class ServiceEntry
		{
			public abstract IServiceFactory DefaultFactory { get; }
		}
		class ManagedServiceEntry : ServiceEntry,IServiceEntry
		{
			public IServiceFactory Factory;
			public IServiceConfig Config;
			public override IServiceFactory DefaultFactory => Factory;
			public ConcurrentDictionary<string, InternalServiceData> InternalServices;
		}
		class UnmanagedServiceEntry : ServiceEntry
		{
			public Lazy<IServiceFactory> Factory;
			public override IServiceFactory DefaultFactory => Factory.Value;
		}
		class InternalServiceData : Dictionary<string,long[]>
		{
			public long[] InternalServiceIds { get; }
			public InternalServiceData(ServiceReference[] srs)
			{
				InternalServiceIds = srs.Select(i => i.Id).ToArray();
				foreach (var g in srs.Where(i => i.Name != null).GroupBy(s => s.Name))
					Add(g.Key, g.Select(i => i.Id).ToArray());
			}
		}
		ConcurrentDictionary<(Type, Type), (ServiceCreator, ConstructorInfo)> ServiceCreatorDict { get; }
			= new ConcurrentDictionary<(Type, Type), (ServiceCreator, ConstructorInfo)>();


		Caching.ILocalCache<IServiceEntry> _ManagedServiceCache;
		ConcurrentDictionary<string, InternalServiceData> TopScopeServices = new ConcurrentDictionary<string, InternalServiceData>();

		ConcurrentDictionary<Type, UnmanagedServiceEntry[]> UnmanagedServiceCache { get; } = new ConcurrentDictionary<Type, UnmanagedServiceEntry[]>();

		public IServiceMetadata ServiceMetadata { get; }
		public IServiceDeclarationTypeResolver ServiceDeclarationTypeResolver { get; }
		public IServiceImplementTypeResolver ServiceImplementTypeResolver { get; }

		public ServiceFactoryManager(
			Caching.ILocalCache<IServiceEntry> ServiceCache,
			IServiceMetadata ServiceMetadata,
			IServiceDeclarationTypeResolver ServiceDeclarationTypeResolver,
			 IServiceImplementTypeResolver ServiceImplementTypeResolver
			)
		{
			_ManagedServiceCache = ServiceCache;
			this.ServiceMetadata = ServiceMetadata;
			this.ServiceDeclarationTypeResolver = ServiceDeclarationTypeResolver;
			this.ServiceImplementTypeResolver = ServiceImplementTypeResolver;
		}

		Caching.ILocalCache<IServiceEntry> ManagedServiceCache(IServiceProvider ServiceProvider)
		{
			if (_ManagedServiceCache != null)
				return _ManagedServiceCache;
			lock (this)
			{
				if (_ManagedServiceCache != null)
					return _ManagedServiceCache;

				var cacheType = typeof(Caching.ILocalCache<IServiceEntry>);
				var entry = GetUnmanagedServiceEntry(cacheType, true);
				var factory = entry.DefaultFactory;
				var cache = (Caching.ILocalCache<IServiceEntry>)factory.Create(ServiceProvider);
				return _ManagedServiceCache = cache;
			}
		}
		ManagedServiceEntry GetManagedServiceEntry(IServiceProvider ServiceProvider, long ServiceId, bool CreateIfNotExists)
		{
			var key = ServiceId.ToString();
			var sc = ManagedServiceCache(ServiceProvider);
			var se = sc?.Get(key);
			if (se != null || !CreateIfNotExists)
				return (ManagedServiceEntry)se;
			se = new ManagedServiceEntry();
			return (ManagedServiceEntry)(sc.AddOrGetExisting(key, se, TimeSpan.FromHours(2)) ?? se);
			
		}

		IServiceFactory CreateUnmanagedServiceFactory(
			IServiceDeclaration decl,
			IServiceImplement impl,
			Type ServiceType,
			int Index
			)
		{
			ServiceCreator Creator;
			IServiceCreateParameterTemplate CreateParameterTemplate = null;
			switch (impl.ServiceImplementType)
			{
				case ServiceImplementType.Creator:
					var func = impl.ImplementCreator;
					Creator = (sp, si, ctr) => func(sp);
					break;
				case ServiceImplementType.Instance:
					var ins = impl.ImplementInstance;
					Creator = (sp, si, ctr) => ins;
					break;
				case ServiceImplementType.Type:
					{
						(Creator, CreateParameterTemplate) = CreateServiceInstanceCreator(
							ServiceType,
							impl.ImplementType,
							null
							);
						break;
					}
				default:
					throw new NotSupportedException();
			}

			return new ServiceFactory(
				-1- Index,
				null,
				decl,
				impl,
				CreateParameterTemplate,
				Creator
				);
		}
		UnmanagedServiceEntry[] GetUnmanagedServiceEntries(Type ServiceType, bool CreateIfNotExists)
		{
			if (UnmanagedServiceCache.TryGetValue(
				ServiceType,
				out var se
				) || !CreateIfNotExists)
				return se;
			var decl = GetServiceDeclaration(ServiceType);

			return UnmanagedServiceCache.GetOrAdd(
				ServiceType,
				decl.Implements.Reverse().Select((impl,idx) => new UnmanagedServiceEntry()
				{
					Factory = new Lazy<IServiceFactory>(() => CreateUnmanagedServiceFactory(decl,impl,ServiceType,idx))
				}).ToArray()
				);
		}
		UnmanagedServiceEntry GetUnmanagedServiceEntry(Type ServiceType, bool CreateIfNotExists)
		{
			var re = GetUnmanagedServiceEntries(ServiceType, CreateIfNotExists);
			if(re==null || re.Length==0) return null;
			return re[0];
		}
		//ServiceEntry GetServiceEntry(IServiceProvider ServiceProvider, Type ServiceType, long ServiceId,bool CreateIfNotExists)
		//{
		//	if (ServiceType == null && ServiceId > 0)
		//		return GetManagedServiceEntry(ServiceProvider, ServiceId, CreateIfNotExists);
		//	else
		//		return GetUnmanagedServiceEntry(ServiceProvider, ServiceType, (int)ServiceId, CreateIfNotExists);
		//}

		ConcurrentDictionary<string, InternalServiceData> EnsureDefaultServiceDict(ManagedServiceEntry se)
		{
			var dss = se.InternalServices;
			if (dss == null)
				dss = se.InternalServices = new ConcurrentDictionary<string, InternalServiceData>();
			return dss;
		}

		IServiceConfig EnsureManagedConfig(
			IServiceConfigLoader ConfigLoader,
			ManagedServiceEntry se,
			long Id
			)
		{
			var cfg = se.Config;
			if (cfg != null) return cfg;
			return se.Config = cfg = ConfigLoader.GetConfig(Id);
		}

		(ServiceCreator,ConstructorInfo) GetServiceCreator(Type ServiceType, Type ImplementType)
			=> ServiceCreatorDict.GetOrAdd((ServiceType, ImplementType), key =>
			 {
				 var st = key.Item1;
				 var it = key.Item2;
				 var ci = ServiceCreatorBuilder
					 .FindBestConstructorInfo(it)
					 .AssertNotNull(
						 () => $"找不到服务实现类型{it}的构造函数"
						 );

				 var creator = ServiceCreatorBuilder.Build(
					 ServiceMetadata,
					 st,
					 it,
					 ci
					 );
				return (creator, ci);
			 });

		(ServiceCreator,IServiceCreateParameterTemplate) CreateServiceInstanceCreator(
			Type ServiceType,
			Type ImplementType,
			string Setting
			)
		{
			var realImplType = ImplementType.IsGenericTypeDefinition ?
							ImplementType.MakeGenericType(ServiceType.GetGenericArguments()) :
							ImplementType;
			
			var (Creator,ConstructorInfo) = GetServiceCreator(ServiceType, realImplType);
			var CreateParameterTemplate = ServiceCreateParameterTemplate.Load(
				ConstructorInfo,
				Setting,
				ServiceMetadata
				);
			return (Creator, CreateParameterTemplate);
		}

		IServiceDeclaration GetServiceDeclaration(Type ServiceType)
		{
			return (ServiceMetadata.Services
				.Get(ServiceType) ??
				(ServiceType.IsGenericType ?
				ServiceMetadata.Services
				.Get(ServiceType.GetGenericTypeDefinition()) : null))
				.AssertNotNull(
					() => $"找不到服务描述({ServiceType})"
					); ;
		}

		
		IServiceFactory GetManagedServiceFactoryByIdent(
			IServiceProvider ServiceProvider, 
			ManagedServiceEntry Entry,
			long ServiceId,
			Type ServiceType
			)
		{
			if (Entry.Factory != null)
				return Entry.Factory;

			var cfg = (Entry.Config ?? ServiceProvider.WithScope(sp =>
				   EnsureManagedConfig(sp.Resolve<IServiceConfigLoader>(), Entry, ServiceId)
				   ))
				   .AssertNotNull(
					() => $"找不到服务实例({ServiceId})的配置数据，服务类型:{ServiceType}"
					);


			var declType = ServiceDeclarationTypeResolver
				.Resolve(cfg.ServiceType)
				.AssertNotNull(
					() => $"找不到服务类型({cfg.ServiceType}),服务ID:{cfg.Id}"
					)
				.Assert(
					type => type == ServiceType,
					type => $"服务实例({cfg.Id})的服务类型({type})和指定服务类型({ServiceType})不一致"
					);

			
			var decl = ServiceMetadata.Services
				.Get(declType)
				.AssertNotNull(
					() => $"找不到服务描述({declType}),服务:{cfg.Id}"
					);

			var implType = ServiceImplementTypeResolver
				.Resolve(cfg.ImplementType)
				.AssertNotNull(
					() => $"找不到服务配置({ServiceId})指定的服务实现类型({cfg.ImplementType}),服务:{declType}"
					);

			var impl = decl.Implements
				.Last(i =>i.ServiceImplementType==ServiceImplementType.Type && i.ImplementType == implType)
				.AssertNotNull(
					() => $"找不到服务配置({ServiceId})指定的服务实现类型({implType}),服务:{declType}"
					);

			var (Creator, CreateParameterTemplate) = CreateServiceInstanceCreator(
				ServiceType,
				impl.ImplementType,
				cfg.Setting
				);

			return Entry.Factory = new ServiceFactory(
				cfg.Id,
				cfg.ParentId,
				decl,
				impl,
				CreateParameterTemplate,
				Creator
				);
		}
		public IServiceFactory GetServiceFactoryByIdent(
			IServiceProvider ServiceProvider,
			long ServiceId,
			Type ServiceType
			)
		{
			var curEntry = GetManagedServiceEntry(ServiceProvider, ServiceId, true);
			return GetManagedServiceFactoryByIdent(ServiceProvider,curEntry, ServiceId, ServiceType);
		}


		InternalServiceData TryGetManagedScopedInternalServiceData(
			IServiceProvider ServiceProvider,
			ConcurrentDictionary<string, InternalServiceData> dss,
			long? ScopeServiceId,
			string ServiceType,
			ref IServiceScope scope,
			ref IServiceInstanceLister serviceInstanceLister
			)
		{
			if (dss.TryGetValue(ServiceType, out var cids))
				return cids;

			if (serviceInstanceLister == null)
			{
				if (scope == null)
					scope = ServiceProvider.Resolve<IServiceScopeFactory>().CreateServiceScope();
				serviceInstanceLister = scope.ServiceProvider.Resolve<IServiceInstanceLister>();
			}
			var re = serviceInstanceLister.List(ScopeServiceId, ServiceType, 100);
			cids = new InternalServiceData(re);
			return dss.GetOrAdd(ServiceType, cids);
		}

		//long[] TryGetManagedScopedServiceIdents(
		//	IServiceProvider ServiceProvider,
		//	ConcurrentDictionary<string,InternalServiceData> dss,
		//	long? ScopeServiceId,
		//	string ServiceType,
		//	ref IServiceScope scope,
		//	ref IServiceInstanceLister serviceInstanceLister
		//	)
		//{
		//	return TryGetManagedScopedInternalServiceData(
		//		ServiceProvider,
		//		dss,
		//		ScopeServiceId,
		//		ServiceType,
		//		ref scope,
		//		ref serviceInstanceLister
		//		)?.InternalServiceIds;
		//}

		InternalServiceData GetManagedScopedServiceData(
			IServiceProvider ServiceProvider,
			long? ScopeServiceId,
			Type ServiceType
			)
		{
			IServiceScope scope = null;
			IServiceInstanceLister serviceInstanceLister = null;
			IServiceConfigLoader configLoader = null;

			try
			{
				while(ScopeServiceId.HasValue)
				{
					var curEntry = GetManagedServiceEntry(ServiceProvider, ScopeServiceId.Value, true);
					var dss = EnsureDefaultServiceDict(curEntry);
					var cids = TryGetManagedScopedInternalServiceData(
						ServiceProvider,
						dss,
						ScopeServiceId.Value,
						ServiceType.FullName,
						ref scope,
						ref serviceInstanceLister
						);

					//找到服务
					if (cids.InternalServiceIds.Length > 0)
						return cids;

					//当前区域不是顶级区域，继续向上层搜索
						
					if (configLoader == null)
					{
						if (scope == null)
							scope = ServiceProvider.Resolve<IServiceScopeFactory>().CreateServiceScope();
						configLoader = scope.ServiceProvider.Resolve<IServiceConfigLoader>();
					}
					var cfg = EnsureManagedConfig(configLoader, curEntry, ScopeServiceId.Value);
					ScopeServiceId = cfg.ParentId;
				}
				return TryGetManagedScopedInternalServiceData(
					ServiceProvider,
					TopScopeServices,
					null,
					ServiceType.FullName,
					ref scope,
					ref serviceInstanceLister
					);
			}
			finally
			{
				if (scope != null)
					scope.Dispose();
			}

		}

		static Type UnmanagedServiceAttributeType { get; } = typeof(UnmanagedServiceAttribute);
		[ThreadStatic]
		static bool IsUnmanagedServiceScope;

		public IServiceFactory GetServiceFactoryByType(
			IServiceProvider ServiceProvider,
			long? ScopeServiceId,
			Type ServiceType,
			string Name
			)
		{
			var IsUnmanagedServiceScopeRoot=false;
			try
			{
				if (!IsUnmanagedServiceScope && ServiceType.IsDefined(UnmanagedServiceAttributeType))
				{
					IsUnmanagedServiceScopeRoot = true;
					IsUnmanagedServiceScope = true;
				}

				if (!IsUnmanagedServiceScope && 
					(!ServiceType.IsGenericType || ServiceType.GetGenericTypeDefinition() != typeof(IEnumerable<>))
					)
				{
					var isd = GetManagedScopedServiceData(ServiceProvider, ScopeServiceId, ServiceType);
					if (Name != null)
					{
						if (!isd.TryGetValue(Name, out var ids) || ids.Length == 0)
							return null;
						return GetManagedServiceFactoryByIdent(
							ServiceProvider,
							GetManagedServiceEntry(ServiceProvider, ids[0], true),
							ids[0],
							ServiceType
							);
					}
					else if ((isd?.InternalServiceIds?.Length ?? 0) > 0)
						return GetManagedServiceFactoryByIdent(
							ServiceProvider,
							GetManagedServiceEntry(ServiceProvider, isd.InternalServiceIds[0], true),
							isd.InternalServiceIds[0],
							ServiceType
							);
				}
				//已经是顶级区域， 直接尝试查找服务
				return GetUnmanagedServiceEntry(ServiceType, true)?.DefaultFactory;
			}
			finally
			{
				if (IsUnmanagedServiceScopeRoot)
					IsUnmanagedServiceScope = false;
			}
		}
		public IEnumerable<IServiceFactory> GetServiceFactoriesByType(
			IServiceProvider ServiceProvider,
			long? ScopeServiceId,
			Type ServiceType,
			string Name
			)
		{
			if (!ServiceType.IsDefined(UnmanagedServiceAttributeType))
			{
				var isd = GetManagedScopedServiceData(ServiceProvider, ScopeServiceId, ServiceType);
				if(Name!=null)
				{
					if (!isd.TryGetValue(Name, out var ids) || ids.Length == 0)
						yield break;
					foreach (var id in ids)
						yield return GetManagedServiceFactoryByIdent(
							ServiceProvider,
							GetManagedServiceEntry(ServiceProvider, id, true),
							id,
							ServiceType
							);
					yield break;
				}
				else if ((isd?.InternalServiceIds?.Length ?? 0) > 0)
				{
					foreach (var id in isd.InternalServiceIds)
						yield return GetManagedServiceFactoryByIdent(
							ServiceProvider,
							GetManagedServiceEntry(ServiceProvider, id, true),
							id,
							ServiceType
							);
					yield break;
				}
			}
			
			//已经是顶级区域， 直接尝试查找服务
			var es = GetUnmanagedServiceEntries(ServiceType, true);
			if (es != null)
				foreach (var e in es)
					yield return e.DefaultFactory;
			

		}

		void IServiceInstanceConfigChangedNotifier.NotifyChanged( long Id)
		{
			var se = GetManagedServiceEntry(null, Id, false);
			if (se == null)
				return;
			_ManagedServiceCache?.Remove(Id.ToString());
		}

		void IServiceInstanceConfigChangedNotifier.NotifyInternalServiceChanged(long? ScopeId, string ServiceType)
		{
			if (ScopeId == null)
				TopScopeServices.TryRemove(ServiceType, out var ids);
			else
				GetManagedServiceEntry(null, ScopeId.Value, false)
					?.InternalServices
					?.TryRemove(ServiceType,out var id);
		}

	}

}
