using SF.Metadata;
using System.Threading.Tasks;

namespace SF.Common.TextMessages
{
	//public class MessageSendResult
	//{
	//	public MessageSendResult(string Target, string Result, Exception Exception)
	//	{
	//		this.Target = Target;
	//		this.Result = Result;
	//		this.Exception = Exception;
	//	}
	//	public string Target { get; }
	//	public string Result { get; }
	//	public Exception Exception { get; }
	//}
	//public class MessageSendFailedException : PublicException
	//{
	//	public MessageSendResult[] Results { get; }
	//	public MessageSendFailedException(string message, MessageSendResult[] results, Exception innerException) : base(message, innerException)
	//	{
	//		Results = results;
	//	}
	//}
	[Comment(Name = "文本消息服务")]
    public interface ITextMessageService
	{
		Task<long> Send(string target, Message message);
	}
	//public interface IMsgBatchProvider :IMsgProvider
	//{
	//	Task<MessageSendResult[]> Send(string[] targets, Message message);
 //       int MaxBatchCount { get; }
	//}
}
