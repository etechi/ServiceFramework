using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using SF.Core.NetworkService.Metadata;
using System.Text;
using System.Threading.Tasks;
using SF.Core.ServiceFeatures;
using SF.Core.Logging;

namespace SF.Core.ServiceFeatures
{
	public class ServiceFeatureControlService : IServiceFeatureControlService
	{
		IServiceProvider ServiceProvider { get; }
		ILogger Logger { get; }
		public ServiceFeatureControlService(
			IServiceProvider ServiceProvider,
			ILogger<ServiceFeatureControlService> Logger
			)
		{
			this.Logger = Logger;
			this.ServiceProvider = ServiceProvider;
		}
		public async Task<string> Init(string Group=null)
		{
			Logger.Info("服务初始化开始");
			await ServiceProvider.InitServices(Group);
			Logger.Info("服务初始化结束");
			return "OK";
		}
	}

}
