#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

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
