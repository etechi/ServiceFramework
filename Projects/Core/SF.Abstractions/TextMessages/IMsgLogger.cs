using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.TextMessages
{
	
	public interface IMsgLogger
	{
		Task<object> PreSend(DI.ISysService Service, Message message,int? targetUserId,string[] targets);
		Task PostSend(object Context,IEnumerable<MessageSendResult> results,Exception error);
	}

}
