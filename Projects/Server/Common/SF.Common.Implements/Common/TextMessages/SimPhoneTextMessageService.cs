using SF.Metadata;
using System.Threading.Tasks;
using System;
using SF.Core.ServiceManagement;
using SF.Common.TextMessages.Management;

namespace SF.Common.TextMessages
{
	[Comment(Name = "模拟手机消息服务提供者")]
	public class SimPhoneTextMessageService : IPhoneMessageService
	{
		ITextMessageLogger Logger { get; }
		IServiceInstanceDescriptor ServiceInstanceDescriptor { get; }
		public SimPhoneTextMessageService(ITextMessageLogger Logger,IServiceInstanceDescriptor ServiceInstanceDescriptor)
		{
			this.Logger = Logger;
			this.ServiceInstanceDescriptor = ServiceInstanceDescriptor;
		}
		public async Task<long> Send(long? targetId,string target, Message message)
		{
			var mid=await Logger.BeginSend(ServiceInstanceDescriptor.InstanceId, target, targetId,message);
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
