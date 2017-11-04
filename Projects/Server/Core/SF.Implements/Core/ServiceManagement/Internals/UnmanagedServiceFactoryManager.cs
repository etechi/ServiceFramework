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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SF.Core.ServiceManagement.Internals
{
	
	
	class UnmanagedServiceFactoryManager 
	{
		class UnmanagedServiceEntry
		{
			public Dictionary<string, int> NameMap;
			public Lazy<IServiceFactory>[] Factories;
		}

		//服务实例创建缓存
		ServiceCreatorCache ServiceCreatorCache { get; }

		//不可配置服务缓存
		ConcurrentDictionary<Type, UnmanagedServiceEntry> UnmanagedServiceCache { get; } = new ConcurrentDictionary<Type, UnmanagedServiceEntry>();

		public IServiceMetadata ServiceMetadata { get; }

		public UnmanagedServiceFactoryManager(
			IServiceMetadata ServiceMetadata,
			ServiceCreatorCache ServiceCreatorCache
			)
		{
			this.ServiceMetadata = ServiceMetadata;
			this.ServiceCreatorCache = ServiceCreatorCache;
		}

		UnmanagedServiceEntry GetUnmanagedServiceEntry(Type ServiceType, bool CreateIfNotExists)
		{
			if (UnmanagedServiceCache.TryGetValue(
				ServiceType,
				out var se
				) || !CreateIfNotExists)
				return se;
			var decl = ServiceMetadata.FindServiceByType(ServiceType);
			if (decl == null)
				return null;

			return UnmanagedServiceCache.GetOrAdd(
				ServiceType,
				(t) =>
				{
					var impls = decl.Implements.Reverse().Select((impl, idx) => (idx, impl)).ToArray();
					return new UnmanagedServiceEntry()
					{
						NameMap = impls.Where(i => i.impl.Name != null).ToDictionary(i => i.impl.Name, i => i.idx),
						Factories = impls.Select(impl =>
							new Lazy<IServiceFactory>(() =>
								ServiceFactory.Create(
									-1 - impl.idx,
									null,
									()=>null,
									decl,
									impl.impl,
									ServiceType,
									ServiceCreatorCache,
									ServiceMetadata,
									null
									)
								)).ToArray()
					};
				});
		}
		public IServiceFactory[] GetUnmanagedServiceFactories(Type ServiceType, bool CreateIfNotExists)
		{
			var re = GetUnmanagedServiceEntry(ServiceType, CreateIfNotExists);
			if (re == null) return Array.Empty<IServiceFactory>();
			return re.Factories.Select(f => f.Value).ToArray();
		}
		public IServiceFactory GetUnmanagedServiceFactory(Type ServiceType,string Name, bool CreateIfNotExists)
		{
			var re = GetUnmanagedServiceEntry(ServiceType, CreateIfNotExists);
			if(re==null) return null;
			if (Name == null)
				return re.Factories[0].Value;
			if (!re.NameMap.TryGetValue(Name, out var idx))
				return null;
			return re.Factories[idx].Value;
		}
	}

}
