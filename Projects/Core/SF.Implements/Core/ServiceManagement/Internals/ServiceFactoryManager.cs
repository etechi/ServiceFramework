using System;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SF.Core.ServiceManagement.Internals
{
	
	
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

		//服务实例创建缓存
		ServiceCreatorCache ServiceCreatorCache { get; }

		//可配置服务缓存
		Caching.ILocalCache<IServiceEntry> _ManagedServiceCache;

		//顶级可配置服务映射
		ConcurrentDictionary<string, InternalServiceData> TopScopeServices = new ConcurrentDictionary<string, InternalServiceData>();

		//不可配置服务缓存
		ConcurrentDictionary<Type, UnmanagedServiceEntry[]> UnmanagedServiceCache { get; } = new ConcurrentDictionary<Type, UnmanagedServiceEntry[]>();

		public IServiceMetadata ServiceMetadata { get; }

		public ServiceFactoryManager(
			Caching.ILocalCache<IServiceEntry> ServiceCache,
			IServiceMetadata ServiceMetadata
			)
		{
			_ManagedServiceCache = ServiceCache;
			this.ServiceMetadata = ServiceMetadata;
			this.ServiceCreatorCache = new ServiceCreatorCache(ServiceMetadata);
		}

		Caching.ILocalCache<IServiceEntry> ManagedServiceCache(IServiceResolver ServiceResolver)
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
				var cache = (Caching.ILocalCache<IServiceEntry>)factory.Create(ServiceResolver);
				return _ManagedServiceCache = cache;
			}
		}
		ManagedServiceEntry GetManagedServiceEntry(IServiceResolver ServiceResolver, long ServiceId, bool CreateIfNotExists)
		{
			var key = ServiceId.ToString();
			var sc = ManagedServiceCache(ServiceResolver);
			var se = sc?.Get(key);
			if (se != null || !CreateIfNotExists)
				return (ManagedServiceEntry)se;
			se = new ManagedServiceEntry();
			return (ManagedServiceEntry)(sc.AddOrGetExisting(key, se, TimeSpan.FromHours(2)) ?? se);
			
		}


		IServiceDeclaration ResolveServiceImplements(Type ServiceType)
		{
			var decl = ServiceMetadata.Services.Get(ServiceType);
			if (decl != null)
				return decl;
			if (!ServiceType.IsGenericType)
				return null;

			var typeDef = ServiceType.GetGenericTypeDefinition();
			decl=ServiceMetadata.Services.Get(typeDef);
			if (decl != null)
				return decl;

			return null;
		}
		UnmanagedServiceEntry[] GetUnmanagedServiceEntries(Type ServiceType, bool CreateIfNotExists)
		{
			if (UnmanagedServiceCache.TryGetValue(
				ServiceType,
				out var se
				) || !CreateIfNotExists)
				return se;
			var decl= ResolveServiceImplements(ServiceType);
			if (decl == null)
				return Array.Empty<UnmanagedServiceEntry>();
			return UnmanagedServiceCache.GetOrAdd(
				ServiceType,
				decl.Implements.Reverse().Select((impl,idx) => new UnmanagedServiceEntry()
				{
					Factory = new Lazy<IServiceFactory>(() =>
						ServiceFactory.Create(
							-1 - idx,
							null,
							decl,
							impl,
							ServiceType,
							ServiceCreatorCache,
							ServiceMetadata,
							null
							)
						)
				}).ToArray()
				);
		}
		UnmanagedServiceEntry GetUnmanagedServiceEntry(Type ServiceType, bool CreateIfNotExists)
		{
			var re = GetUnmanagedServiceEntries(ServiceType, CreateIfNotExists);
			if(re==null || re.Length==0) return null;
			return re[0];
		}
		//ServiceEntry GetServiceEntry(IServiceResolver ServiceResolver, Type ServiceType, long ServiceId,bool CreateIfNotExists)
		//{
		//	if (ServiceType == null && ServiceId > 0)
		//		return GetManagedServiceEntry(ServiceResolver, ServiceId, CreateIfNotExists);
		//	else
		//		return GetUnmanagedServiceEntry(ServiceResolver, ServiceType, (int)ServiceId, CreateIfNotExists);
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

	
		
		IServiceFactory GetManagedServiceFactoryByIdent(
			IServiceResolver ServiceResolver, 
			ManagedServiceEntry Entry,
			long ServiceId,
			Type ServiceType
			)
		{
			if (Entry.Factory != null)
				return Entry.Factory;

			var cfg = (Entry.Config ?? ServiceResolver.Provider.WithScope(sp =>
				   EnsureManagedConfig(sp.Resolve<IServiceConfigLoader>(), Entry, ServiceId)
				   ))
				   .AssertNotNull(
					() => $"找不到服务实例({ServiceId})的配置数据，服务类型:{ServiceType}"
					);

			var (decl, impl) = ServiceFactory.ResolveMetadata(
				ServiceResolver,
				ServiceId,
				cfg.ServiceType,
				cfg.ImplementType,
				ServiceType
				);

			return Entry.Factory =  ServiceFactory.Create(
				cfg.Id,
				cfg.ParentId,
				decl,
				impl,
				ServiceType,
				ServiceCreatorCache,
				ServiceMetadata,
				cfg.Setting
				);
		}
		public IServiceFactory GetServiceFactoryByIdent(
			IServiceResolver ServiceResolver,
			long ServiceId,
			Type ServiceType
			)
		{
			var curEntry = GetManagedServiceEntry(ServiceResolver, ServiceId, true);
			return GetManagedServiceFactoryByIdent(ServiceResolver,curEntry, ServiceId, ServiceType);
		}


		InternalServiceData TryGetManagedScopedInternalServiceData(
			IServiceResolver ServiceResolver,
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
					scope = ((IServiceScopeFactory)ServiceResolver.ResolveServiceByType(null,typeof(IServiceScopeFactory),null)).CreateServiceScope();
				serviceInstanceLister = scope.ServiceProvider.Resolve<IServiceInstanceLister>();
			}
			var re = serviceInstanceLister.List(ScopeServiceId, ServiceType, 100);
			cids = new InternalServiceData(re);
			return dss.GetOrAdd(ServiceType, cids);
		}

		
		InternalServiceData GetManagedScopedServiceData(
			IServiceResolver ServiceResolver,
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
					var curEntry = GetManagedServiceEntry(ServiceResolver, ScopeServiceId.Value, true);
					var dss = EnsureDefaultServiceDict(curEntry);
					var cids = TryGetManagedScopedInternalServiceData(
						ServiceResolver,
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
							scope = ServiceResolver.Resolve<IServiceScopeFactory>().CreateServiceScope();
						configLoader = scope.ServiceProvider.Resolve<IServiceConfigLoader>();
					}
					var cfg = EnsureManagedConfig(configLoader, curEntry, ScopeServiceId.Value);
					ScopeServiceId = cfg.ParentId;
				}
				return TryGetManagedScopedInternalServiceData(
					ServiceResolver,
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
			IServiceResolver ServiceResolver,
			long? ScopeServiceId,
			Type ServiceType,
			string Name
			)
		{
			var IsUnmanagedServiceScopeRoot=false;
			try
			{
				if (!IsUnmanagedServiceScope && (ServiceType.IsDefined(UnmanagedServiceAttributeType) || !ServiceType.IsInterfaceType()))
				{
					IsUnmanagedServiceScopeRoot = true;
					IsUnmanagedServiceScope = true;
				}

				if (!IsUnmanagedServiceScope && 
					(!ServiceType.IsGenericType || ServiceType.GetGenericTypeDefinition() != typeof(IEnumerable<>))
					)
				{
					var isd = GetManagedScopedServiceData(ServiceResolver, ScopeServiceId, ServiceType);
					if (Name != null)
					{
						if (!isd.TryGetValue(Name, out var ids) || ids.Length == 0)
							return null;
						return GetManagedServiceFactoryByIdent(
							ServiceResolver,
							GetManagedServiceEntry(ServiceResolver, ids[0], true),
							ids[0],
							ServiceType
							);
					}
					else if ((isd?.InternalServiceIds?.Length ?? 0) > 0)
						return GetManagedServiceFactoryByIdent(
							ServiceResolver,
							GetManagedServiceEntry(ServiceResolver, isd.InternalServiceIds[0], true),
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
			IServiceResolver ServiceResolver,
			long? ScopeServiceId,
			Type ServiceType,
			string Name
			)
		{
			if (!ServiceType.IsDefined(UnmanagedServiceAttributeType))
			{
				var isd = GetManagedScopedServiceData(ServiceResolver, ScopeServiceId, ServiceType);
				if(Name!=null)
				{
					if (!isd.TryGetValue(Name, out var ids) || ids.Length == 0)
						yield break;
					foreach (var id in ids)
						yield return GetManagedServiceFactoryByIdent(
							ServiceResolver,
							GetManagedServiceEntry(ServiceResolver, id, true),
							id,
							ServiceType
							);
					yield break;
				}
				else if ((isd?.InternalServiceIds?.Length ?? 0) > 0)
				{
					foreach (var id in isd.InternalServiceIds)
						yield return GetManagedServiceFactoryByIdent(
							ServiceResolver,
							GetManagedServiceEntry(ServiceResolver, id, true),
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
