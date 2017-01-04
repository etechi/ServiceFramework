using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Logging
{
	public static class LoggerExtension
	{
		public static void Write(this ILogger Logger, LogLevel Level, Exception exception, string Message)
		{
			Logger.Write(new LogRecord { Exception = exception, Level = Level, Message = Message });
		}
		public static void Write(this ILogger Logger, LogLevel Level, Exception exception, string Format, params object[] args)
		{
			Logger.Write(new LogRecord { Exception = exception, Level = Level, Message = Format, Arguments = args });
		}

		public static void Debug(this ILogger Logger, string Message)
		{
			Write(Logger, LogLevel.Trace, null, Message);
		}
		public static void Debug(this ILogger Logger, string Format, params object[] args)
		{
			Write(Logger, LogLevel.Trace, null, Format, args);
		}
		public static void Debug(this ILogger Logger, Exception exception, string Message)
		{
			Write(Logger, LogLevel.Trace, exception, Message);
		}
		public static void Debug(this ILogger Logger, Exception exception, string Format, params object[] args)
		{
			Write(Logger, LogLevel.Trace, exception, Format, args);
		}

		public static void Trace(this ILogger Logger, string Message)
		{
			Write(Logger, LogLevel.Trace, null, Message);
		}
		public static void Trace(this ILogger Logger, string Format, params object[] args)
		{
			Write(Logger, LogLevel.Trace, null, Format, args);
		}
		public static void Trace(this ILogger Logger, Exception exception, string Message)
		{
			Write(Logger, LogLevel.Trace, exception, Message);
		}
		public static void Trace(this ILogger Logger, Exception exception, string Format, params object[] args)
		{
			Write(Logger, LogLevel.Trace, exception, Format, args);
		}
		public static void Info(this ILogger Logger, string Message)
		{
			Write(Logger, LogLevel.Info, null, Message);
		}
		public static void Info(this ILogger Logger, string Format, params object[] args)
		{
			Write(Logger, LogLevel.Info, null, Format, args);
		}
		public static void Info(this ILogger Logger, Exception exception, string Message)
		{
			Write(Logger, LogLevel.Info, exception, Message);
		}
		public static void Info(this ILogger Logger, Exception exception, string Format, params object[] args)
		{
			Write(Logger, LogLevel.Info, exception, Format, args);
		}
		public static void Warn(this ILogger Logger, string Message)
		{
			Write(Logger, LogLevel.Warn, null, Message);
		}
		public static void Warn(this ILogger Logger, string Format, params object[] args)
		{
			Write(Logger, LogLevel.Warn, null, Format, args);
		}
		public static void Warn(this ILogger Logger, Exception exception, string Message)
		{
			Write(Logger, LogLevel.Warn, exception, Message);
		}
		public static void Warn(this ILogger Logger, Exception exception, string Format, params object[] args)
		{
			Write(Logger, LogLevel.Warn, exception, Format, args);
		}

		public static void Critical(this ILogger Logger, string Message)
		{
			Write(Logger, LogLevel.Critical, null, Message);
		}
		public static void Critical(this ILogger Logger, string Format, params object[] args)
		{
			Write(Logger, LogLevel.Critical, null, Format, args);
		}
		public static void Critical(this ILogger Logger, Exception exception, string Message)
		{
			Write(Logger, LogLevel.Critical, exception, Message);
		}
		public static void Critical(this ILogger Logger, Exception exception, string Format, params object[] args)
		{
			Write(Logger, LogLevel.Critical, exception, Format, args);
		}

		public static void Error(this ILogger Logger, string Message)
		{
			Write(Logger, LogLevel.Error, null, Message);
		}
		public static void Error(this ILogger Logger, string Format, params object[] args)
		{
			Write(Logger, LogLevel.Error, null, Format, args);
		}
		public static void Error(this ILogger Logger, Exception exception, string Message)
		{
			Write(Logger, LogLevel.Error, exception, Message);
		}
		public static void Error(this ILogger Logger, Exception exception, string Format, params object[] args)
		{
			Write(Logger, LogLevel.Error, exception, Format, args);
		}
	}
}
