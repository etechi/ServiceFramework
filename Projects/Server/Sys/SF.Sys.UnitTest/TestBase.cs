using SF.Sys.Hosting;
using SF.Sys.Services;
using System;
using System.Threading.Tasks;

namespace SF.Sys.UnitTest
{
	public class TestBase 
	{
		public IAppInstanceBuilder AppInstanceBuilder { get; }

		public Task<T> ScopedServices<S,T>(Func<S,Task<T>> Callback)
			=> AppInstanceBuilder.AppInstance(async app => 
					await app.ServiceProvider.WithScopedServices(Callback)
				);

		public Task<T> Scope<T>(Func<IServiceProvider, Task<T>> Callback)
			=> AppInstanceBuilder.Scope(Callback);

		public Task<T> WithAppInstance<T>(Func<IServiceProvider,Task<T>> Callback)
			=> AppInstanceBuilder.AppInstance(app=>Callback(app.ServiceProvider));

		public TestBase(IAppInstanceBuilder AppInstanceBuilder)
		{
			this.AppInstanceBuilder = AppInstanceBuilder;
		}
		
	}
	

}
