using SF.Metadata;
using System.Threading.Tasks;
using System;
using SF.Core.ManagedServices;

namespace SF.Services.TextMessages
{
	[Comment(Name = "模拟手机消息服务提供者")]
	public class SimPhoneMessageService : IPhoneMessageService
	{
		ITextMessageLogger Logger { get; }
		Option<IServiceInstanceIdent> InstanceIdent { get; }
		public SimPhoneMessageService(ITextMessageLogger Logger, Option<IServiceInstanceIdent> InstanceIdent)
		{
			this.Logger = Logger;
			this.InstanceIdent = InstanceIdent;
		}
		public async Task<long> Send(string target, Message message)
		{
			var mid=await Logger.BeginSend(InstanceIdent.ValueOrDefault()?.Value, target, message);
			var eid = Guid.NewGuid().ToString("N");
			await Logger.EndSend(mid, eid, null);
			return mid;
		}
	}
	//public interface IMsgBatchProvider :IMsgProvider
	//{
	//	Task<MessageSendResult[]> Send(string[] targets, Message message);
	//       int MaxBatchCount { get; }
	//}
}
