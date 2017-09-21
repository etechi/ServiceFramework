using SF.Auth;
using SF.Auth.Identities;
using SF.Auth.Identities.Models;
using SF.Metadata;
using SF.Users.Promotions.MemberInvitations.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Users.Promotions.MemberInvitations
{
	public class MemberInvitationQueryArgument : Entities.IQueryArgument<long>
	{
		[Comment("Id")]
		public Option<long> Id { get; set; }

		[Comment("名称")]
		public string Name { get; set; }
	}

	
	[EntityManager("会员邀请")]
	[Authorize("admin")]
	[NetworkService]
	[Comment("会员邀请")]
	[Category("用户管理", "会员邀请管理")]
	public interface IMemberInvitationManagementService : 
		Entities.IEntitySource<long,MemberInvitationInternal, MemberInvitationQueryArgument>,
		Entities.IEntityManager<long, MemberInvitationInternal>
    {
	}

}

