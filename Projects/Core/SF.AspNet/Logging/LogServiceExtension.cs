using SF.Core.DI;
using SF.Core.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace SF.Core.Logging
{
	public static class AspNetLogServiceExtension
	{
		public static ILogService AddAspNetTrace(this ILogService Service,LogLevel level=LogLevel.Trace)
		{
			Service.AddProvider(new AspNetHttpContextTraceLogProvider(level));
			return Service;
		}
	}
}
