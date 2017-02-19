using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using SF.Core.NetworkService.Metadata;
using System.Text;
using System.Threading.Tasks;
using SF.Core.ServiceFeatures;

namespace SF.Core.ServiceFeatures
{
	public class ServiceFeatureControlService : IServiceFeatureControlService
	{
		IServiceProvider ServiceProvider { get; }
		public ServiceFeatureControlService(IServiceProvider ServiceProvider)
		{
			this.ServiceProvider = ServiceProvider;
		}
		public async Task Init()
		{
			await ServiceProvider.InitServices();
		}
	}

}
