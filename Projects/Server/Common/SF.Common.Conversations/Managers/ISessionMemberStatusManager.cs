
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
		[EntityIdent(typeof(SessionStatus))]
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
	public interface ISessionMemberStatusManager :
		IEntitySource<ObjectKey<long>, SessionMemberStatus, SessionMemberQueryArgument>,
		IEntityManager<ObjectKey<long>, SessionMemberStatus>
	{
		Task SetReadTime(long SessionId,long UserId);
	}
}