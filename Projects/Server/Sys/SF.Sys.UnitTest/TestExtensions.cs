using SF.Sys.Data;
using SF.Sys.Hosting;
using SF.Sys.Services;
using SF.Sys.TimeServices;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Sys.UnitTest
{
	public static class TestExtensions
	{
		
		public static DateTime Now(this IServiceProvider sp)
			=> sp.Resolve<ITimeService>().Now;


		public static IScope<IAppInstance> NewAppInstance(this IAppInstanceBuilder AppInstanceBuilder) 
			=> Scope.From(AppInstanceBuilder).Using(b => b.Build());


		public static IScope<IServiceProvider> NewServiceScope(this IAppInstanceBuilder AppInstanceBuilder)
			=> (from ai in AppInstanceBuilder.NewAppInstance()
				select ai.ServiceProvider
				).NewServiceScope();

		public static IScope<IDataContext> NewDataContext(this IScope<IServiceProvider> scope)
			=> scope.Convert<IServiceProvider, IDataContext>(
				(sp, cb, ct) => sp.Resolve<IDataScope>().Use("����", ctx => cb(ctx, ct))
				);


		public static async Task<long> GetIdent(this IServiceProvider sp)
		{
			var ig = sp.Resolve<IIdentGenerator>();
			return await ig.GenerateAsync("�������");
		}
	}
	

}
