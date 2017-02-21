using System;
using System.Collections.Generic;

namespace SF.Core.Logging.MicrosoftExtensions
{
	public class MSLogMessageFactory : ILogMessageFactory
	{
		public object CreateLogMessage(LogLevel level, Exception exception, string format, params object[] args)
		{
			return new Microsoft.Extensions.Logging.Internal.FormattedLogValues(format, args);
		}
	}
}
