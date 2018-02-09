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
		
		public static IScope<IServiceProvider> SetTime(this IScope<IServiceProvider> s,DateTime Time)
		{
			return s.Wrap(async (sp, cb, ct) =>
			{
				var ts = sp.Resolve<ITimeService>();
				var org = ts.UtcNow;
				ts.SetTime(Time.ToUtcTime());
				await cb(sp,ct);
				ts.SetTime(org);
			});

		}

	}


}
