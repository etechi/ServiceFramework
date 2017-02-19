using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SF.Core.Logging;
using SF.Core.DI;
using Microsoft.Extensions.Logging;

namespace SF.Core.Logging.MicrosoftExtensions
{
	public static class MSLoggerProviderExtensions
	{
		public static IServiceProvider AddMSLogging(this IServiceProvider sp)
		{
			return sp.AddMSLogging(
					sp.Resolve<Microsoft.Extensions.Logging.ILoggerFactory>()
					);
		}
		public static IServiceProvider AddMSLogging(
			this IServiceProvider sp,
			Microsoft.Extensions.Logging.ILoggerFactory LoggingFactory
			)
		{
			LoggingFactory.AddConsole();
			LoggingFactory.AddDebug();

			var logService = sp.Resolve<ILogService>();
			logService.AddProvider(
				new LoggerProvider(
					LoggingFactory
					)
				);
			return sp;
		}
	}
}
