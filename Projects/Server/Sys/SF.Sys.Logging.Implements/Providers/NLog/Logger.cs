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

namespace SF.Sys.Logging.Providors.NLog
{
	partial class NLogProvider
	{
		class Logger : IProviderLogger
		{
			NLogProvider NLogProvider { get; }

			public ILoggerProvider Provider => NLogProvider;
			public global::NLog.Logger NLogger { get; }

			global::NLog.LogLevel MapLevel(LogLevel Level)
			{
				switch (Level)
				{
					case LogLevel.Critical:return global::NLog.LogLevel.Fatal;
					case LogLevel.Debug: return global::NLog.LogLevel.Debug;
					case LogLevel.Error: return global::NLog.LogLevel.Error;
					case LogLevel.Info: return global::NLog.LogLevel.Info;
					case LogLevel.None: return global::NLog.LogLevel.Off;
					case LogLevel.Trace: return global::NLog.LogLevel.Trace;
					case LogLevel.Warn: return global::NLog.LogLevel.Warn;
					default:return global::NLog.LogLevel.Info;
				}
			}
			public Logger(NLogProvider NLogProvider ,global::NLog.Logger Logger)
			{
				this.NLogProvider = NLogProvider;
				this.NLogger = Logger;
			}

			public void Write<TState>(LogLevel logLevel, EventId EventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
			{
				var ll = MapLevel(logLevel);
				if (NLogger.IsEnabled(ll))
				{
					if (exception == null)
						NLogger.Log(ll, "{0}", formatter(state, exception));
					else
						NLogger.Log(ll, "{0} {1}",formatter(state, exception), exception.ToString());
				}
			}
			public IDisposable BeginScope<T>(T State)
			{
				return Disposable.Empty;
			}

			public bool IsEnabled(LogLevel level)
			{
				return NLogger.IsEnabled(MapLevel(level));
			}
		}
	}
}
