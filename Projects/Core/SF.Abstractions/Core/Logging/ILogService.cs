using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.Logging
{
	public interface ILogService : IDisposable
	{
		ILogger GetLogger(string categoryName);
		void AddProvider(ILoggerProvider provider);
		void RemoveProvider(ILoggerProvider provider);
	}

}
