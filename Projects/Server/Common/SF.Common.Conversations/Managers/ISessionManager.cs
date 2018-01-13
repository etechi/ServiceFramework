
using System.Threading.Tasks;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using SF.Sys.Annotations;
using SF.Common.Conversations.Models;
using SF.Sys.Auth;

namespace SF.Common.Conversations.Managers
{
	public class SessionQueryArgument : ObjectQueryArgument
	{
		/// <summary>
		/// 所有人
		/// </summary>
		[EntityIdent(typeof(User))]
		public long? OwnerId { get; set; }

		/// <summary>
		/// 业务实体类型
		/// </summary>
		
		public int BizType { get; set; }

		/// <summary>
		/// 业务实体类型
		/// </summary>
		[EntityType]
		public string BizIdentType { get; set; }

		/// <summary>
		/// 业务实体对象
		/// </summary>
		[EntityIdent(EntityTypeField =nameof(BizIdentType))]
		public long BizIdent { get; set; }

		/// <summary>
		/// 业务分组
		/// </summary>
		public int BizGroup { get; set; }
	}

	/// <summary>
	/// 会话管理
	/// </summary>
	[NetworkService]
	[EntityManager]
	public interface ISessionManager :
		IEntitySource<ObjectKey<long>, Session, SessionQueryArgument>,
		IEntityManager<ObjectKey<long>, SessionEditable>
	{


	}
}