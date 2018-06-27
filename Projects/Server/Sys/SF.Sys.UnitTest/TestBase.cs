using SF.Sys.Hosting;
using SF.Sys.Services;
using System;
using System.Threading.Tasks;

namespace SF.Sys.UnitTest
{
	public class TestBase 
	{
		public IAppInstanceBuilder AppInstanceBuilder { get; }

		//public Task<T> ScopedServices<S,T>(Func<S,Task<T>> Callback)
		//	=> AppInstanceBuilder.AppInstance(async app =>
		//			{
		//				await OnInitServices(app.ServiceProvider);
		//				return await app.ServiceProvider.WithScopedServices(Callback);
		//			}
		//		);

		public IScope<IServiceProvider> NewServiceScope()
			=> from sp in AppInstanceBuilder.NewServiceScope()
			   from t in InitServices(sp)
			   select sp;


		//public IScope<IAppInstance> WithAppInstance()
		//	=>SF.Sys.Scope.Create(cb=>
		//		AppInstanceBuilder.AppInstance(app =>
		//			cb(app)
		//		);
		public TestBase() : this(TestAppBuilder.Instance)
		{ }
		public TestBase(IAppInstanceBuilder AppInstanceBuilder)
		{
			this.AppInstanceBuilder = AppInstanceBuilder;
		}
		async Task<int> InitServices(IServiceProvider sp)
		{
			await OnInitServices(sp);
			return 0;
		}

		protected virtual Task OnInitServices(IServiceProvider sp)
		{
			return Task.CompletedTask;

		}
		public string NewRandString(int Len)
			=>Strings.NumberAndLowerUpperChars.Random(Len);
	}
	

}
