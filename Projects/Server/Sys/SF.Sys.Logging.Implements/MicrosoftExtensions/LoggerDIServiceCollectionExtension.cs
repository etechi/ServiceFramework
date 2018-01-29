#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;
using SF.Sys.Logging;
using Microsoft.Extensions.Logging;
using SF.Sys.Logging.MicrosoftExtensions;

namespace SF.Sys.Logging
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
					InnerLogger.Write(MapLevel(logLevel),new SF.Sys.Logging.EventId(eventId.Id,eventId.Name), state, exception, formatter);
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
		public static ILogService AddDebug(this ILogService svc,LogLevel LogLevel=LogLevel.Warn)
		{
			svc.AsMSLoggerFactory().AddDebug(MSLogLevelMapper.MapLevel(LogLevel));
			return svc;
		}
		public static ILogService AddConsole(this ILogService svc, LogLevel LogLevel = LogLevel.Warn)
		{
			svc.AsMSLoggerFactory().AddConsole(MSLogLevelMapper.MapLevel(LogLevel));
			return svc;
		}
	}
}
