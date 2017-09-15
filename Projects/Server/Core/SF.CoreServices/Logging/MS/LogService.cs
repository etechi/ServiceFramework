using System;

namespace SF.Logging.MS
{
	public class Logger : ILogger
	{
		Microsoft.Extensions.Logging.ILogger InnerLogger { get; }
		public Logger(Microsoft.Extensions.Logging.ILogger InnerLogger)
		{
			this.InnerLogger = InnerLogger;
		}
		Microsoft.Extensions.Logging.LogLevel MapLevel(LogLevel level)
		{
			switch (level)
			{
				case LogLevel.Trace:return Microsoft.Extensions.Logging.LogLevel.Trace;
				case LogLevel.Debug: return Microsoft.Extensions.Logging.LogLevel.Debug;
				case LogLevel.Info: return Microsoft.Extensions.Logging.LogLevel.Information;
				case LogLevel.Warn: return Microsoft.Extensions.Logging.LogLevel.Warning;
				case LogLevel.Error: return Microsoft.Extensions.Logging.LogLevel.Error;
				case LogLevel.Critical: return Microsoft.Extensions.Logging.LogLevel.Critical;
				case LogLevel.None: return Microsoft.Extensions.Logging.LogLevel.None;
				default:
					throw new NotSupportedException();
			}
		}
		public bool IsEnabled(LogLevel level)
		{
			return InnerLogger.IsEnabled(MapLevel(level));
		}
		static string MessageFormatter(object state,Exception ex)
		{
			var r = (ILogRecord)state;
			return r.Arguments == null ? r.Message : string.Format(r.Message, r.Arguments);
		}
		static readonly Func<object, Exception, string> _messageFormatter = new Func<object, Exception, string>(MessageFormatter);

		public void Write(ILogRecord Record)
		{
			InnerLogger.Log(
				MapLevel(Record.Level),
				0,
				Record,
				Record.Exception,
				_messageFormatter
				);
		}
		public IDisposable BeginScope<T>(T State)
		{
			return InnerLogger.BeginScope(State);
		}
	}
	public class LogService :
		SF.Logging.ILogService
	{
		Microsoft.Extensions.Logging.ILoggerFactory LoggerFactory { get; }
		public LogService(Microsoft.Extensions.Logging.ILoggerFactory LoggerFactory)
		{
			this.LoggerFactory = LoggerFactory;
		}
		public ILogger GetLogger(string Name)
		{
			return new Logger(LoggerFactory.CreateLogger(Name));
		}
	}
}
