using SF.Core.DI;
using System;
using System.Collections.Generic;
using System.Linq;
namespace SF.Services.ManagedServices.Runtime
{
	class ServiceMetadata : IServiceMetadata
	{
		public IReadOnlyDictionary<Type, IReadOnlyList<IManagedServiceDescriptor>> ManagedServices { get; }

		public IEnumerable<Type> NormalServices { get; }

		Dictionary<Type,ServiceType> ServiceTypes { get; }
		public ServiceType GetServiceType(Type type)
		{
			ServiceType st;
			return ServiceTypes.TryGetValue(type, out st) ? st : ServiceType.Unknown;
		}
		class ManagedServiceDescriptor : IManagedServiceDescriptor
		{
			public bool IsScopedLifeTime { get; set; }

			public Type Type { get; set; }
		}
		public ServiceMetadata(IEnumerable<Type> ServiceTypes,IEnumerable<ServiceDescriptor> ServiceDescriptors)
		{
			this.ManagedServices = ServiceDescriptors
				   .GroupBy(m => m.ServiceType)
				   .ToDictionary(
						g=>g.Key,
					   g=>(IReadOnlyList<IManagedServiceDescriptor>)g.Select(i => new ManagedServiceDescriptor
					   {
						   Type = i.ImplementType,
						   IsScopedLifeTime = i.Lifetime == ServiceLifetime.Scoped
					   })
					   .Cast<IManagedServiceDescriptor>()
					   .ToArray()
					   );
			this.NormalServices = ServiceTypes.Where(t=>!ManagedServices.ContainsKey(t)).ToArray();
			this.ServiceTypes=
				this.NormalServices.Select(s => new { s = s, t = ServiceType.Normal })
				.Union(this.ManagedServices.Select(p => new { s = p.Key, t = ServiceType.Managed }))
				.ToDictionary(p => p.s, p => p.t);
		}
	}

}
