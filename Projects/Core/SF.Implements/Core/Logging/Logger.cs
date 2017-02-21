using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using SF.Core.NetworkService.Metadata;
using System.Text;
using System.Threading.Tasks;
using SF.Core.ServiceFeatures;

namespace SF.Core.Logging
{
	class Logger<T> : ILogger<T>, IDisposable
	{
		ILogger GlobalLogger { get; }
		ILogger ScopedLogger { get; }
		ILogService LogService { get; }
		public Logger(ILogService LogService)
		{
			var name = typeof(T).Comment().Name;
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
		public void Write(LogLevel logLevel, Exception exception, string message)
		{
			var re = LogService.LogMessageFactory.CreateLogMessage(logLevel, exception, message, null);
			GlobalLogger.Write(logLevel, re, exception, _formatter);
			ScopedLogger.Write(logLevel, re, exception, _formatter);
		}
		public void Write(LogLevel logLevel, Exception exception, string format, params object[] args)
		{
			var re = LogService.LogMessageFactory.CreateLogMessage(logLevel, exception, format, args);
			GlobalLogger.Write(logLevel, re, exception, _formatter);
			ScopedLogger.Write(logLevel, re, exception, _formatter);
		}
		public void Write<TState>(LogLevel logLevel, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			GlobalLogger.Write(logLevel, state, exception, formatter);
			ScopedLogger.Write(logLevel, state, exception, formatter);
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

	}

}
