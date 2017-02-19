using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.Logging
{

	public interface ILogger
	{
		void Write<TState>(LogLevel logLevel, TState state, Exception exception, Func<TState, Exception, string> formatter);
		bool IsEnabled(LogLevel level);
		IDisposable BeginScope<T>(T State);
	}
	public interface ILogger<T> : ILogger
	{

	}
	public interface IDIScopeLogger<T> : ILogger
	{

	}
	
}
