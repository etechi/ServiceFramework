using System;

using Microsoft.Extensions.DependencyInjection;

namespace SF.DI.Microsoft
{
	public class ServiceResolver : IServiceResolver
	{
		public IServiceProvider ServiceProvider { get; }
		public ServiceResolver(IServiceProvider ServiceProvider)
		{
			this.ServiceProvider = ServiceProvider;
		}
		public object Resolve(Type Type) => ServiceProvider.GetService(Type);
	}
	public class DIScopeFactory : IDIScopeFactory
	{
		class Scope : IDIScope,IServiceResolver
		{
			public IServiceScope InnerScope { get; }
			ServiceResolver ServiceResolver { get; }
			public IServiceResolver Resolver => this;

			public Scope(IServiceScope InnerScope)
			{
				this.InnerScope = InnerScope;
			}

			public void Dispose()
			{
				InnerScope.Dispose();
			}

			public object Resolve(Type Type)
			{
				return InnerScope.ServiceProvider.GetService(Type);
			}
		}
		IServiceScopeFactory ServiceScopeFactory { get; }
		public DIScopeFactory(IServiceScopeFactory ServiceScopeFactory)
		{
			this.ServiceScopeFactory = ServiceScopeFactory;
		}

		public IDIScope BeginScope()
		{
			return new Scope(ServiceScopeFactory.CreateScope());
		}
	}
	public class ServiceCollection :
		SF.DI.IServiceCollection
	{
		global::Microsoft.Extensions.DependencyInjection.IServiceCollection Collection { get; }
		public ServiceCollection( global::Microsoft.Extensions.DependencyInjection.IServiceCollection Collection)
		{
			this.Collection = Collection;
		}
		public void Add(Type Interface, Func<IServiceResolver, object> ImplementCreator, ServiceLifetime LiftTime)
		{
			switch (LiftTime)
			{
				case ServiceLifetime.Scoped:
					ServiceCollectionServiceExtensions.AddScoped(
						Collection, 
						Interface, 
						p=>ImplementCreator(new ServiceResolver(p))
						);
					break;
				case ServiceLifetime.Singleton:
					ServiceCollectionServiceExtensions.AddSingleton(
						Collection, 
						Interface, 
						p => ImplementCreator(new ServiceResolver(p))
						);
					break;
				case ServiceLifetime.Transient:
					ServiceCollectionServiceExtensions.AddTransient(
						Collection, 
						Interface, 
						p => ImplementCreator(new ServiceResolver(p))
						);
					break;
			}
			
		}
		public void Add(Type Interface, Type Implement, ServiceLifetime LiftTime)
		{
			switch (LiftTime)
			{
				case ServiceLifetime.Scoped:
					ServiceCollectionServiceExtensions.AddScoped(Collection, Interface, Implement);
					break;
				case ServiceLifetime.Singleton:
					ServiceCollectionServiceExtensions.AddSingleton(Collection, Interface, Implement);
					break;
				case ServiceLifetime.Transient:
					ServiceCollectionServiceExtensions.AddTransient(Collection, Interface, Implement);
					break;
			}
		}

		public void AddSingleton(Type Interface, object Implement)
		{
			ServiceCollectionServiceExtensions.AddSingleton(Collection, Interface, Implement);     
		}
	}
}
