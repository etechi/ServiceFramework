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
		IEnumerable<long> TargetIds { get; }
		IEnumerable<string> Targets { get; }
		IEnumerable<long> GroupIds { get; }
		IEnumerable<string> Groups { get; }
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
		Task<IEnumerable<string>> TargetResolve(IEnumerable<long> TargetIds);
		Task<IEnumerable<string>> GroupResolve(IEnumerable<long> GroupIds);
		Task<string> Send(ISendArgument Argument);
	}
	public interface IDebugNotificationSendProvider : INotificationSendProvider
	{
		ISendArgument LastArgument { get; }
	}
	
}
