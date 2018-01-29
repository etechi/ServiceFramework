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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SF.Sys.Logging
{
	public static class LoggerExtension
	{

		public static void Write(this ILogger logger, LogLevel logLevel, Exception exception, string message)
			=> logger.Write(logLevel, EventId.None, exception, message);
		public static void Write(this ILogger logger, LogLevel logLevel, Exception exception, string format, params object[] args)
			=> logger.Write(logLevel, EventId.None, exception, format,args);
		public static void Write<TState>(this ILogger logger, LogLevel logLevel, TState state, Exception exception, Func<TState, Exception, string> formatter)
			=> logger.Write(logLevel, EventId.None, state, exception, formatter);

		[Conditional("DEBUG")]
		public static void Debug(this ILogger Logger, string Message)
		{
			Logger.Write(LogLevel.Debug, null, Message);
		}
		[Conditional("DEBUG")]
		public static void Debug(this ILogger Logger, string Format, params object[] args)
		{
			Logger.Write( LogLevel.Debug, null, Format, args);
		}
		[Conditional("DEBUG")]
		public static void Debug(this ILogger Logger, Exception exception, string Message)
		{
			Logger.Write( LogLevel.Debug, exception, Message);
		}
		[Conditional("DEBUG")]
		public static void Debug(this ILogger Logger, Exception exception, string Format, params object[] args)
		{
			Logger.Write( LogLevel.Debug, exception, Format, args);
		}

		public static void Trace(this ILogger Logger, string Message)
		{
			Logger.Write( LogLevel.Trace, null, Message);
		}
		public static void Trace(this ILogger Logger, string Format, params object[] args)
		{
			Logger.Write( LogLevel.Trace, null, Format, args);
		}
		public static void Trace(this ILogger Logger, Exception exception, string Message)
		{
			Logger.Write( LogLevel.Trace, exception, Message);
		}
		public static void Trace(this ILogger Logger, Exception exception, string Format, params object[] args)
		{
			Logger.Write( LogLevel.Trace, exception, Format, args);
		}
		public static void Info(this ILogger Logger, string Message)
		{
			Logger.Write( LogLevel.Info, null, Message);
		}
		public static void Info(this ILogger Logger, string Format, params object[] args)
		{
			Logger.Write( LogLevel.Info, null, Format, args);
		}
		public static void Info(this ILogger Logger, Exception exception, string Message)
		{
			Logger.Write( LogLevel.Info, exception, Message);
		}
		public static void Info(this ILogger Logger, Exception exception, string Format, params object[] args)
		{
			Logger.Write( LogLevel.Info, exception, Format, args);
		}
		public static void Warn(this ILogger Logger, string Message)
		{
			Logger.Write( LogLevel.Warn, null, Message);
		}
		public static void Warn(this ILogger Logger, string Format, params object[] args)
		{
			Logger.Write( LogLevel.Warn, null, Format, args);
		}
		public static void Warn(this ILogger Logger, Exception exception, string Message)
		{
			Logger.Write( LogLevel.Warn, exception, Message);
		}
		public static void Warn(this ILogger Logger, Exception exception, string Format, params object[] args)
		{
			Logger.Write( LogLevel.Warn, exception, Format, args);
		}

		public static void Critical(this ILogger Logger, string Message)
		{
			Logger.Write( LogLevel.Critical, null, Message);
		}
		public static void Critical(this ILogger Logger, string Format, params object[] args)
		{
			Logger.Write( LogLevel.Critical, null, Format, args);
		}
		public static void Critical(this ILogger Logger, Exception exception, string Message)
		{
			Logger.Write( LogLevel.Critical, exception, Message);
		}
		public static void Critical(this ILogger Logger, Exception exception, string Format, params object[] args)
		{
			Logger.Write( LogLevel.Critical, exception, Format, args);
		}

		public static void Error(this ILogger Logger, string Message)
		{
			Logger.Write( LogLevel.Error, null, Message);
		}
		public static void Error(this ILogger Logger, string Format, params object[] args)
		{
			Logger.Write( LogLevel.Error, null, Format, args);
		}
		public static void Error(this ILogger Logger, Exception exception, string Message)
		{
			Logger.Write( LogLevel.Error, exception, Message);
		}
		public static void Error(this ILogger Logger, Exception exception, string Format, params object[] args)
		{
			Logger.Write( LogLevel.Error, exception, Format, args);
		}
	}
}
