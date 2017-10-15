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
using System.Reflection;

namespace SF.Core.ServiceManagement.Internals
{
	class ServiceMetadata : IServiceMetadata
	{

		public class ServiceImplement : IServiceImplement
		{
			public string ImplementName { get; set; }
			public Type ServiceType { get; set; }
			public Type ImplementType { get; set; }
			public ServiceImplementLifetime LifeTime { get; set; }
			public object ImplementInstance { get; set; }
			public Func<IServiceProvider, object> ImplementCreator { get; set; }
			public MethodInfo ImplementMethod { get; set; }
			public bool IsDataScope { get; set; }
			public ServiceImplementType ServiceImplementType { get; set; }

			public bool IsManagedService { get; set; }
			public string Name { get; set; }
			public IManagedServiceInitializer ManagedServiceInitializer { get; set; }
		}
		public class Service : IServiceDeclaration
		{
			public string ServiceName { get; set; }
			public Type ServiceType { get; set; }
			public IReadOnlyList<IServiceImplement> Implements{ get; set; }
			public bool HasManagedServiceImplement { get; set; }
			public bool HasUnmanagedServiceImplement { get; set; }
		}

		//public Type DetectServiceType(Type InterfaceType)
		//{
		//	var re = ServiceTypeForInterface.Get(InterfaceType);
		//	if (re == null)
		//		throw new InvalidOperationException($"找不到服务接口{InterfaceType}");
		//	if (re is Type)
		//		return (Type)re;
		//	throw new InvalidOperationException($"接口{InterfaceType}被用于多个服务{((Type[])re).Join(",")}");
		//}

		//public Dictionary<Type,object> ServiceTypeForInterface { get; }
		public IReadOnlyDictionary<Type, IServiceDeclaration> Services { get; }
		public IReadOnlyDictionary<string, IServiceDeclaration> ServicesByTypeName { get; }
		public IReadOnlyDictionary<string, IServiceImplement[]> ImplementsByTypeName { get; }
		public IReadOnlyDictionary<string, IServiceDeclaration> ServicesById { get; }
		public IReadOnlyDictionary<string, IServiceImplement> ImplementsById { get; }


		public ServiceMetadata(IEnumerable<ServiceDescriptor> ServiceDescriptors)
		{
			var lists = (
				from svc in ServiceDescriptors
				select new ServiceImplement
				{
					ServiceType = svc.InterfaceType,
					ImplementType = svc.ImplementType,
					ImplementName=svc.ImplementType?.GetFullName(),
					LifeTime = svc.Lifetime,
					IsManagedService = svc.IsManagedService,
					Name=svc.Name,
					IsDataScope=svc.IsDataScope,
					ManagedServiceInitializer = svc.ManagedServiceInitializer,
					ImplementCreator = svc.ImplementCreator,
					ImplementMethod=svc.ImplementMethod,
					ImplementInstance = svc.ImplementInstance,
					ServiceImplementType = svc.ServiceImplementType
				}
				).ToArray();

			Services = lists
				.GroupBy(i => i.ServiceType)
				.ToDictionary(
					g => g.Key,
					g => (IServiceDeclaration)new Service
					{
						ServiceType = g.Key,
						ServiceName = g.Key.GetFullName(),
						Implements = g.ToArray(),
						HasManagedServiceImplement=g.Any(i=>i.IsManagedService),
						HasUnmanagedServiceImplement = g.Any(i =>!i.IsManagedService),
					});
			ServicesByTypeName = Services.ToDictionary(p => p.Key.GetFullName(), p => p.Value);

			ImplementsByTypeName = lists
				.Where(i => i.ImplementType != null)
				.GroupBy(i => i.ImplementType)
				.ToDictionary(g => g.Key.GetFullName(), g => g.Cast<IServiceImplement>().ToArray());

			ServicesById = ServicesByTypeName.ToDictionary(p => p.Key.UTF8Bytes().MD5().Hex(), p => p.Value);

			ImplementsByTypeName = lists
				.Where(i => i.ImplementType != null)
				.GroupBy(i => i.ImplementType)
				.ToDictionary(g => g.Key.GetFullName(), g => g.Cast<IServiceImplement>().ToArray());

			var dupImpls = (from i in lists
							where i.ImplementType != null && i.IsManagedService
							let svc = i.ServiceType
							let impl = i.ImplementType
							group (svc, impl) by (svc, impl) into g
							where g.Count() > 1
							group g.Key.impl by g.Key.svc into gi
							select gi
						   ).ToArray();
						   
			if (dupImpls.Length > 0)
				throw new InvalidOperationException(
					"服务实现重复定义:"+
					dupImpls.Select(di=>$"服务:{di.Key.GetFullName()}的实现包括:{di.Select(i=>i.GetFullName()).Join(",")}").Join(";")
					);

			ImplementsById = lists
				.Where(i => i.ImplementType != null && i.IsManagedService)
				.ToDictionary(i => 
				$"{i.ImplementType.GetFullName()}@{i.ServiceType.GetFullName()}".UTF8Bytes().MD5().Hex(),
				i => i as IServiceImplement
				);

			//ServiceTypeForInterface =
			//	(from svc in Services.Values
			//	 from impl in svc.Implements
			//	 from si in impl.Interfaces
			//	 select new { svc = svc, si = si }
			//	).GroupBy(i => i.si.Type)
			//	.Select(g => new { type = g.Key, services = g.Select(i => i.svc.ServiceType).Distinct().ToArray() })
			//	.ToDictionary(i => i.type, i => i.services.Count() == 1 ? (object)i.services[0] : i.services);

		}

		public bool IsService(Type Type)
		{
			return Services.ContainsKey(Type) || Type.IsDefined(typeof(AutoBindAttribute));
		}
	}

}
