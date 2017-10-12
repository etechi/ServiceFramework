#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using SF.Auth.Identities;
using SF.Core.ServiceManagement.Management;
using SF.Entities;
using SF.Entities.AutoEntityProvider;
using SF.Entities.Tests;
using SF.Users.Members;
using SF.Users.Promotions.MemberInvitations;
using SF.Users.Promotions.MemberInvitations.Entity;
using SF.Users.Promotions.MemberInvitations.Models;

namespace SF.Core.ServiceManagement
{
	public static class InvitationDIExtension
	{
		public static IServiceCollection AddMemberInvitationService(
			   this IServiceCollection sc,
			   string TablePrefix = null
			   )
		{

				//(TablePrefix ?? "") + "Invitation",
				//typeof(MemberInvitation),
				//typeof(MemberInvitationInternal)
				//);

			sc.EntityServices(
				"MemberInvitation",
				"会员邀请",
				d=>d.Add<IMemberInvitationManagementService, EntityMemberInvitationManagementService>(typeof(MemberInvitation))
				);
			sc.GenerateEntityDataModel("MemberInvitation");
			sc.GenerateEntityManager("MemberInvitation");

			//sc.AddServiceInstanceCreator(
			//	(sim) =>
			//		sim.DefaultService<IMemberInvitationManagementService, EntityMemberInvitationManagementService>(
			//		new { }
			//		)
			//	);

			sc.AddAutoEntityTest(NewMemberInvitationServive);
			return sc;
		}
		
		public static IServiceInstanceInitializer<IMemberInvitationManagementService> NewMemberInvitationServive(
			this IServiceInstanceManager sim
			)
		{
			return sim.DefaultService<IMemberInvitationManagementService, EntityMemberInvitationManagementService>(
				new{}
				);
		}

	}
}