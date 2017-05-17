using SF.Core.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SF.Core.ServiceManagement.Internals
{
	class ServiceMetadata : IServiceMetadata
	{
		class ServiceImplement: IServiceImplement
		{
			public Type ServiceType { get; set; }
			public Type ImplementType { get; set; }
			public ServiceImplementLifetime LifeTime { get; set; }
			public object ImplementInstance { get; set; }
			public Func<IServiceResolver, object> ImplementCreator { get; set; }

			public ServiceImplementType ServiceImplementType { get; set; }

			public bool IsManagedService { get; set; }
		}
		class Service : IServiceDeclaration
		{
			public Type ServiceType { get; set; }
			public IReadOnlyList<IServiceImplement> Implements { get; set; }
		}


		public IReadOnlyDictionary<Type, IServiceDeclaration> Services { get; }

		public ServiceMetadata(IEnumerable<ServiceDescriptor> ServiceDescriptors)
		{
			var lists = ServiceDescriptors
				.Select(impl =>
				 new ServiceImplement
				 {
					 ServiceType = impl.InterfaceType,
					 ImplementType = impl.ImplementType,
					 LifeTime = impl.Lifetime,
					 IsManagedService = impl.ImplementType != null &&
										impl.Lifetime != ServiceImplementLifetime.Singleton &&
										impl.IsManagedServiceImplement(),
					 ImplementCreator = impl.ImplementCreator,
					 ImplementInstance = impl.ImplementInstance,
					 ServiceImplementType = impl.ServiceImplementType
				 }).Cast<IServiceImplement>().ToArray();

			Services = lists
				.GroupBy(i => i.ServiceType)
				.ToDictionary(
					g => g.Key,
					g => (IServiceDeclaration)new Service
					{
						ServiceType = g.Key,
						Implements = g.ToArray()
					});
		}

		public bool IsService(Type Type)
		{
			return Services.ContainsKey(Type);
		}
	}

}
