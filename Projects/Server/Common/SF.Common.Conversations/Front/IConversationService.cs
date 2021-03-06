﻿
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
	[DefaultAuthorize]
	public interface IConversationService
	{
		/// <summary>
		/// 查询会话成员
		/// </summary>
		/// <param name="Arg"></param>
		/// <returns></returns>
		Task<QueryResult<SessionMember>> QueryMembers(MemberQueryArgument Arg);

		/// <summary>
		/// 查询当前用户会话
		/// </summary>
		/// <param name="Arg">查询参数</param>
		/// <returns></returns>
		Task<QueryResult<SessionGroup>> QuerySessions(SessionQueryArgument Arg);

		/// <summary>
		/// 查询会话消息
		/// </summary>
		/// <param name="Arg">查询参数</param>
		/// <returns></returns>
		Task<QueryResult<SessionMessage>> QueryMessages(MessageQueryArgument Arg);


		/// <summary>
		/// 发送会话消息
		/// </summary>
		/// <param name="Arg">消息发送参数</param>
		/// <returns>消息ID</returns>
		Task<ObjectKey<long>> SendMessage(MessageSendArgument Arg);


		/// <summary>
		/// 设置最后访问时间,清除未读标记
		/// </summary>
		/// <param name="Arg">会话</param>
		/// <returns></returns>
		Task SetReadTime(SetReadTimeArgument Arg);


	}
}