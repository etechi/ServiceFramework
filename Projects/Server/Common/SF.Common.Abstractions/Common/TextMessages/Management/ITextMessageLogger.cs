using SF.Metadata;
using System.Threading.Tasks;

namespace SF.Common.TextMessages.Management
{
	[Comment(Name = "消息服务提供者")]
    public interface ITextMessageLogger
	{
		Task<long> BeginSend(long ServiceId,string Target, long? TargetUserId, Message message);
		Task EndSend(long MessageId, string ExtIdent, string Error);
	}
}
