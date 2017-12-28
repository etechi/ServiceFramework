using SF.Sys.Data;
using SF.Sys.Hosting;
using SF.Sys.Services;
using SF.Sys.TimeServices;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Sys.UnitTest
{
	public static class CommonTestContextExtension
	{
		
		public static ITestContext<V> SetTime<V>(this ITestContext<V> s,DateTime Time)
		{
			return s.NewContext<V,V>(
				async (sp, v, cb) =>
				{
					var ts = sp.Resolve<ITimeService>();
					var org = ts.UtcNow;
					ts.SetTime(Time.ToUtcTime());
					await cb(sp, v);
					ts.SetTime(org);
				});
		}

		public static ITestContext<V> NewScope<V>(this ITestContext<V> s)
		{
			return s.NewContext<V, V>(
				async (sp, v, cb) =>
					await sp.WithScope( 
						async isp =>
							await cb(isp, v)
						)
				);
		}
		public static ITestContext<IServiceProvider> ScopedTestContext(this IServiceProvider sp)
			=> sp.TestContext().NewScope();



	}


}
