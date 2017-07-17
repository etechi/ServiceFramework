﻿using SF.Core.ServiceManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.Logging
{
	[UnmanagedService]
	public interface ILogMessageFactory
	{
		object CreateLogMessage(LogLevel level, Exception exception, string format, params object[] args);
	}
	[UnmanagedService]
	public interface ILogService : IDisposable
	{
		ILogger GetLogger(string categoryName);
		ILogger CreateScopedLogger(string categoryName);
		void AddProvider(ILoggerProvider provider);
		void RemoveProvider(ILoggerProvider provider);
		ILogMessageFactory LogMessageFactory { get; }
	}
}
