using SF.Auth.Identities;
using SF.Auth.Identities.Entity;
using SF.Auth.Identities.IdentityCredentialProviders;
using SF.Auth.Identities.Internals;
using SF.Core.ServiceManagement;
using SF.Core.ServiceManagement.Management;
using SF.Entities;
using SF.Users.Members;
using SF.Users.Members.Entity;
using System;
using System.Collections.Generic;

namespace SF.Core.ServiceManagement
{
	public static class MemberServiceDIExtension
	{
		public static IServiceCollection AddMemberManagementService<TMember>(
			   this IServiceCollection sc,
			   string TablePrefix = null
			   )
			   where TMember : SF.Users.Members.Entity.DataModels.Member<TMember>,new()
		{
			sc.AddDataModules<TMember>(TablePrefix);

			sc.AddManagedScoped<IMemberManagementService, EntityMemberManagementService<TMember>>(
				async (sp,svc)=>
					await svc.RemoveAllAsync()
				);

			return sc;
		}
		public static IServiceCollection AddMemberManagementService(
			this IServiceCollection sc,
			string TablePrefix = null
			) => sc.AddMemberManagementService<SF.Users.Members.Entity.DataModels.Member>(
				TablePrefix
				);

		public static IServiceCollection AddMemberService(
			this IServiceCollection sc
			)
		{
			sc.AddManagedScoped<IMemberService, MemberService>();
			return sc;
		}
		public static IServiceInstanceInitializer<IMemberManagementService> NewMemberManagementService<TMember>(this IServiceInstanceManager sim)
				 where TMember : SF.Users.Members.Entity.DataModels.Member<TMember>, new()
			=> sim.DefaultService<IMemberManagementService, EntityMemberManagementService<TMember>>(null);

		public static IServiceInstanceInitializer<IMemberManagementService> NewMemberManagementService(this IServiceInstanceManager sim)
			=> sim.NewMemberManagementService<SF.Users.Members.Entity.DataModels.Member>();

		public static IServiceInstanceInitializer<IMemberService> NewMemberServive(
			this IServiceInstanceManager sim,
			IServiceInstanceInitializer<IMemberManagementService> MemberManagementService=null,
			IServiceInstanceInitializer<IIdentityService> IdentityService = null
			)
		{
			return sim.DefaultService<IMemberService, MemberService>(
				new{},
				MemberManagementService,
				IdentityService
				);
		}

	}
}