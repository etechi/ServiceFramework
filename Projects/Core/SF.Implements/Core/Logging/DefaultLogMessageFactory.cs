using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using SF.Core.NetworkService.Metadata;
using System.Text;
using System.Threading.Tasks;
using SF.Core.ServiceFeatures;

namespace SF.Core.Logging
{
	public class DefaultLogMessageFactory : ILogMessageFactory
	{
		public static ILogMessageFactory Instance { get; } = new DefaultLogMessageFactory();

		DefaultLogMessageFactory()
		{ }

		public object CreateLogMessage(LogLevel level, Exception exception, string format, params object[] args)
		{
			if (args == null || args.Length == 0)
				return format;
			return string.Format(format, args);
		}
	}

}
