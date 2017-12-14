using SF.Sys.Linq;
using SF.Sys.Logging;
using SF.Sys.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Common.TextMessages
{

	public class DebugMsgProvider : IDebugMsgProvider
	{
		public MsgSendArgument LastArgument { get; set; }

		public Task<string> Send(MsgSendArgument Argument)
		{
			LastArgument = Argument;
			return Task.FromResult(Guid.NewGuid().ToString("N"));
		}

		public Task<string> TargetResolve(long TargetId)
		{
			return Task.FromResult(TargetId.ToString());
		}
	}

}
