using System;

namespace SF.Core.Logging.MicrosoftExtensions
{
	public class Logger : IProviderLogger
	{
		Microsoft.Extensions.Logging.ILogger InnerLogger { get; }

		public ILoggerProvider Provider { get; }
		

		public Logger(ILoggerProvider Provider ,Microsoft.Extensions.Logging.ILogger InnerLogger)
		{
			this.Provider = Provider;
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

		public IDisposable BeginScope<T>(T State)
		{
			return InnerLogger.BeginScope(State);
		}

		public void Write<TState>(LogLevel logLevel, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			InnerLogger.Log(
				MapLevel(logLevel),
				default(Microsoft.Extensions.Logging.EventId),
				state,
				exception,
				formatter
				);

		}
	}
	public class LoggerProvider :
		SF.Core.Logging.ILoggerProvider
	{
		Microsoft.Extensions.Logging.ILoggerFactory LoggerFactory { get; }
		public LoggerProvider(Microsoft.Extensions.Logging.ILoggerFactory LoggerFactory)
		{
			this.LoggerFactory = LoggerFactory;
		}
		public IProviderLogger CreateLogger(string Name)
		{
			return new Logger(this,LoggerFactory.CreateLogger(Name));
		}

		public void Dispose()
		{

		}
	}
}
