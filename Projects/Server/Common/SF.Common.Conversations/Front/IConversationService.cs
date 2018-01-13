
using System.Threading.Tasks;
using System.Data.Common;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using SF.Sys.Auth;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Annotations;

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
		/// 查询会话成员
		/// </summary>
		/// <param name="Arg">查询参数</param>
		/// <returns></returns>
		Task<QueryResult<SessionMember>> QueryMember(MemberQueryArgument Arg);

		/// <summary>
		/// 查询会话
		/// </summary>
		/// <param name="Arg">查询参数</param>
		/// <returns></returns>
		Task<QueryResult<Session>> QuerySession(SessionQueryArgument Arg);
		
		
		/// <summary>
		/// 将某人从自己的会话中移除
		/// </summary>
		/// <param name="MemberId">成员ID</param>
		/// <returns></returns>
		Task RemoveMember(long MemberId);

		/// <summary>
		/// 离开某个已加入的会话
		/// </summary>
		/// <param name="SessionId">会话ID</param>
		/// <returns></returns>
		Task LeaveSession(long SessionId);

		/// <summary>
		/// 发送会话消息
		/// </summary>
		/// <param name="Arg">消息发送参数</param>
		/// <returns>消息ID</returns>
		Task<ObjectKey<long>> SendMessage(MessageSendArgument Arg);

		/// <summary>
		/// 修改成员信息
		/// </summary>
		/// <param name="Member">成员信息</param>
		/// <returns></returns>
		Task UpdateMember(SessionMember Member);

		/// <summary>
		/// 修改会话信息
		/// </summary>
		/// <param name="Session">会话信息</param>
		/// <returns></returns>
		Task UpdateSession(Session Session);

		/// <summary>
		/// 设置最后访问时间,清楚未读标记
		/// </summary>
		/// <param name="SessionId">会话</param>
		/// <returns></returns>
		Task SetReadTime(long SessionId);


		/// <summary>
		/// 设置成员加入会话状态
		/// </summary>
		/// <remarks>若当前用户是会话所有者,则设置所有者一方假如状态,否则设置成员方加入状态</remarks>
		/// <param name="MemberId">会话成员ID</param>
		/// <param name="AcceptType">是否愿意加入</param>
		/// <returns></returns>
		Task SetAcceptType(long MemberId,bool AcceptType);

	}
}