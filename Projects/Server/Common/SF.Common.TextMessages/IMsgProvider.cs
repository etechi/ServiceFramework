using System;
using System.Threading.Tasks;

namespace SF.Common.TextMessages
{
	
	public class MsgSendArgument
	{
		public long MsgProviderId { get; set; }
		public long? TargetId { get; set; }
		public string Target { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public string Template { get; set; }
		public (string Key,string Value)[] Arguments { get; set; }

		public Message Message { get; set; }
	}
	/// <summary>
	/// 消息服务提供者
	/// </summary>
	public interface IMsgProvider
	{
		Task<string> TargetResolve(long TargetId);
		Task<string> Send(MsgSendArgument Argument);
	}
	public interface IDebugMsgProvider: IMsgProvider
	{
		MsgSendArgument LastArgument { get; }
	}
	public class MsgArgumentResult
	{
		public MsgSendArgument[] Args { get; set; }
		public long PolicyId { get; set; }
	}

	public interface IMsgArgumentFactory
	{
		Task<MsgArgumentResult> Create(long? TargetId, Message Message);
	}
}
