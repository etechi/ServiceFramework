using SF.Auth.Identities;
using SF.Core;
using SF.Core.CallPlans;
using SF.Core.Times;
using SF.Data;
using SF.Entities;
using SF.Entities.AutoEntityProvider;
using SF.Users.Members.Models;
using SF.Users.Promotions.MemberInvitations.Entity.DataModels;
using SF.Users.Promotions.MemberInvitations.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SF.Users.Promotions.MemberInvitations.Entity
{
	public class EntityMemberInvitationManagementService :
		AutoEntityManager< MemberInvitationInternal, MemberInvitationInternal, MemberInvitationInternal, MemberInvitationQueryArgument>,
		IMemberInvitationManagementService
	{
		public EntityMemberInvitationManagementService(IDataSetAutoEntityProvider<MemberInvitationInternal, MemberInvitationInternal, MemberInvitationInternal, MemberInvitationQueryArgument> AutoEntityProvider) : base(AutoEntityProvider)
		{
		}
	}

}
