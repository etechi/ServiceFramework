
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

	}
}