using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Sys.Services;
namespace SF.Common.TextMessages
{
	
	public interface IMsgLogger
	{
		Task<object> PreSend(IServiceInstanceDescriptor Service, Message message,long? targetUserId,string[] targets);
		Task PostSend(object Context,IEnumerable<MessageSendResult> results,Exception error);
	}

}
