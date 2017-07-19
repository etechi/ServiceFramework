using SF.Core.DI;
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

			public ServiceImplementType ServiceImplementType { get; set; }

			public bool IsManagedService { get; set; }
		}
		public class Service : IServiceDeclaration
		{
			public string ServiceName { get; set; }
			public Type ServiceType { get; set; }
			public IReadOnlyList<IServiceImplement> Implements{ get; set; }
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


		public ServiceMetadata(IEnumerable<ServiceDescriptor> ServiceDescriptors)
		{
			var lists = (
				from svc in ServiceDescriptors
				select new ServiceImplement
				{
					ServiceType = svc.InterfaceType,
						ImplementType = svc.ImplementType,
						ImplementName=svc.ImplementType?.FullName,
						LifeTime = svc.Lifetime,
						IsManagedService = svc.ImplementType != null &&
									svc.Lifetime != ServiceImplementLifetime.Singleton &&
									svc.IsManagedServiceImplement(),
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
						ServiceName = g.Key.FullName,
						Implements = g.ToArray()
					});
			ServicesByTypeName = Services.ToDictionary(p => p.Key.GetFullName(), p => p.Value);

			ImplementsByTypeName = lists
				.Where(i => i.ImplementType != null)
				.GroupBy(i => i.ImplementType)
				.ToDictionary(g => g.Key.GetFullName(), g => g.Cast<IServiceImplement>().ToArray());


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
			return Services.ContainsKey(Type);
		}
	}

}
