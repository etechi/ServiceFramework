using SF.Sys.Linq;
using SF.Sys.Logging;
using SF.Sys.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Common.Notifications.Senders
{

	public class DebugNotificationSendProvider : IDebugNotificationSendProvider
	{
		public ISendArgument LastArgument { get; set; }

		public Task<string> Send(ISendArgument Argument)
		{
			LastArgument = Argument;
			return Task.FromResult("N"+Argument.Id.ToString());
		}

		public Task<string> TargetResolve(long TargetId)
		{
			return Task.FromResult(TargetId.ToString());
		}
	}

}
