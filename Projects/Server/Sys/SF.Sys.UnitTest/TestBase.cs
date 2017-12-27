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
					{
						await OnInitServices(app.ServiceProvider);
						return await app.ServiceProvider.WithScopedServices(Callback);
					}
				);
		public ITestScope<IServiceProvider> TestScope() => AppInstanceBuilder.TestScope();


		public Task<T> Scope<T>(Func<IServiceProvider, Task<T>> Callback)
			=> AppInstanceBuilder.Scope(async sp=>
			{
				await OnInitServices(sp);
				return await Callback(sp);
			});

		public Task<T> WithAppInstance<T>(Func<IServiceProvider,Task<T>> Callback)
			=> AppInstanceBuilder.AppInstance(async app =>
			{
				await OnInitServices(app.ServiceProvider);
				return await Callback(app.ServiceProvider);
			}
			);

		public TestBase(IAppInstanceBuilder AppInstanceBuilder)
		{
			this.AppInstanceBuilder = AppInstanceBuilder;
		}
		protected  virtual Task OnInitServices(IServiceProvider sp)
		{
			return Task.CompletedTask;

		}
		
	}
	

}
