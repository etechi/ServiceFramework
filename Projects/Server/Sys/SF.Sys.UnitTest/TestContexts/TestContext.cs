using SF.Sys.Data;
using SF.Sys.Hosting;
using SF.Sys.Services;
using SF.Sys.TimeServices;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys.ServiceFeatures;

namespace SF.Sys.UnitTest
{
	public static class TestContextExtension
	{
		class AppInstanceBuilderRootTestContext : IScope<IServiceProvider>
		{
			public IAppInstanceBuilder AppInstanceBuilder { get; set; }
			public bool BootServices { get; set; }

			public async Task Use(Func<IServiceProvider, Task> Callback)
			{
				using (var app = AppInstanceBuilder.Build())
				{
					IDisposable Disposable = null;
					if (BootServices)
						Disposable = await app.ServiceProvider.BootServices();
					try
					{
						await Callback(app.ServiceProvider);
					}
					finally
					{
						if (Disposable != null)
							Disposable.Dispose();
					}
				}
			}
		}
		public static IScope<IServiceProvider> TestContext(
			this IAppInstanceBuilder builder,
			bool BootServices=true
			)
		{
			return new AppInstanceBuilderRootTestContext
			{
				AppInstanceBuilder = builder,
				BootServices= BootServices
			};
		}

		class ServiceProviderRootTestContext : IScope<IServiceProvider>
		{
			public IServiceProvider ServiceProvider { get; set; }
			public Task Use(Func<IServiceProvider, Task> Callback)
			{
				return Callback(ServiceProvider);
			}
		}
		public static IScope<IServiceProvider> TestContext(this IServiceProvider sp)
			=> new ServiceProviderRootTestContext { ServiceProvider = sp };

			
	
	}
	

}
