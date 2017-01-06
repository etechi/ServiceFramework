using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
namespace SF.DI
{

	public class DIServcieCollection :  IDIServiceCollection
	{
		static global::Microsoft.Extensions.DependencyInjection.ServiceLifetime MapLifeTime(ServiceLifetime lifetime)
		{
			switch (lifetime)
			{
				case ServiceLifetime.Scoped:
					return global::Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped;
				case ServiceLifetime.Singleton:
					return global::Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton;
				case ServiceLifetime.Transient:
					return global::Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient;
				default:
					throw new NotSupportedException();
			}
		}
		static global::Microsoft.Extensions.DependencyInjection.ServiceDescriptor MapDescriptor(ServiceDescriptor Descriptor)
		{
			switch (Descriptor.ServiceImplementType)
			{
				case ServiceImplementType.Creator:
					return new global::Microsoft.Extensions.DependencyInjection.ServiceDescriptor(
						Descriptor.ServiceType,
						Descriptor.ImplementCreator,
						MapLifeTime(Descriptor.Lifetime)
						);
				case ServiceImplementType.Instance:
					return new global::Microsoft.Extensions.DependencyInjection.ServiceDescriptor(
						Descriptor.ServiceType,
						Descriptor.ImplementInstance
						);
				case ServiceImplementType.Type:
					return new global::Microsoft.Extensions.DependencyInjection.ServiceDescriptor(
						Descriptor.ServiceType,
						Descriptor.ImplementType,
						MapLifeTime(Descriptor.Lifetime)
						);
				default:
					throw new NotSupportedException();

			}
		}

		public IServiceCollection InnerCollection { get; }

		public IEnumerable<Type> ServiceTypes =>
			InnerCollection.Select(s => s.ServiceType).Distinct();


		public DIServcieCollection(IServiceCollection InnerCollection)
		{
			this.InnerCollection = InnerCollection;
		}
		public void Add(ServiceDescriptor Descriptor)
		{
			InnerCollection.Add(MapDescriptor(Descriptor));
		}

		public void Remove(Type Service)
		{
			var idxs = InnerCollection
				.Select((s, i) => new { s = s, i = i })
				.Where(s => s.s.ServiceType == Service)
				.Select(s => s.i)
				.Reverse()
				.ToArray();
			foreach (var i in idxs)
				InnerCollection.RemoveAt(i);
		}
	}
}
