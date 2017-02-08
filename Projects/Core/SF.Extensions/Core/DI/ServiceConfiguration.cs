using System;
using System.Collections.Generic;

namespace SF.Core.DI
{
	public abstract class ServiceConfiguration<T>
		where T: ServiceConfiguration<T>,new()
	{
		static Lazy<IServiceProvider> LazyDefaultServiceProvider { get; } = new Lazy<IServiceProvider>(() =>
			new T().ServiceProvider
		);
		public static IServiceProvider DefaultServiceProvider  => LazyDefaultServiceProvider.Value;

		Lazy<IServiceProvider> LazyServiceProvider { get; }
		public IServiceProvider ServiceProvider => LazyServiceProvider.Value;
		public ServiceConfiguration(IDIServiceCollection Services, EnvironmentType EnvironmentType)
		{
			this.Services = Services;
			this.EnvType = EnvironmentType;
			LazyServiceProvider = new Lazy<IServiceProvider>(() =>
			{
				OnConfigServices(Services, EnvType);
				return OnBuildServiceProvider(Services, EnvType);
			});
		}
		EnvironmentType EnvType { get; }
		IDIServiceCollection Services { get; }
		protected abstract void OnConfigServices(IDIServiceCollection Services, EnvironmentType EnvironmentType);
		protected abstract IServiceProvider OnBuildServiceProvider(IDIServiceCollection Services, EnvironmentType EnvironmentType);

	}
}
