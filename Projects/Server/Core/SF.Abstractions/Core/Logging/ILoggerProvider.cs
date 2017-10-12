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
using System.Collections.Generic;
using System.Text;

namespace SF.Core.Logging
{

	public interface IProviderLogger 
	{
		ILoggerProvider Provider { get; }
		void Write<TState>(LogLevel logLevel, EventId EventId, TState state, Exception exception, Func<TState, Exception, string> formatter);
		IDisposable BeginScope<T>(T State);
		bool IsEnabled(LogLevel level);
	}
	public interface ILoggerProvider : IDisposable
	{
		bool Scoped { get; }
		IProviderLogger CreateLogger(string categoryName);
	}
}
