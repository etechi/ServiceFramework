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
using System;
using System.Collections.Generic;

namespace SF.Core.ServiceManagement
{
	public static class IdentityServiceDIExtension
	{
		public static IServiceCollection AddAuthIdentityManagementService<TIdentity, TIdentityCredential>(
			   this IServiceCollection sc,
			   string TablePrefix = null
			   )
			   where TIdentity : SF.Auth.Identities.Entity.DataModels.Identity<TIdentity, TIdentityCredential>, new()
			   where TIdentityCredential : SF.Auth.Identities.Entity.DataModels.IdentityCredential<TIdentity, TIdentityCredential>, new()
		{
			sc.AddDataModules<TIdentity, TIdentityCredential>(TablePrefix);
			sc.AddTransient<IIdentStorage>(sp => (IIdentStorage)sp.Resolve<IIdentityManagementService>());
			sc.AddManagedScoped<IIdentityCredentialStorage, EntityIdentityCredentialStorage<TIdentity, TIdentityCredential>>();

			sc.EntityServices(
				"AuthIdentity",
				"身份认证",
				d => d.Add<IIdentityManagementService, EntityIdentityManagementService<TIdentity, TIdentityCredential>>("AuthIdentity", "用户身份")
				);

			return sc;
		}
		public static IServiceCollection AddAuthIdentityManagementService(
			this IServiceCollection sc,
			string TablePrefix = null
			) => sc.AddAuthIdentityManagementService<SF.Auth.Identities.Entity.DataModels.Identity, SF.Auth.Identities.Entity.DataModels.IdentityCredential>(
				TablePrefix
				);

		public static IServiceCollection AddAuthIdentityService(
			this IServiceCollection sc
			)
		{
			sc.AddManagedScoped<IIdentityService, IdentityService>();
			return sc;
		}
		public static IServiceCollection AddAuthIdentityCredentialProviders(
			this IServiceCollection sc
			)
		{
			sc.AddManagedScoped<IIdentityCredentialProvider, PhoneNumberIdentityCredentialProvider>();
			sc.AddManagedScoped<IIdentityCredentialProvider, UserAccountIdentityCredentialProvider>();
			return sc;
		}
		public static IServiceCollection AddAuthIdentityServices(this IServiceCollection sc)
		{
			sc.AddAuthIdentityManagementService();
			sc.AddAuthIdentityCredentialProviders();
			sc.AddAuthIdentityService();
			return sc;
		}
		public static IServiceInstanceInitializer<IIdentityCredentialStorage> NewAuthIdentityCredentialStorage<TIdentity, TIdentityCredential>(this IServiceInstanceManager sim)
				 where TIdentity : SF.Auth.Identities.Entity.DataModels.Identity<TIdentity, TIdentityCredential>, new()
			   where TIdentityCredential : SF.Auth.Identities.Entity.DataModels.IdentityCredential<TIdentity, TIdentityCredential>, new()
			=> sim.DefaultService<IIdentityCredentialStorage, EntityIdentityCredentialStorage<TIdentity, TIdentityCredential>>(null);

		public static IServiceInstanceInitializer<IIdentityCredentialStorage> NewAuthIdentityCredentialStorage(this IServiceInstanceManager sim)
			=> sim.NewAuthIdentityCredentialStorage<SF.Auth.Identities.Entity.DataModels.Identity, SF.Auth.Identities.Entity.DataModels.IdentityCredential>();

		public static IServiceInstanceInitializer<IIdentityManagementService> NewAuthIdentityManagementService<TIdentity, TIdentityCredential>(this IServiceInstanceManager sim)
				 where TIdentity : SF.Auth.Identities.Entity.DataModels.Identity<TIdentity, TIdentityCredential>, new()
			   where TIdentityCredential : SF.Auth.Identities.Entity.DataModels.IdentityCredential<TIdentity, TIdentityCredential>, new()
			=> sim.DefaultService<IIdentityManagementService, EntityIdentityManagementService<TIdentity, TIdentityCredential>>(null);

		public static IServiceInstanceInitializer<IIdentityManagementService> NewAuthIdentityManagementService(this IServiceInstanceManager sim)
			=> sim.NewAuthIdentityManagementService<SF.Auth.Identities.Entity.DataModels.Identity, SF.Auth.Identities.Entity.DataModels.IdentityCredential>();

		public static IServiceInstanceInitializer<IIdentityCredentialProvider> NewAuthPhoneNumberIdentityCredentialProvider(
			this IServiceInstanceManager sim,
			ConfirmMessageTemplateSetting Template=null
			)
			=> sim.Service<IIdentityCredentialProvider, PhoneNumberIdentityCredentialProvider>(
				new
				{
					ConfirmMessageSetting= Template??new ConfirmMessageTemplateSetting()
				}
				).WithIdent("手机号认证");
		public static IServiceInstanceInitializer<IIdentityCredentialProvider> NewAuthUserAccountIdentityCredentialProvider(
			this IServiceInstanceManager sim
			)
			=> sim.Service<IIdentityCredentialProvider, UserAccountIdentityCredentialProvider>(null)
			.WithIdent("账号认证");

		public static IServiceInstanceInitializer<IIdentityService> NewAuthIdentityServive(
			this IServiceInstanceManager sim,
			IServiceInstanceInitializer<IIdentityManagementService> IdentityManagementService=null,
			IServiceInstanceInitializer<IIdentityCredentialStorage> IdentityCredentialStorage = null,
			params IServiceInstanceInitializer<IIdentityCredentialProvider>[] IdentityCredentialProviders
			)
		{
			var chds = new List<IServiceInstanceInitializer>();
			chds.Add(IdentityCredentialStorage ?? sim.NewAuthIdentityCredentialStorage());
			if (IdentityCredentialProviders.Length== 0)
				chds.Add(sim.NewAuthUserAccountIdentityCredentialProvider());
			else
				chds.AddRange(IdentityCredentialProviders);
			chds.Add(IdentityManagementService ?? sim.NewAuthIdentityManagementService());

			return sim.DefaultService<IIdentityService, IdentityService>(
				new{},
				chds.ToArray()
				);
		}

	}
}