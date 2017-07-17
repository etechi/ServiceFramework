using SF.Core.ServiceManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.Logging
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
	[UnmanagedService]
	public interface ILogger
	{
		void Write(LogLevel logLevel, EventId EventId, Exception exception, string message);
		void Write(LogLevel logLevel, EventId EventId, Exception exception, string format,params object[] args);
		void Write<TState>(LogLevel logLevel, EventId EventId, TState state, Exception exception, Func<TState, Exception, string> formatter);
		bool IsEnabled(LogLevel level);
		IDisposable BeginScope<T>(T State);
	}
	[UnmanagedService]
	public interface ILogger<T> : ILogger
	{

	}
}
