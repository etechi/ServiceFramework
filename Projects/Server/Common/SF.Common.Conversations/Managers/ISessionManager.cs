
using System.Threading.Tasks;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using SF.Sys.Annotations;
using SF.Common.Conversations.Models;
using SF.Sys.Auth;
using System;
using SF.Sys.Services;
using System.Linq;

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
		public long? BizIdent { get; set; }

		/// <summary>
		/// 业务分组
		/// </summary>
		public int BizGroup { get; set; }
	}

	public class SessionCreateArgument
	{
		public string BizIdentType { get; set; }
		public long BizIdent { get; set; }

		public string Name { get; set; }
		public long OwnerId { get; set; }
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

	public static class SessionManagerExtension
	{
		

		public static Task<long> GetOrCreateSession(
			this IScoped<ISessionManager> SessionManagerScope,
			string SessionBizIdentType,
			long SessionBizIdent,
			Func<Task<SessionCreateArgument>> GetSessionEnsureArgument
			)
		{
			return SF.Sys.Threading.TaskUtils.Retry(() =>
				SessionManagerScope.Use(async (sm) =>
				{
					var re = await sm.QueryIdentsAsync(new SessionQueryArgument
					{
						BizIdentType = SessionBizIdentType,
						BizIdent = SessionBizIdent
					});
					var sid = re.Items.SingleOrDefault();
					if (sid != null) return sid.Id;

					var arg = await GetSessionEnsureArgument();
					var sess= await sm.CreateAsync(
						new SessionEditable
						{
							BizIdent=arg.BizIdent,
							BizIdentType=arg.BizIdentType,
							Name=arg.Name,
							OwnerId=arg.OwnerId
						}
						);
					return sess.Id;
				}));
		}
	}
}