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
	public interface ITestContext<V>
	{
		Task Exec(Func<IServiceProvider, V, Task> Callback);
	}
	public static class TestContextExtension
	{
		class AppInstanceBuilderRootTestContext : ITestContext<IServiceProvider>
		{
			public IAppInstanceBuilder AppInstanceBuilder { get; set; }
			public bool BootServices { get; set; }

			public async Task Exec(Func<IServiceProvider, IServiceProvider, Task> Callback)
			{
				using (var app = AppInstanceBuilder.Build())
				{
					IDisposable Disposable = null;
					if (BootServices)
						Disposable = await app.ServiceProvider.BootServices();
					try
					{
						await Callback(app.ServiceProvider, app.ServiceProvider);
					}
					finally
					{
						if (Disposable != null)
							Disposable.Dispose();
					}
				}
			}
		}
		public static ITestContext<IServiceProvider> TestContext(
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

		class ServiceProviderRootTestContext : ITestContext<IServiceProvider>
		{
			public IServiceProvider ServiceProvider { get; set; }
			public Task Exec(Func<IServiceProvider, IServiceProvider, Task> Callback)
			{
				return Callback(ServiceProvider, ServiceProvider);
			}
		}
		public static ITestContext<IServiceProvider> TestContext(this IServiceProvider sp)
			=> new ServiceProviderRootTestContext { ServiceProvider = sp };

		abstract class BaseContext<OV,NV> : ITestContext<NV>
		{
			public ITestContext<OV> PrevContext { get; set; }
			public Task Exec(Func<IServiceProvider, NV, Task> Callback)
			{
				return PrevContext.Exec(async (sp,ov) =>
				{
					await OnRun(sp, ov, Callback);
				});
			}
			protected abstract Task OnRun(IServiceProvider ServiceProvider, OV PrevValue, Func<IServiceProvider, NV, Task> Callback);
		}
		class DelegateContext<OV,NV>: BaseContext<OV,NV>
		{
			public Func<IServiceProvider,OV, Func<IServiceProvider, NV, Task>, Task> ContextCreator { get; set; }

			protected override Task OnRun(IServiceProvider ServiceProvider, OV PrevValue, Func<IServiceProvider,NV, Task> Callback)
			{
				return ContextCreator(ServiceProvider, PrevValue, Callback);
			}
		}
		
		public static ITestContext<NV> NewContext<OV,NV>(
			this ITestContext<OV> PrevContext,
			Func<IServiceProvider, OV, Func<IServiceProvider, NV, Task>, Task> ContextCreator
			) 
		{
			return new DelegateContext<OV, NV>
			{
				PrevContext = PrevContext,
				ContextCreator = ContextCreator
			};
		}
		public static async Task<R> Run<V,R>(
			this ITestContext<V> Scope,
			Func<IServiceProvider, V, Task<R>> Callback
			)
		{
			var re = default(R);
			await Scope.Exec(async (sp,v) =>
			{
				re = await Callback(sp,v);
			});
			return re;
		}
		public static Task<R> Run<V, R>(
				this ITestContext<V> Context,
				Func<V, Task<R>> Callback
				)
		{
			return Context.Run<V,R>((sp, v) => Callback(v));
		}
		public static Task Run<V>(
				this ITestContext<V> Context,
				Func<V, Task> Callback
				)
		{
			return Context.Exec(
				 (sp, v) =>
					 Callback(v)
			);
		}
		public static Task Run<V>(
				this ITestContext<V> Context,
				Func<IServiceProvider, V, Task> Callback
				)
		{
			return Context.Exec(
				 (sp, v) =>
					 Callback(sp,v)
			);
		}
	}
	

}
