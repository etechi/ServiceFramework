using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.Logging
{
	public enum LogLevel
	{
		Trace,
		Debug,
		Info,
		Warn,
		Error,
		Critical,
		None
	}

	public interface ILogger
	{
		void Write(ILogRecord Record);
		bool IsEnabled(LogLevel level);
		IDisposable BeginScope<T>(T State);
	}
	public interface ILogger<T> : ILogger
	{

	}
	public interface ILogRecord
	{
		LogLevel Level { get; }
		Exception Exception { get; }
		string Message { get; }
		object[] Arguments { get; }
	}
	public class LogRecord : ILogRecord
	{
		public LogLevel Level { get; set; }
		public Exception Exception { get; set; }
		public string Message { get; set; }
		public object[] Arguments { get; set; }
	}

	public class Logger<T> : ILogger<T>
	{
		ILogger InnerLogger { get; }
		public Logger(ILogService Service){
			this.InnerLogger = Service.GetLogger(typeof(T).FullName);
		}
		bool ILogger.IsEnabled(LogLevel level)
		{
			return InnerLogger.IsEnabled(level);
		}

		void ILogger.Write(ILogRecord Record)
		{
			InnerLogger.Write(Record);
		}

		IDisposable ILogger.BeginScope<T1>(T1 State)
		{
			return InnerLogger.BeginScope(State);
		}
	}
}
