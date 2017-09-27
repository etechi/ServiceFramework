using SF.Auth.Identities;
using SF.Core.ServiceManagement.Management;
using SF.Entities;
using SF.Entities.AutoEntityProvider;
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
			sc.AddAutoEntityType(
				(TablePrefix ?? "") + "Invitation",
				typeof(MemberInvitation),
				typeof(MemberInvitationInternal)
				);

			sc.AddManagedScoped<IMemberInvitationManagementService, EntityMemberInvitationManagementService>(
				async (sp,svc)=>
					await svc.RemoveAllAsync()
				);

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