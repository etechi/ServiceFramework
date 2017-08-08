using System;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SF.Core.ServiceManagement.Internals
{
	
	
	class UnmanagedServiceFactoryManager 
	{
		
		public class UnmanagedServiceEntry
		{
			public Lazy<IServiceFactory> Factory;
		}

		//服务实例创建缓存
		ServiceCreatorCache ServiceCreatorCache { get; }

		//不可配置服务缓存
		ConcurrentDictionary<Type, UnmanagedServiceEntry[]> UnmanagedServiceCache { get; } = new ConcurrentDictionary<Type, UnmanagedServiceEntry[]>();

		public IServiceMetadata ServiceMetadata { get; }

		public UnmanagedServiceFactoryManager(
			IServiceMetadata ServiceMetadata,
			ServiceCreatorCache ServiceCreatorCache
			)
		{
			this.ServiceMetadata = ServiceMetadata;
			this.ServiceCreatorCache = ServiceCreatorCache;
		}

		
		public UnmanagedServiceEntry[] GetUnmanagedServiceEntries(Type ServiceType, bool CreateIfNotExists)
		{
			if (UnmanagedServiceCache.TryGetValue(
				ServiceType,
				out var se
				) || !CreateIfNotExists)
				return se;
			var decl= ServiceMetadata.FindServiceByType(ServiceType);
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
		public UnmanagedServiceEntry GetUnmanagedServiceEntry(Type ServiceType, bool CreateIfNotExists)
		{
			var re = GetUnmanagedServiceEntries(ServiceType, CreateIfNotExists);
			if(re==null || re.Length==0) return null;
			return re[0];
		}
	}

}
