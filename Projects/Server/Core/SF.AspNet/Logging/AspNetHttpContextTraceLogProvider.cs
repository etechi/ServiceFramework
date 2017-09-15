using SF.Core.Hosting;
using SF.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace SF.Core.Logging
{
	public class AspNetHttpContextTraceLogProvider : ILoggerProvider
	{

		class Logger : IProviderLogger
		{
			ILoggerProvider IProviderLogger.Provider => Provider;
				
			public AspNetHttpContextTraceLogProvider Provider { get; }
			public TraceContext Trace { get; }
			public string Name { get; }
			public Logger(AspNetHttpContextTraceLogProvider Provider,string Name)
			{
				this.Provider = Provider;
				var ctx = HttpContext.Current;
				if (ctx == null)
					return ;
				Trace = new HttpContextWrapper(ctx).Trace;
				this.Name = Name;
			}
			public IDisposable BeginScope<T>(T State)
			{
				return Disposable.Empty;
			}

			public bool IsEnabled(LogLevel level)
			{
				return Trace!=null && Trace.IsEnabled && level>=Provider.Level;
			}

			public void Write<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
			{
				if (IsEnabled(logLevel))
				{
					if (logLevel >= LogLevel.Warn)
						Trace.Warn(Name, formatter(state, exception));
					else
						Trace.Write(Name, formatter(state, exception));
				}
			}
		}

		public bool Scoped => true;
		public LogLevel Level { get; }
		public AspNetHttpContextTraceLogProvider(LogLevel level)
		{
			this.Level = level;
		}


		public IProviderLogger CreateLogger(string categoryName)
		{
			return new Logger(this,categoryName);
		}

		public void Dispose()
		{
			
		}
	}
}
