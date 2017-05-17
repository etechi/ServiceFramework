using SF.Core.ServiceFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement
{
	public static class BootableDIServiceCollectionExtension
	{
		class BootableHelper : IServiceBootable
		{
			public Func<Task<IDisposable>> Callback { get; set; }
			public Task<IDisposable> Boot()
			{
				return Callback();
			}
		}
		public static IServiceCollection AddBootstrap(
			this IServiceCollection sc, 
			Func<IServiceProvider,Task<IDisposable>> Callback)
			{
				sc.AddSingleton<IServiceBootable>(sp =>
					new BootableHelper
					{
						Callback = () => Callback(sp)
					});
				return sc;
			}
		public static IServiceCollection AddBootstrap(this IServiceCollection sc, Func<IServiceProvider,Task> Callback)
			=> sc.AddBootstrap(
				async sp => {
					await Callback(sp);
					return Disposable.Empty;
				}
				);
		public static IServiceCollection AddBootstrap(
			this IServiceCollection sc,
			Func<IServiceProvider, IDisposable> Callback)
			=> sc.AddBootstrap(
				sp =>
				Task.FromResult(Callback(sp))
				);
		
		public static IServiceCollection AddBootstrap(this IServiceCollection sc, Action<IServiceProvider> Callback)
			=> sc.AddBootstrap(
				sp => {
					Callback(sp);
					return Disposable.Empty;
				}
				);
	}
}
