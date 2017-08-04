using SF.Core.Logging;
using SF.Core.ServiceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Hosting
{
    public interface IAppInstanceBuilder
    {
		EnvironmentType EnvType { get; }
		IServiceCollection Services { get; }
		ILogService LogService { get; }
		IAppInstance Build(Func<IServiceCollection, IServiceProvider> BuildServiceProvider);
		void AddStartupAction(Func<IAppInstance, IDisposable> action);
	}
}
