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

namespace SF.Sys.Logging
{
	public struct EventId
	{
		public int Id { get; }
		public string Name { get; }
		public static EventId None { get; } = new EventId(0);
		public EventId(int Id,string Name = null)
		{
			this.Id = Id;
			this.Name = Name;
		}
		public override string ToString()
		{
			if (Name == null)
				return Id.ToString();
			return Id + ":" + Name;
		}
		public static implicit operator EventId(int Id)
		{
			return new EventId(Id);
		}
	}
	public interface ILogger
	{
		void Write(LogLevel logLevel, EventId EventId, Exception exception, string message);
		void Write(LogLevel logLevel, EventId EventId, Exception exception, string format,params object[] args);
		void Write<TState>(LogLevel logLevel, EventId EventId, TState state, Exception exception, Func<TState, Exception, string> formatter);
		bool IsEnabled(LogLevel level);
		IDisposable BeginScope<T>(T State);
	}
	public interface ILogger<T> : ILogger
	{

	}
}
