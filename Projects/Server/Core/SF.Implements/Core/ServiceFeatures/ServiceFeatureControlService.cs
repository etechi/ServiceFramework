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
		public async Task<string> Init(string Id=null)
		{
			await ServiceProvider.InitServices(Id);
			return "OK";
		}
	}

}
