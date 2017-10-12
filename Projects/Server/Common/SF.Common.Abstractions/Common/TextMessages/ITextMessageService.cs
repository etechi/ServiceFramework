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

namespace SF.Common.TextMessages
{
	//public class MessageSendResult
	//{
	//	public MessageSendResult(string Target, string Result, Exception Exception)
	//	{
	//		this.Target = Target;
	//		this.Result = Result;
	//		this.Exception = Exception;
	//	}
	//	public string Target { get; }
	//	public string Result { get; }
	//	public Exception Exception { get; }
	//}
	//public class MessageSendFailedException : PublicException
	//{
	//	public MessageSendResult[] Results { get; }
	//	public MessageSendFailedException(string message, MessageSendResult[] results, Exception innerException) : base(message, innerException)
	//	{
	//		Results = results;
	//	}
	//}
	[Comment(Name = "文本消息服务")]
    public interface ITextMessageService
	{
		Task<long> Send(long? targetId,string target, Message message);
	}
	//public interface IMsgBatchProvider :IMsgProvider
	//{
	//	Task<MessageSendResult[]> Send(string[] targets, Message message);
 //       int MaxBatchCount { get; }
	//}
}
