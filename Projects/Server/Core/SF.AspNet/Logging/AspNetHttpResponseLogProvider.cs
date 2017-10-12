#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

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
