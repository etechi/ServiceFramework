using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Common.Notifications.Senders
{
	
	public interface ISendArgument
	{
		long Id { get; }
		long NotificationId { get;  }
		long ProviderId { get; }
		long? TargetId { get; }
		string Target { get; }
		string Title { get; }
		string Content { get; }
		string Template { get; }
		IReadOnlyDictionary<string,string> GetArguments();
	}
	/// <summary>
	/// 通知发送提供者
	/// </summary>
	public interface INotificationSendProvider
	{
		Task<string> TargetResolve(long TargetId);
		Task<string> Send(ISendArgument Argument);
	}
	public interface IDebugNotificationSendProvider : INotificationSendProvider
	{
		ISendArgument LastArgument { get; }
	}
	
}
