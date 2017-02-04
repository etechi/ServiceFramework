using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
namespace SF.Core.DI.MicrosoftExtensions
{
	public class DIServiceCollection :  IDIServiceCollection
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
		static ServiceLifetime UnmapLifeTime(global::Microsoft.Extensions.DependencyInjection.ServiceLifetime lifetime)
		{
			switch (lifetime)
			{
				case global::Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped:
					return ServiceLifetime.Scoped;
				case global::Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton:
					return ServiceLifetime.Singleton;
				case global::Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient:
					return ServiceLifetime.Transient;
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

		


		public DIServiceCollection(IServiceCollection InnerCollection)
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
		
		public static IDIServiceCollection Create()
		{
			var sc= new DIServiceCollection(new ServiceCollection());
			sc.InnerCollection.AddTransient<IDIScopeFactory, ScopeFactory>();
			return sc;
		}

		public IEnumerator<ServiceDescriptor> GetEnumerator()
		{
			return InnerCollection.Select(s =>
			{
				if (s.ImplementationInstance != null)
					return new ServiceDescriptor(s.ServiceType, s.ImplementationInstance);
				else if (s.ImplementationType != null)
					return new ServiceDescriptor(s.ServiceType, s.ImplementationType, UnmapLifeTime(s.Lifetime));
				else if (s.ImplementationFactory != null)
					return new ServiceDescriptor(s.ServiceType, s.ImplementationFactory, UnmapLifeTime(s.Lifetime));
				else
					throw new NotSupportedException();
			}).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
	public static class IDIServiceCollectionExtension
	{
		public static IServiceProvider BuildServiceProvider(this IDIServiceCollection sc)
		{
			return ((DIServiceCollection)sc).InnerCollection.BuildServiceProvider();
		}
	}
}
