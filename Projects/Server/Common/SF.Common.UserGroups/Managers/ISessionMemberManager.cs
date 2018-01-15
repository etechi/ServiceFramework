
using System.Threading.Tasks;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using SF.Sys.Annotations;
using SF.Common.Conversations.Models;
using SF.Sys.Auth;

namespace SF.Common.Conversations.Managers
{
	public class SessionMemberQueryArgument : ObjectQueryArgument
	{
		/// <summary>
		/// 会话
		/// </summary>
		[EntityIdent(typeof(Session))]
		public long? SessionId { get; set; }

		/// <summary>
		/// 用户
		/// </summary>
		[EntityIdent(typeof(User))]
		public long? OwnerId { get; set; }
	}

	/// <summary>
	/// 会话成员管理
	/// </summary>
	[NetworkService]
	[EntityManager]
	public interface ISessionMemberManager :
		IEntitySource<ObjectKey<long>, SessionMember, SessionMemberQueryArgument>,
		IEntityManager<ObjectKey<long>, SessionMember>
	{

		/// <summary>
		/// 创建成员
		/// </summary>
		/// <param name="SessionId">目标会话</param>
		/// <param name="TargetUserId">目标用户ID</param>
		/// <param name="MemberAccepted">成员是否同意加入</param>
		/// <param name="SessionAccepted">会话是否同意加入</param>
		/// <param name="BizType">业务类型</param>
		/// <param name="BizIdentType">业务标识类型</param>
		/// <param name="BizIdent">业务标识</param>
		/// <returns>成员ID</returns>
		Task<long> MemberEnsure(
			long SessionId,
			long TargetUserId,
			bool SessionAccepted,
			bool MemberAccepted,
			int BizType,
			string BizIdentType,
			long? BizIdent
			);

		/// <summary>
		/// 创建一个成员加入动作令牌
		/// </summary>
		/// <param name="ActionName">动作名称</param>
		/// <param name="SessionId">会话ID</param>
		/// <param name="TargetUserId">目标用户ID</param>
		/// <param name="MemberAccepted">成员是否同意加入</param>
		/// <param name="SessionAccepted">会话是否同意加入</param>
		/// <param name="BizType">业务类型</param>
		/// <param name="BizIdentType">业务标识类型</param>
		/// <param name="BizIdent">业务标识</param>
		/// <returns>动作令牌</returns>
		Task<string> CreateMemberEnsureToken(
			string ActionName,
			long SessionId,
			long? TargetUserId,
			bool MemberAccepted,
			bool SessionAccepted,
			int BizType,
			string BizIdentType,
			long? BizIdent
			);

		/// <summary>
		/// 使用令牌执行动作
		/// </summary>
		/// <param name="ActionName">动作名称</param>
		/// <param name="Token">动作令牌</param>
		/// <param name="TargetUserId">当前用户</param>
		/// <returns>成员ID</returns>
		Task<long> EvalMemberEnsureToken(string ActionName, string Token, long TargetUserId);
	}
}