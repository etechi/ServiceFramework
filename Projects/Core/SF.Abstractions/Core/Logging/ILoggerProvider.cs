using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.Logging
{

	public interface IProviderLogger : ILogger
	{
		ILoggerProvider Provider { get; }

	}
	public interface ILoggerProvider : IDisposable
	{
		IProviderLogger CreateLogger(string categoryName);
	}
}
