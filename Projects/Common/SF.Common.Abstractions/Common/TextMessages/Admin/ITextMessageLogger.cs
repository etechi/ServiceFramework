using SF.Metadata;
using System.Threading.Tasks;

namespace SF.System.TextMessages
{
	[Comment(Name = "消息服务提供者")]
    public interface ITextMessageLogger
	{
		Task<long> BeginSend(string ServiceInstance,string Target, Message message);
		Task EndSend(long MessageId, string ExtIdent, string Error);
	}
}
