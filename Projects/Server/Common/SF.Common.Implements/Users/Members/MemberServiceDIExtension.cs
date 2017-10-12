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

			sc.EntityServices(
				"Member",
				"会员",
				d => d.Add<IMemberManagementService, EntityMemberManagementService<TMember>>()
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
			sc.AddMemberManagementService();
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
				MemberManagementService ?? sim.NewMemberManagementService(),
				IdentityService ?? sim.NewAuthIdentityServive()
				);
		}

	}
}