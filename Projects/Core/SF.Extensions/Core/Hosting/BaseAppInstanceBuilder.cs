using SF.Core.DI;
using SF.Core.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SF.Core.ServiceFeatures;
using SF.Core.Logging;

namespace SF.Core.Hosting
{
	public abstract class BaseAppInstanceBuilder: IAppInstanceBuilder
	{
		public EnvironmentType EnvType { get; }
		public string Name { get; }
		public ILogService LogService { get; }
		public BaseAppInstanceBuilder(EnvironmentType EnvType, string Name=null, ILogService LogService = null)
		{
			this.Name = Name;
			this.EnvType = EnvType;
			this.LogService = LogService?? OnCreateLogService();
		}
		protected virtual void OnInitServices(IServiceProvider sp)
		{
			sp.InitServices().Wait();
		}
		protected virtual IDisposable OnBootServices(IServiceProvider sp)
		{
			if (EnvType == EnvironmentType.Utils)
				return Disposable.Empty;
			return sp.BootServices().Result;
		}
		protected virtual ILogService OnCreateLogService()
		{
			return null;
		}
		protected abstract IDIServiceCollection OnBuildServiceCollection();
		protected abstract void OnConfigServices(IDIServiceCollection Services);
		protected abstract IServiceProvider OnBuildServiceProvider(IDIServiceCollection Services);

		protected virtual IAppInstance OnBuildAppInstance(IServiceProvider ServiceProvider,IDisposable Shutdown)
		{
			return new AppInstance(Name, EnvType, ServiceProvider, Shutdown);
		}

		public IAppInstance Build()
		{
			var Services = OnBuildServiceCollection();
			IAppInstance ai= null;
			Services.AddTransient<IAppInstance>(isp=> {
				if (ai == null)
					throw new InvalidOperationException();
				return ai;
			});
			OnConfigServices(Services);
			var sp = OnBuildServiceProvider(Services);
			//OnInitServices(sp);
			var shutdown = OnBootServices(sp);

			return ai= OnBuildAppInstance(sp,shutdown);
		}
	}
}
