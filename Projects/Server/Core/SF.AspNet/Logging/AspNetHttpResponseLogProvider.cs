using SF.Core.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace SF.Core.Logging
{
	//class HttpResponseLogger : IProviderLogger
	//{
	//	public ILoggerProvider Provider => null;
	//	public LogLevel Level { get; }
	//	public System.IO.TextWriter Writer { get; }
	//	public HttpResponseLogger(LogLevel Level)
	//	{
	//		this.Level = Level;
	//		var ctx = HttpContext.Current;
	//		if (ctx != null)
	//			Writer = ctx.Response.Output;
	//	}
	//	public IDisposable BeginScope<T>(T State)
	//	{
	//		return Disposable.Empty;
	//	}

	//	public bool IsEnabled(LogLevel level)
	//	{
	//		return Writer!=null && level >= this.Level;
	//	}

	//	public void Write<TState>(LogLevel logLevel, TState state, Exception exception, Func<TState, Exception, string> formatter)
	//	{
	//		if (IsEnabled(logLevel))
	//		{
	//			Writer.Write(formatter(state, exception));
	//			if (exception != null)
	//				Writer.Write(exception.ToString());
	//			Writer.Flush();
	//		}
	//	}
	//}
}
