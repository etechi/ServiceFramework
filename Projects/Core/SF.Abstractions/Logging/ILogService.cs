using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Logging
{
	public interface ILogService
	{
		ILogger GetLogger(string Name);
	}
}
