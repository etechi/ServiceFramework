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

using Microsoft.Extensions.Logging;
using SF.Sys.Comments;
using System;

namespace SF.Sys.Logging
{
	class Logger<T> : ILogger<T>,Microsoft.Extensions.Logging.ILogger<T>, IDisposable
	{
		ILogger GlobalLogger { get; }
		ILogger ScopedLogger { get; }
		ILogService LogService { get; }
		public Logger(ILogService LogService)
		{
			var name = typeof(T).Comment().Title;
			this.LogService = LogService;
			GlobalLogger = LogService.GetLogger(name);
			ScopedLogger = LogService.CreateScopedLogger(name);
		}

		public void Dispose()
		{
			var d = ScopedLogger as IDisposable;
			if (d != null)
				d.Dispose();
		}
		static Func<object, Exception, string> _formatter { get; } = new Func<object, Exception, string>((o, e) => o.ToString());
		public void Write(LogLevel logLevel, EventId eventId, Exception exception, string message)
		{
			var re = LogService.LogMessageFactory.CreateLogMessage(logLevel, exception, message, null);
			GlobalLogger.Write(logLevel, eventId,re, exception, _formatter);
			ScopedLogger.Write(logLevel, eventId, re, exception, _formatter);
		}
		public void Write(LogLevel logLevel, EventId eventId, Exception exception, string format, params object[] args)
		{
			var re = LogService.LogMessageFactory.CreateLogMessage(logLevel, exception, format, args);
			GlobalLogger.Write(logLevel, eventId, re, exception, _formatter);
			ScopedLogger.Write(logLevel, eventId, re, exception, _formatter);
		}
		public void Write<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			GlobalLogger.Write(logLevel, eventId, state, exception, formatter);
			ScopedLogger.Write(logLevel, eventId, state, exception, formatter);
		}

		public bool IsEnabled(LogLevel level)
		{
			return GlobalLogger.IsEnabled(level) || ScopedLogger.IsEnabled(level);
		}

		public IDisposable BeginScope<T1>(T1 State)
		{
			return Disposable.Combine(
				GlobalLogger.BeginScope(State),
				ScopedLogger.BeginScope(State)
				);
		}
		
		void Microsoft.Extensions.Logging.ILogger.Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			Write(LoggerCollection.MapLevel(logLevel), new EventId(eventId.Id, eventId.Name), state, exception, formatter);
		}

		bool Microsoft.Extensions.Logging.ILogger.IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
		{
			return IsEnabled(LoggerCollection.MapLevel(logLevel));
		}

		IDisposable Microsoft.Extensions.Logging.ILogger.BeginScope<TState>(TState state)
		{
			return BeginScope(state);
		}
	}

}
