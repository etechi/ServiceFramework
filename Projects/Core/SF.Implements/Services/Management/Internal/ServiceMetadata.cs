using SF.Core.DI;
using System;
using System.Collections.Generic;
using System.Linq;
namespace SF.Services.Management.Internal
{
	class ServiceMetadata : IServiceMetadata
	{
		public IEnumerable<KeyValuePair<Type, IReadOnlyList<Type>>> ManagedServices { get; }
		public IEnumerable<Type> NormalServices { get; }

		Dictionary<Type,ServiceType> ServiceTypes { get; }
		public ServiceType GetServiceType(Type type)
		{
			ServiceType st;
			return ServiceTypes.TryGetValue(type, out st) ? st : ServiceType.Unknown;
		}

		public ServiceMetadata(IEnumerable<Type> NormalServices,IEnumerable<ServiceDescriptor> SerciceDescriptors)
		{
			this.NormalServices = NormalServices.ToArray();
			this.ManagedServices = SerciceDescriptors
				.GroupBy(m => m.ServiceType)
				.Select(g => new KeyValuePair<Type, IReadOnlyList<Type>>(
					g.Key, 
					g.Select(i => i.ImplementType).ToArray()
					))
				.ToArray();
			this.ServiceTypes=
				this.NormalServices.Select(s => new { s = s, t = ServiceType.Normal })
				.Union(this.ManagedServices.Select(p => new { s = p.Key, t = ServiceType.Managed }))
				.ToDictionary(p => p.s, p => p.t);
		}
	}

}
