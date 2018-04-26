
using System.Threading.Tasks;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using SF.Sys.Annotations;
using SF.Common.UserGroups.Models;
using SF.Sys.Auth;
using System;
using SF.Sys.Services;
using System.Linq;

namespace SF.Common.UserGroups.Managers
{
	public class GroupQueryArgument : ObjectQueryArgument
	{
		/// <summary>
		/// 所有人
		/// </summary>
		[EntityIdent(typeof(User))]
		public long? OwnerId { get; set; }

		/// <summary>
		/// 业务实体类型
		/// </summary>
		[EntityType]
		[Ignore]
		public string BizIdentType { get; set; }

		/// <summary>
		/// 业务实体对象
		/// </summary>
		[EntityIdent(EntityTypeField =nameof(BizIdentType))]
		[Ignore]
		public long? BizIdent { get; set; }

	}

	/// <summary>
	/// 用户组管理
	/// </summary>
	[NetworkService]
	[EntityManager]
	[DefaultAuthorize(PredefinedRoles.客服专员,true)]
	[DefaultAuthorize(PredefinedRoles.系统管理员, true)]
	public interface IGroupManager<TGroup,TMember,TGroupEditable,TQueryArgument> :
		IEntitySource<ObjectKey<long>, TGroup, TQueryArgument>,
		IEntityManager<ObjectKey<long>, TGroupEditable>
		where TGroup:Group<TGroup, TMember>
		where TGroupEditable:Group<TGroup, TMember>
		where TMember:GroupMember<TGroup,TMember>
		where TQueryArgument:GroupQueryArgument
	{

		
	}

}