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
using SF.Core.Logging.MicrosoftExtensions;

namespace SF.Core.Logging
{
	public static class MSLoggerLogServiceExtensions
	{
		class FakeLoggerFactory : Microsoft.Extensions.Logging.ILoggerFactory
		{
			public ILogService LogService { get; set; }
			public void AddProvider(Microsoft.Extensions.Logging.ILoggerProvider provider)
			{
				LogService.AddProvider(
					new MSLoggerProvider(provider)
					);
			}

			public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
			{
				throw new NotSupportedException();
			}

			public void Dispose()
			{
			}
		}

		public static Microsoft.Extensions.Logging.ILoggerFactory AsMSLoggerFactory(
			this ILogService ls
			)
		{
			return new FakeLoggerFactory { LogService = ls };
		}
	}
}
