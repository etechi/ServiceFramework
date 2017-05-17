﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SF.Core.Logging;
using Microsoft.Extensions.Logging;
using SF.Core.Logging.MicrosoftExtensions;

namespace SF.Core.Logging
{
	public static class MSLoggerLogServiceExtensions
	{
		class FakeLoggerFactory : Microsoft.Extensions.Logging.ILoggerFactory
		{
			class Logger : Microsoft.Extensions.Logging.ILogger
			{
				ILogger InnerLogger { get; }
				public Logger(ILogger InnerLogger)
				{
					this.InnerLogger = InnerLogger;
				}
				public IDisposable BeginScope<TState>(TState state)
				{
					return InnerLogger.BeginScope(state);
				}
				LogLevel MapLevel( Microsoft.Extensions.Logging.LogLevel level)
				{
					switch (level)
					{
						case Microsoft.Extensions.Logging.LogLevel.Critical:return LogLevel.Critical;
						case Microsoft.Extensions.Logging.LogLevel.Debug: return LogLevel.Debug;
						case Microsoft.Extensions.Logging.LogLevel.Error: return LogLevel.Error;
						case Microsoft.Extensions.Logging.LogLevel.Information: return LogLevel.Info;
						case Microsoft.Extensions.Logging.LogLevel.None: return LogLevel.None;
						case Microsoft.Extensions.Logging.LogLevel.Trace: return LogLevel.Trace;
						case Microsoft.Extensions.Logging.LogLevel.Warning: return LogLevel.Warn;
						default:
							throw new NotSupportedException();
					}
				}
				public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
				{
					return InnerLogger.IsEnabled(MapLevel(logLevel));
				}

				public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
				{
					InnerLogger.Write(MapLevel(logLevel),new SF.Core.Logging.EventId(eventId.Id,eventId.Name), state, exception, formatter);
				}
			}

			public ILogService LogService { get; set; }
			public void AddProvider(Microsoft.Extensions.Logging.ILoggerProvider provider)
			{
				LogService.AddProvider(
					new MSLoggerProvider(provider)
					);
			}

			public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
			{
				return new Logger(LogService.GetLogger(categoryName));
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
