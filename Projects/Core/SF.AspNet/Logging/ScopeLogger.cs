using SF.Core.DI;
using SF.Core.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SF.Core.Logging
{
	public class AspNetDIScopeLogger<T> : IDIScopeLogger<T>
	{
		ILogger<T> Logger { get; }
		public AspNetDIScopeLogger(ILogger<T> Logger)
		{
			this.Logger = Logger;
		}
		public IDisposable BeginScope<S>(S State)
		{
			return Logger.BeginScope<S>(State);
		}

		public bool IsEnabled(LogLevel level)
		{
			return Logger.IsEnabled(level);
		}

		public void Write<TState>(LogLevel logLevel, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			Logger.Write(logLevel, state, exception, formatter);
		}
	}
}
