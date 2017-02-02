using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.Logging
{
	public interface ILogService
	{
		ILogger GetLogger(string Name);
	}
}
