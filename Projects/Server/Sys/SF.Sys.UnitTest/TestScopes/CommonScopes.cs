using SF.Sys.Data;
using SF.Sys.Hosting;
using SF.Sys.Services;
using SF.Sys.TimeServices;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Sys.UnitTest
{
	public static class CommonTestScopeExtension
	{
		
		public static ITestScope<V> SetTime<V>(this ITestScope<V> s,DateTime Time)
		{
			return s.CreateNewScope<V,V>(
				async (sp, v, cb) =>
				{
					var ts = sp.Resolve<ITimeService>();
					var org = ts.UtcNow;
					ts.SetTime(Time.ToUtcTime());
					await cb(sp, v);
					ts.SetTime(org);
				});
		}

		public static ITestScope<V> ServiceScope<V>(this ITestScope<V> s)
		{
			return s.CreateNewScope<V, V>(
				async (sp, v, cb) =>
					await sp.WithScope( 
						async isp =>
							await cb(isp, v)
						)
				);
		}
		public static ITestScope<IServiceProvider> ServiceTestScope(this IServiceProvider sp)
			=> sp.TestScope().ServiceScope();



	}


}
