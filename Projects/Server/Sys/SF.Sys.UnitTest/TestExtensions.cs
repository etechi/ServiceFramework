using SF.Sys.Data;
using SF.Sys.Hosting;
using SF.Sys.Services;
using System;
using System.Threading.Tasks;

namespace SF.Sys.UnitTest
{
	public static class TestExtensions
	{
		public static async Task<T> AppInstance<T>(this IAppInstanceBuilder AppInstanceBuilder,Func<IAppInstance,Task<T>> Callback)
		{
			using (var ins = AppInstanceBuilder.Build())
				return await Callback(ins);
		}
		public static async Task<T> Scope<T>(this IAppInstance AppInstance, Func<IServiceProvider,Task<T>> Callback)
		{
			using (var s = AppInstance.ServiceProvider.Resolve<IServiceScopeFactory>().CreateServiceScope())
			{
				return await Callback(s.ServiceProvider);
			}
		}
		public static async Task<T> Scope<T>(this IAppInstanceBuilder AppInstanceBuilder, Func<IServiceProvider, Task<T>> Callback)
		{
			return await AppInstanceBuilder.AppInstance(ins => ins.Scope(Callback));
		}

		public static async Task<long> GetIdent(this IServiceProvider sp)
		{
			var ig = sp.Resolve<IIdentGenerator>();
			return await ig.GenerateAsync("≤‚ ‘–Ú∫≈");
		}
	}
	

}
