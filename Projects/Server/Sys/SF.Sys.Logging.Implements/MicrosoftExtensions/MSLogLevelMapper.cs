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

namespace SF.Sys.Logging.MicrosoftExtensions
{
	public static class MSLogLevelMapper
	{
		public static Microsoft.Extensions.Logging.LogLevel MapLevel(LogLevel level)
		{
			switch (level)
			{
				case LogLevel.Trace: return Microsoft.Extensions.Logging.LogLevel.Trace;
				case LogLevel.Debug: return Microsoft.Extensions.Logging.LogLevel.Debug;
				case LogLevel.Info: return Microsoft.Extensions.Logging.LogLevel.Information;
				case LogLevel.Warn: return Microsoft.Extensions.Logging.LogLevel.Warning;
				case LogLevel.Error: return Microsoft.Extensions.Logging.LogLevel.Error;
				case LogLevel.Critical: return Microsoft.Extensions.Logging.LogLevel.Critical;
				case LogLevel.None: return Microsoft.Extensions.Logging.LogLevel.None;
				default:
					throw new NotSupportedException();
			}
		}
		public static LogLevel MapLevel(Microsoft.Extensions.Logging.LogLevel level)
		{
			switch (level)
			{
				case Microsoft.Extensions.Logging.LogLevel.Trace: return LogLevel.Trace;
				case Microsoft.Extensions.Logging.LogLevel.Debug: return LogLevel.Debug;
				case Microsoft.Extensions.Logging.LogLevel.Information: return LogLevel.Info;
				case Microsoft.Extensions.Logging.LogLevel.Warning: return LogLevel.Warn;
				case Microsoft.Extensions.Logging.LogLevel.Error: return LogLevel.Error;
				case Microsoft.Extensions.Logging.LogLevel.Critical: return LogLevel.Critical;
				case Microsoft.Extensions.Logging.LogLevel.None: return LogLevel.None;
				default:
					throw new NotSupportedException();
			}
		}
	}
	
}
