using SF.Core.DI;
using SF.Core.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SF.Core.ServiceFeatures;
using SF.Core.Logging;

namespace SF.Core.Hosting
{
	public abstract class BaseAppInstanceBuilder<T>
		where T: BaseAppInstanceBuilder<T>, new()
	{
		public EnvironmentType EnvType { get; private set; }
		public string Name { get; private set; }
		public ILogService LogService { get; private set; }
		IDIServiceCollection ServiceCollection { get; set; }

		public static IAppInstance Build(EnvironmentType EnvType, string Name=null, IDIServiceCollection ServiceCollection=null, ILogService LogService = null)
		{
			var builder = new T();
			builder.Name = Name;
			builder.EnvType = EnvType;
			builder.LogService = LogService?? builder.OnCreateLogService();
			builder.ServiceCollection = ServiceCollection;
			return builder.OnBuild();
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
		protected virtual IDIServiceCollection OnBuildServiceCollection()
		{
			if (ServiceCollection == null)
				throw new InvalidOperationException("Require ServiceCollection");
			return ServiceCollection;
		}
		protected abstract void OnConfigServices(IDIServiceCollection Services);
		protected abstract IServiceProvider OnBuildServiceProvider(IDIServiceCollection Services);

		protected virtual IAppInstance OnBuildAppInstance(IServiceProvider ServiceProvider,IDisposable Shutdown)
		{
			return new AppInstance(Name, EnvType, ServiceProvider, Shutdown);
		}

		protected virtual IAppInstance OnBuild()
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
