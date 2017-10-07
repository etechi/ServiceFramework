using SF.Auth;
using SF.Auth.Identities;
using SF.Auth.Identities.Models;
using SF.Entities;
using SF.Metadata;
using SF.Users.Promotions.MemberInvitations.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Users.Promotions.MemberInvitations
{
	public class MemberInvitationQueryArgument : Entities.IQueryArgument<ObjectKey<long>>
	{
		[Comment("Id")]
		public ObjectKey<long> Id { get; set; }

		[Comment("名称")]
		public string Name { get; set; }
	}


	[EntityManager]
	[Authorize("admin")]
	[NetworkService]
	[Comment("会员邀请")]
	[Category("用户管理", "会员邀请管理")]

	public interface IMemberInvitationManagementService : 
		Entities.IEntitySource<ObjectKey<long>, MemberInvitationInternal, MemberInvitationQueryArgument>,
		Entities.IEntityManager<ObjectKey<long>, MemberInvitationInternal>
    {
	}

}

