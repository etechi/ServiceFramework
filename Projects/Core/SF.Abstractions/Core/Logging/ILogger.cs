using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.Logging
{

	public interface ILogger
	{
		void Write(LogLevel logLevel, Exception exception, string message);
		void Write(LogLevel logLevel, Exception exception, string format,params object[] args);
		void Write<TState>(LogLevel logLevel, TState state, Exception exception, Func<TState, Exception, string> formatter);
		bool IsEnabled(LogLevel level);
		IDisposable BeginScope<T>(T State);
	}
	public interface ILogger<T> : ILogger
	{

	}
}
