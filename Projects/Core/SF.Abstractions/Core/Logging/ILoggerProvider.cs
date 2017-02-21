using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.Logging
{

	public interface IProviderLogger 
	{
		ILoggerProvider Provider { get; }
		void Write<TState>(LogLevel logLevel, TState state, Exception exception, Func<TState, Exception, string> formatter);
		IDisposable BeginScope<T>(T State);
		bool IsEnabled(LogLevel level);
	}
	public interface ILoggerProvider : IDisposable
	{
		bool Scoped { get; }
		IProviderLogger CreateLogger(string categoryName);
	}
}
