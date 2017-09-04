using System;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SF.Core.ServiceManagement.Internals
{
	
	
	class ManagedServiceFactoryManager 
	{
		class ManagedServiceEntry : IServiceEntry
		{
			public ConcurrentDictionary<Type, IServiceFactory> Factories { get; } = new ConcurrentDictionary<Type, IServiceFactory>();
			public IServiceConfig Config;
			public ConcurrentDictionary<string, InternalServiceData> InternalServices;
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

		//����ʵ����������
		ServiceCreatorCache ServiceCreatorCache { get; }

		//�����÷��񻺴�
		Caching.ILocalCache<IServiceEntry> _ManagedServiceCache;

		//���������÷���ӳ��
		ConcurrentDictionary<string, InternalServiceData> TopScopeServices = new ConcurrentDictionary<string, InternalServiceData>();


		public IServiceMetadata ServiceMetadata { get; }
		Func<IServiceResolver,Caching.ILocalCache<IServiceEntry>> LocalCacheCreator { get; }

		public ManagedServiceFactoryManager(
			Caching.ILocalCache<IServiceEntry> ServiceCache,
			IServiceMetadata ServiceMetadata,
			ServiceCreatorCache ServiceCreatorCache,
			Func<IServiceResolver,Caching.ILocalCache<IServiceEntry>> LocalCacheCreator
			)
		{
			_ManagedServiceCache = ServiceCache;
			this.ServiceMetadata = ServiceMetadata;
			this.ServiceCreatorCache = ServiceCreatorCache;
			this.LocalCacheCreator = LocalCacheCreator;
		}

		Caching.ILocalCache<IServiceEntry> ManagedServiceCache(IServiceResolver ServiceResolver)
		{
			if (_ManagedServiceCache != null)
				return _ManagedServiceCache;
			lock (this)
			{
				if (_ManagedServiceCache != null)
					return _ManagedServiceCache;

				//var cacheType = typeof(Caching.ILocalCache<IServiceEntry>);
				//var entry = GetUnmanagedServiceEntry(cacheType, true);
				//var factory = entry.Factory.Value;
				//var cache = (Caching.ILocalCache<IServiceEntry>)factory.Create(ServiceResolver);
				var cache = LocalCacheCreator(ServiceResolver);
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
			if (Entry.Factories.TryGetValue(ServiceType, out var factory))
				return factory;

			var cfg = (Entry.Config ?? ServiceResolver.Provider.WithScope(sp =>
				   EnsureManagedConfig(sp.Resolve<IServiceConfigLoader>(), Entry, ServiceId)
				   ))
				   .AssertNotNull(
					() => $"�Ҳ�������ʵ��({ServiceId})���������ݣ���������:{ServiceType}"
					);

			var (decl, impl) = ServiceFactory.ResolveMetadata(
				ServiceResolver,
				ServiceId,
				cfg.ServiceType,
				cfg.ImplementType,
				ServiceType
				);

			factory =  ServiceFactory.Create(
				cfg.Id,
				cfg.ParentId,
				decl,
				impl,
				decl.ServiceType,
				ServiceCreatorCache,
				ServiceMetadata,
				cfg.Setting
				);
			return Entry.Factories.GetOrAdd(ServiceType, factory);
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
						ServiceType.GetFullName(),
						ref scope,
						ref serviceInstanceLister
						);

					//�ҵ�����
					if (cids.InternalServiceIds.Length > 0)
						return cids;

					//��ǰ�����Ƕ������򣬼������ϲ�����
						
					if (configLoader == null)
					{
						if (scope == null)
							scope = ServiceResolver.Resolve<IServiceScopeFactory>().CreateServiceScope();
						configLoader = scope.ServiceProvider.Resolve<IServiceConfigLoader>();
					}
					var cfg = EnsureManagedConfig(configLoader, curEntry, ScopeServiceId.Value);
					ScopeServiceId = cfg?.ParentId;
				}
				return TryGetManagedScopedInternalServiceData(
					ServiceResolver,
					TopScopeServices,
					null,
					ServiceType.GetFullName(),
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

		

		public bool TryGetServiceFactoryByType(
			IServiceResolver ServiceResolver,
			long? ScopeServiceId,
			Type ServiceType,
			string Name,
			out IServiceFactory factory
			)
		{
			factory = null;

				var isd = GetManagedScopedServiceData(ServiceResolver, ScopeServiceId, ServiceType);
			if (Name != null)
			{
				if (!isd.TryGetValue(Name, out var ids) || ids.Length == 0)
					return true;
				factory = GetManagedServiceFactoryByIdent(
					ServiceResolver,
					GetManagedServiceEntry(ServiceResolver, ids[0], true),
					ids[0],
					ServiceType
					);
				return true;
			}
			else if ((isd?.InternalServiceIds?.Length ?? 0) > 0)
			{
				factory = GetManagedServiceFactoryByIdent(
						ServiceResolver,
						GetManagedServiceEntry(ServiceResolver, isd.InternalServiceIds[0], true),
						isd.InternalServiceIds[0],
						ServiceType
						);
				return true;
			}
			return false;
		}

		
		public bool TryGetServiceFactoriesByType(
			IServiceResolver ServiceResolver,
			long? ScopeServiceId,
			Type ServiceType,
			string Name,
			out IEnumerable<IServiceFactory> enumerable
			)
		{
			var isd = GetManagedScopedServiceData(ServiceResolver, ScopeServiceId, ServiceType);
			if (Name != null)
			{
				if (!isd.TryGetValue(Name, out var ids) || ids.Length == 0)
				{
					enumerable = Enumerable.Empty<IServiceFactory>();
					return true;
				}
				enumerable = ids.Select(id => GetManagedServiceFactoryByIdent(
					ServiceResolver,
					GetManagedServiceEntry(ServiceResolver, id, true),
					id,
					ServiceType
					));
				return true;
			}
			else if ((isd?.InternalServiceIds?.Length ?? 0) > 0)
			{
				enumerable = isd.InternalServiceIds.Select(id =>
					  GetManagedServiceFactoryByIdent(
						  ServiceResolver,
						  GetManagedServiceEntry(ServiceResolver, id, true),
						  id,
						  ServiceType
						  )
					);
				return true;
			}
			else
			{
				enumerable = null;
				return false;
			}
		}

		public void NotifyChanged( long Id)
		{
			var se = GetManagedServiceEntry(null, Id, false);
			if (se == null)
				return;
			_ManagedServiceCache?.Remove(Id.ToString());
		}

		public void NotifyInternalServiceChanged(long? ScopeId, string ServiceType)
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