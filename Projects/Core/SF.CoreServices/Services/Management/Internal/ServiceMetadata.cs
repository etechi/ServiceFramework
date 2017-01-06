using System;
using System.Collections.Generic;
using System.Linq;
namespace SF.Services.Management.Internal
{
	class ServiceMetadata : IServiceMetadata
	{
		public IEnumerable<KeyValuePair<Type, IReadOnlyList<Type>>> ManagedServices { get; }
		public IEnumerable<Type> NormalServices { get; }

		Dictionary<Type,ServiceType> serviceTypes { get; }
		public ServiceType GetServiceType(Type type)
		{
			return serviceTypes.TryGetValue(type, out var st) ? st : ServiceType.Unknown;
		}

		public ServiceMetadata(IEnumerable<Type> NormalServices,IEnumerable<ManagedServiceDescriptor> ManagedServices)
		{
			this.NormalServices = NormalServices.ToArray();
			this.ManagedServices = ManagedServices
				.GroupBy(m => m.ServiceType)
				.Select(g => new KeyValuePair<Type, IReadOnlyList<Type>>(
					g.Key, 
					g.Select(i => i.ImplementType).ToArray()
					))
				.ToArray();
			this.serviceTypes=
				this.NormalServices.Select(s => new { s = s, t = ServiceType.Normal })
				.Union(this.ManagedServices.Select(p => new { s = p.Key, t = ServiceType.Managed }))
				.ToDictionary(p => p.s, p => p.t);
		}
	}

}
