using System;

using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using SF.Core.ServiceManagement.Internals;
using System.Linq;
namespace SF.Core.ServiceManagement
{
	public static class MSDependencyInjectionExtension
	{
		class MSScope : Microsoft.Extensions.DependencyInjection.IServiceScope
		{
			IServiceScope ServiceScope { get;}
			public MSScope(IServiceScope ServiceScope)
			{
				this.ServiceScope = ServiceScope;
			}
			public IServiceProvider ServiceProvider => ServiceScope.ServiceProvider;

			public void Dispose()
			{
				ServiceScope.Dispose();
			}
		}
		class MSScopeFactory :Microsoft.Extensions.DependencyInjection.IServiceScopeFactory
		{

			IServiceScopeFactory ScopeFactory { get; }
			
			public MSScopeFactory(IServiceScopeFactory ScopeFactory)
			{
				this.ScopeFactory = ScopeFactory;
			}

			public Microsoft.Extensions.DependencyInjection.IServiceScope CreateScope()
			{
				return new MSScope(ScopeFactory.CreateServiceScope());
			}
		}
		static ServiceLifetime MapLifetime(ServiceImplementLifetime lifetime)
		{
			switch (lifetime)
			{
				case ServiceImplementLifetime.Scoped:
					return ServiceLifetime.Scoped;
				case ServiceImplementLifetime.Singleton:
					return ServiceLifetime.Singleton;
					case ServiceImplementLifetime.Transient:
					return ServiceLifetime.Transient;
				default:
					throw new NotSupportedException();
			}
		}
		static ServiceImplementLifetime MapLifetime(ServiceLifetime lifetime)
		{
			switch (lifetime)
			{
				case ServiceLifetime.Scoped:
					return ServiceImplementLifetime.Scoped;
				case ServiceLifetime.Singleton:
					return ServiceImplementLifetime.Singleton;
				case ServiceLifetime.Transient:
					return ServiceImplementLifetime.Transient;
				default:
					throw new NotSupportedException();
			}
		}
		public static IServiceCollection AddServices(this IServiceCollection Services, Microsoft.Extensions.DependencyInjection.ServiceCollection MSServices)
		{
			return Services.AddRange(
				MSServices.Select(s =>
				{
					if (s.ImplementationFactory != null)
						return new ServiceDescriptor(s.ServiceType, s.ImplementationFactory, MapLifetime(s.Lifetime));
					else if (s.ImplementationInstance != null)
						return new ServiceDescriptor(s.ServiceType, s.ImplementationInstance);
					else if (s.ImplementationType != null)
						return new ServiceDescriptor(s.ServiceType, s.ImplementationType, MapLifetime(s.Lifetime));
					else
						throw new NotSupportedException();
				}));
		}
	}
}
