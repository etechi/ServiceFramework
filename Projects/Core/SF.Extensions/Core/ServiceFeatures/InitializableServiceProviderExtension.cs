using SF.Core.ServiceFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Core.ServiceManagement;
using SF.Core.Logging;

namespace SF.Core.ServiceFeatures
{
	public static class InitializableServiceProviderExtension
	{
		class ServiceInitializer
		{
			public static ServiceInitializer Instance { get; } = new ServiceInitializer();
			public async Task InitServices(IServiceProvider sp)
			{
				var bss = sp.Resolve<IEnumerable<IServiceInitializable>>();
				var logger = sp.Resolve<ILogger<ServiceInitializer>>();
				foreach (var bs in bss)
				{
					logger.Info("初始化开始:{0}", bs.Title);
					await bs.Init();
					logger.Info("初始化结束:{0}", bs.Title);
				}
			}
		}
		public static async Task InitServices(this IServiceProvider sp)
		{
			await ServiceInitializer.Instance.InitServices(sp);
		}

	}
}
