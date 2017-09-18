﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.Logging
{
	public static class LoggerExtension
	{

		public static void Write(this ILogger logger, LogLevel logLevel, Exception exception, string message)
			=> logger.Write(logLevel, EventId.None, exception, message);
		public static void Write(this ILogger logger, LogLevel logLevel, Exception exception, string format, params object[] args)
			=> logger.Write(logLevel, EventId.None, exception, format,args);
		public static void Write<TState>(this ILogger logger, LogLevel logLevel, TState state, Exception exception, Func<TState, Exception, string> formatter)
			=> logger.Write(logLevel, EventId.None, state, exception, formatter);

		public static void Debug(this ILogger Logger, string Message)
		{
			Logger.Write(LogLevel.Trace, null, Message);
		}
		public static void Debug(this ILogger Logger, string Format, params object[] args)
		{
			Logger.Write( LogLevel.Trace, null, Format, args);
		}
		public static void Debug(this ILogger Logger, Exception exception, string Message)
		{
			Logger.Write( LogLevel.Trace, exception, Message);
		}
		public static void Debug(this ILogger Logger, Exception exception, string Format, params object[] args)
		{
			Logger.Write( LogLevel.Trace, exception, Format, args);
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