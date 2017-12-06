using System;
using System.Threading.Tasks;

namespace SF.Common.TextMessages
{
	
	/// <summary>
	/// 消息服务提供者
	/// </summary>
	public interface IMsgProvider
	{
		Task<string> Send(string target, Message message);
	}
	public interface IMsgBatchProvider :IMsgProvider
	{
		Task<MessageSendResult[]> Send(string[] targets, Message message);
        int MaxBatchCount { get; }
	}
}
