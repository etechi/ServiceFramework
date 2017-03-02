using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
			if(!this.GetServiceTypes().Any(t=>t==typeof(IDIScopeFactory)))
				this.AddTransient<IDIScopeFactory, ScopeFactory>();
		}
		static object NewLazy<T>(IServiceProvider sp)
		{
			return new Lazy<T>(() => (T)sp.GetRequiredService(typeof(T)));
		}
		static object NewCreator<T>(IServiceProvider sp)
		{
			return new Func<T>(() => (T)sp.GetRequiredService(typeof(T)));
		}
		static MethodInfo NewLazyMethodInfo = typeof(DIServiceCollection)
				.GetMethodExt(nameof(NewLazy), typeof(IServiceProvider));
		static MethodInfo NewCreatorMethodInfo= typeof(DIServiceCollection)
			.GetMethodExt(nameof(NewCreator), typeof(IServiceProvider));

		public void Add(ServiceDescriptor Descriptor)
		{
			var desc = MapDescriptor(Descriptor);
			InnerCollection.Add(desc);
			if (Descriptor.ServiceImplementType==ServiceImplementType.Type)
			{
				var td = Descriptor.ImplementType.IsGeneric() ? Descriptor.ImplementType.GetGenericTypeDefinition() : null;
				if (td == null || td != typeof(Lazy<>) && td != typeof(Func<>))
				{
					var newLazy = NewLazyMethodInfo
						.MakeGenericMethod(Descriptor.ServiceType)
						.CreateDelegate<Func<IServiceProvider, object>>();
					InnerCollection.Add(new Microsoft.Extensions.DependencyInjection.ServiceDescriptor(
						typeof(Lazy<>).MakeGenericType(desc.ServiceType),
						newLazy,
						desc.Lifetime
						));
					var newCreator = NewCreatorMethodInfo
						.MakeGenericMethod(Descriptor.ServiceType)
						.CreateDelegate<Func<IServiceProvider, object>>();
					InnerCollection.Add(new Microsoft.Extensions.DependencyInjection.ServiceDescriptor(
						typeof(Func<>).MakeGenericType(desc.ServiceType),
						newCreator,
						desc.Lifetime
						));
				}
			}
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
			return sc;
		}

		public IEnumerator<ServiceDescriptor> GetEnumerator()
		{
			return InnerCollection.Where(s=>
			{
				var type = s.ServiceType;
				if (!type.IsGeneric()) return true;
				var gd = type.GetGenericTypeDefinition();
				return gd != typeof(Func<>) && gd != typeof(Lazy<>);
			}).Select(s =>
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
