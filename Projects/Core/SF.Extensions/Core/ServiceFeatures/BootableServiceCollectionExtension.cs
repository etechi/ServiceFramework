using SF.Core.ServiceFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Core.DI
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
		public static IDIServiceCollection AddBootstrap(
			this IDIServiceCollection sc, 
			Func<IServiceProvider,Task<IDisposable>> Callback)
			{
				sc.Normal().AddSingleton<IServiceBootable>(sp =>
					new BootableHelper
					{
						Callback = () => Callback(sp)
					});
				return sc;
			}
		public static IDIServiceCollection AddBootstrap(this IDIServiceCollection sc, Func<IServiceProvider,Task> Callback)
			=> sc.AddBootstrap(
				async sp => {
					await Callback(sp);
					return Disposable.Empty;
				}
				);
		public static IDIServiceCollection AddBootstrap(
			this IDIServiceCollection sc,
			Func<IServiceProvider, IDisposable> Callback)
			=> sc.AddBootstrap(
				sp =>
				Task.FromResult(Callback(sp))
				);
		
		public static IDIServiceCollection AddBootstrap(this IDIServiceCollection sc, Action<IServiceProvider> Callback)
			=> sc.AddBootstrap(
				sp => {
					Callback(sp);
					return Disposable.Empty;
				}
				);
	}
}
