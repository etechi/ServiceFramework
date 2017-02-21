using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.Logging
{
	public interface ILogMessageFactory
	{
		object CreateLogMessage(LogLevel level, Exception exception, string format, params object[] args);
	}
	public interface ILogService : IDisposable
	{
		ILogger GetLogger(string categoryName);
		ILogger CreateScopedLogger(string categoryName);
		void AddProvider(ILoggerProvider provider);
		void RemoveProvider(ILoggerProvider provider);
		ILogMessageFactory LogMessageFactory { get; }
	}
}
