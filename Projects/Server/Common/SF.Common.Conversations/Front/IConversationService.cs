
using System.Threading.Tasks;
using System.Data.Common;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using SF.Sys.Auth;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Annotations;
using System.Collections.Generic;

namespace SF.Common.Conversations.Front
{
	
	/// <summary>
	/// 交谈服务
	/// </summary>
	[NetworkService]
	public interface IConversationService
	{
		/// <summary>
		/// 查询会话消息
		/// </summary>
		/// <param name="Arg">查询参数</param>
		/// <returns></returns>
		Task<QueryResult<SessionMessage>> QueryMessage(MessageQueryArgument Arg);


		/// <summary>
		/// 发送会话消息
		/// </summary>
		/// <param name="Arg">消息发送参数</param>
		/// <returns>消息ID</returns>
		Task<ObjectKey<long>> SendMessage(MessageSendArgument Arg);


		/// <summary>
		/// 设置最后访问时间,清除未读标记
		/// </summary>
		/// <param name="SessionId">会话</param>
		/// <returns></returns>
		Task SetReadTime(long SessionId);


	}
}