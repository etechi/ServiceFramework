using System;
using System.Collections.Generic;

namespace SF.Core.DI
{
	public abstract class ServiceConfiguration<T>
		where T: ServiceConfiguration<T>,new()
	{
		static Lazy<IServiceProvider> Instance { get; } = new Lazy<IServiceProvider>(()=>
		{
			var sc = new T();
			sc.OnConfigServices(sc.Services,sc.EnvironmentType);
			return sc.OnBuildServiceProvider(sc.Services,sc.EnvironmentType);
		});
		public static IServiceProvider ServiceProvider  => Instance.Value;
		public ServiceConfiguration(IDIServiceCollection Services, EnvironmentType EnvironmentType)
		{
			this.Services = Services;
			this.EnvironmentType = EnvironmentType;
		}
		EnvironmentType EnvironmentType { get; }
		IDIServiceCollection Services { get; }
		protected abstract void OnConfigServices(IDIServiceCollection Services, EnvironmentType EnvironmentType);
		protected abstract IServiceProvider OnBuildServiceProvider(IDIServiceCollection Services, EnvironmentType EnvironmentType);

	}
}
