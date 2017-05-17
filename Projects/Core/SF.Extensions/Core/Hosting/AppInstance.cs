using SF.Core.ServiceManagement;
using SF.Core.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Core.Hosting
{
	public class AppInstance : IAppInstance
	{
		public EnvironmentType EnvType { get; }
		public string Name { get; }
		public IServiceProvider ServiceProvider { get; }
		IDisposable _Shutdown;
		public AppInstance(string Name,EnvironmentType EnvType,IServiceProvider ServiceProvider,IDisposable Shutdown)
		{
			this.Name = Name;
			this.EnvType = EnvType;
			this.ServiceProvider = ServiceProvider;
			this._Shutdown = Shutdown;
		}
		public void Dispose()
		{
			Disposable.Release(ref _Shutdown);
		}
	}
}
