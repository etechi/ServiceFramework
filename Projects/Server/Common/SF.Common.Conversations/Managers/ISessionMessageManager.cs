
using System.Threading.Tasks;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using SF.Sys.Annotations;
using SF.Common.Conversations.Models;
using SF.Sys.Auth;

namespace SF.Common.Conversations.Managers
{
	public class SessionMessageQueryArgument : EventQueryArgument
	{

		/// <summary>
		/// 用户
		/// </summary>
		[EntityIdent(typeof(User))]
		public virtual long? UserId { get; set; }

	}

	/// <summary>
	/// 会话消息管理
	/// </summary>
	[NetworkService]
	[EntityManager]
	[DefaultAuthorize(PredefinedRoles.客服专员, true)]

	public interface ISessionMessageManager :
		IEntitySource<ObjectKey<long>, SessionMessage, SessionMessageQueryArgument>,
		IEntityManager<ObjectKey<long>, SessionMessage>
	{
		Task<SessionMessageDetail> GetMessageDetail(long Id);
	}
}