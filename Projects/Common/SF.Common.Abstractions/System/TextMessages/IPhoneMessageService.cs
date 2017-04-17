using SF.Metadata;
using System.Threading.Tasks;

namespace SF.System.TextMessages
{
	[Comment(Name = "手机消息服务")]
    public interface IPhoneMessageService : ITextMessageService
	{
	}
	//public interface IMsgBatchProvider :IMsgProvider
	//{
	//	Task<MessageSendResult[]> Send(string[] targets, Message message);
 //       int MaxBatchCount { get; }
	//}
}
