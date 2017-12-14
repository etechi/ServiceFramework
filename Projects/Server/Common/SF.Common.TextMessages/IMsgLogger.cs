using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Sys.Services;
namespace SF.Common.TextMessages
{
	public interface IMsgActionLogger
	{
		Task<string> Add(
			MsgSendArgument Arg,
			Func<Task<string>> Action
			);
	}
	public interface IMsgLogger
	{
		Task<long> Add(
			long? DstUserId,
			Message Message,
			Func<IMsgActionLogger,Task<long>> Action
			);
	}

}
