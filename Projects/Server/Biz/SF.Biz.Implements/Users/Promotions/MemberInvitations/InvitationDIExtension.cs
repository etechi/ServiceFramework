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