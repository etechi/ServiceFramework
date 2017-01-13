using ServiceProtocol.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.TextMessages
{
	public class MessageSendResult
	{
		public MessageSendResult(string Target, string Result, Exception Exception)
		{
			this.Target = Target;
			this.Result = Result;
			this.Exception = Exception;
		}
		public string Target { get; }
		public string Result { get; }
		public Exception Exception { get; }
	}
	public class MessageSendFailedException : PublicException
	{
		public MessageSendResult[] Results { get; }
		public MessageSendFailedException(string message, MessageSendResult[] results, Exception innerException) : base(message, innerException)
		{
			Results = results;
		}
	}
    [TypeDisplay(Name = "消息服务提供者")]
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
