
using System.Threading.Tasks;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using SF.Sys.Annotations;
using SF.Common.Conversations.Models;

namespace SF.Common.Conversations.Managers
{
	public class SessionMessageQueryArgument : ObjectQueryArgument
	{
		
	}

	/// <summary>
	/// 会话消息管理
	/// </summary>
	[NetworkService]
	[EntityManager]
	public interface ISessionMessageManager :
		IEntitySource<ObjectKey<long>, SessionMessage, SessionMessageQueryArgument>,
		IEntityManager<ObjectKey<long>, SessionMessage>
	{

	}
}