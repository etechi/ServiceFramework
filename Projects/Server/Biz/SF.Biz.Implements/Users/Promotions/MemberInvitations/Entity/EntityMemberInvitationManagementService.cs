﻿using SF.Auth.Identities;
using SF.Core;
using SF.Core.CallPlans;
using SF.Core.Times;
using SF.Data;
using SF.Entities;
using SF.Entities.AutoEntityProvider;
using SF.Users.Members.Models;
using SF.Users.Promotions.MemberInvitations.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SF.Users.Promotions.MemberInvitations.Entity
{
	public class EntityMemberInvitationManagementService :
		AutoEntityManager<ObjectKey<long>, MemberInvitationInternal, MemberInvitationInternal, MemberInvitationInternal, MemberInvitationQueryArgument>,
		IMemberInvitationManagementService
	{
		public EntityMemberInvitationManagementService(IDataSetAutoEntityProviderFactory DataSetAutoEntityProviderFactory) : base(DataSetAutoEntityProviderFactory)
		{
		}
	}

}
