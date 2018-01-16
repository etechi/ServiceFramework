﻿using SF.Sys.Linq;
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

		public Task<IEnumerable<string>> GroupResolve(IEnumerable<long> GroupIds)
		{
			return Task.FromResult(GroupIds.Select(i => i.ToString()));
		}
		public Task<IEnumerable<string>> TargetResolve(IEnumerable<long> TargetIds)
		{
			return Task.FromResult(TargetIds.Select(i => i.ToString()));
		}
	}

}