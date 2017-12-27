using SF.Sys.Data;
using SF.Sys.Hosting;
using SF.Sys.Services;
using SF.Sys.TimeServices;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Sys.UnitTest
{
	public interface ITestScope<V>
	{
		Task Exec(Func<IServiceProvider, V, Task> Callback);
	}
	public static class TestScopeExtension
	{
		class AppInstanceBuilderRootTestScope : ITestScope<IServiceProvider>
		{
			public IAppInstanceBuilder AppInstanceBuilder { get; set; }
			public async Task Exec(Func<IServiceProvider, IServiceProvider, Task> Callback)
			{
				using (var app = AppInstanceBuilder.Build())
				{
					await Callback(app.ServiceProvider, app.ServiceProvider);
				}
			}
		}
		public static ITestScope<IServiceProvider> TestScope(this IAppInstanceBuilder builder)
		{
			return new AppInstanceBuilderRootTestScope
			{
				AppInstanceBuilder = builder
			};
		}

		class ServiceProviderRootTestScope : ITestScope<IServiceProvider>
		{
			public IServiceProvider ServiceProvider { get; set; }
			public Task Exec(Func<IServiceProvider, IServiceProvider, Task> Callback)
			{
				return Callback(ServiceProvider, ServiceProvider);
			}
		}
		public static ITestScope<IServiceProvider> TestScope(this IServiceProvider sp)
			=> new ServiceProviderRootTestScope { ServiceProvider = sp };

		abstract class BaseScope<OV,NV> : ITestScope<NV>
		{
			public ITestScope<OV> PrevScope { get; set; }
			public Task Exec(Func<IServiceProvider, NV, Task> Callback)
			{
				return PrevScope.Exec(async (sp,ov) =>
				{
					await OnRun(sp, ov, Callback);
				});
			}
			protected abstract Task OnRun(IServiceProvider ServiceProvider, OV PrevValue, Func<IServiceProvider, NV, Task> Callback);
		}
		class DelegateScope<OV,NV>: BaseScope<OV,NV>
		{
			public Func<IServiceProvider,OV, Func<IServiceProvider, NV, Task>, Task> NewScope { get; set; }

			protected override Task OnRun(IServiceProvider ServiceProvider, OV PrevValue, Func<IServiceProvider,NV, Task> Callback)
			{
				return NewScope(ServiceProvider, PrevValue, Callback);
			}
		}
		
		public static ITestScope<NV> CreateNewScope<OV,NV>(
			this ITestScope<OV> PrevScope,
			Func<IServiceProvider, OV, Func<IServiceProvider, NV, Task>, Task> NewScope
			) 
		{
			return new DelegateScope<OV, NV>
			{
				PrevScope = PrevScope,
				NewScope = NewScope
			};
		}
		public static async Task<R> Run<V,R>(
			this ITestScope<V> Scope,
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
				this ITestScope<V> Scope,
				Func<V, Task<R>> Callback
				)
		{
			return Scope.Run<V,R>((sp, v) => Callback(v));
		}
		public static Task Run<V>(
				this ITestScope<V> Scope,
				Func<V, Task> Callback
				)
		{
			return Scope.Exec(
				 (sp, v) =>
					 Callback(v)
			);
		}
		public static Task Run<V>(
				this ITestScope<V> Scope,
				Func<IServiceProvider, V, Task> Callback
				)
		{
			return Scope.Exec(
				 (sp, v) =>
					 Callback(sp,v)
			);
		}
	}
	

}
