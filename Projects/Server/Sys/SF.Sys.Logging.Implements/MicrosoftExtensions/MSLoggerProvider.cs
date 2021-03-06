﻿#region Apache License Version 2.0
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
	public class Logger : IProviderLogger
	{
		Microsoft.Extensions.Logging.ILogger InnerLogger { get; }

		public ILoggerProvider Provider { get; }
		

		public Logger(ILoggerProvider Provider ,Microsoft.Extensions.Logging.ILogger InnerLogger)
		{
			this.Provider = Provider;
			this.InnerLogger = InnerLogger;
		}
		
		public bool IsEnabled(LogLevel level)
		{
			return InnerLogger.IsEnabled(MSLogLevelMapper.MapLevel(level));
		}

		public IDisposable BeginScope<T>(T State)
		{
			return InnerLogger.BeginScope(State);
		}

		public void Write<TState>(LogLevel logLevel, SF.Sys.Logging.EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			InnerLogger.Log(
				MSLogLevelMapper.MapLevel(logLevel),
				new Microsoft.Extensions.Logging.EventId(eventId.Id,eventId.Name),
				state,
				exception,
				formatter
				);

		}
	}
	public class MSLoggerProvider :
		SF.Sys.Logging.ILoggerProvider
	{
		Microsoft.Extensions.Logging.ILoggerProvider MSProvider { get; }

		public bool Scoped => false;

		public MSLoggerProvider(Microsoft.Extensions.Logging.ILoggerProvider MSProvider)
		{
			this.MSProvider = MSProvider;
		}
		public IProviderLogger CreateLogger(string Name)
		{
			return new Logger(this, MSProvider.CreateLogger(Name));
		}

		public void Dispose()
		{
			MSProvider.Dispose();
		}
	}
}
