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

using SF.Auth.Users;
using SF.Auth.Users.Entity;
using SF.Auth.Users.IdentityCredentialProviders;
using SF.Auth.Users.Internals;
using SF.Auth.Users.Models;
using SF.Common.Members;
using SF.Core.ServiceManagement;
using SF.Core.ServiceManagement.Management;
using SF.Entities;
using System;
using System.Collections.Generic;

namespace SF.Core.ServiceManagement
{
	public static class IdentityServiceDIExtension
	{
		public static IServiceCollection AddAuthUserManager<
			TService,TImplement,
			TInternal,TEditable,TQueryArgument, 
			TUser, TUserCredential,TUserClaimValue,TUserRole
			>(
			   this IServiceCollection sc,
			   string TablePrefix = null
			   )
			   where TUser : SF.Auth.Users.Entity.DataModels.User<TUser, TUserCredential, TUserClaimValue, TUserRole>, new()
			   where TUserCredential : SF.Auth.Users.Entity.DataModels.UserCredential<TUser, TUserCredential, TUserClaimValue, TUserRole>, new()
			where TUserClaimValue : SF.Auth.Users.Entity.DataModels.UserClaimValue<TUser, TUserCredential, TUserClaimValue, TUserRole>, new()
			where TUserRole: SF.Auth.Users.Entity.DataModels.UserRole<TUser, TUserCredential, TUserClaimValue, TUserRole>, new()
			where TInternal : SF.Auth.Users.Models.UserInternal,new()
			where TEditable : SF.Auth.Users.Models.UserEditable,new()
			where TQueryArgument : SF.Auth.Users.UserQueryArgument,new()
			where TService:class,IUserManager<TInternal,TEditable,TQueryArgument>
			where TImplement:EntityUserManager<TInternal,TEditable,TQueryArgument,TUser,TUserCredential,TUserClaimValue,TUserRole>,TService
		{
			sc.AddDataModules<
				TUser, 
				TUserCredential,
				TUserClaimValue,
				TUserRole,
				SF.Auth.Users.Entity.DataModels.ClaimType,
				SF.Auth.Users.Entity.DataModels.RoleClaimValue
				>(TablePrefix);
			sc.AddTransient<IUserStorage>(sp => (IUserStorage)sp.Resolve<TService>());
			sc.AddManagedScoped<IUserCredentialStorage, EntityUserCredentialStorage<TUser, TUserCredential, TUserClaimValue,TUserRole>>();

			sc.EntityServices(
				"AuthUser",
				"身份认证",
				d => d.Add<TService,TImplement>("User", "用户")
				);

			return sc;
		}
		public static IServiceCollection AddAuthUserManager(
			this IServiceCollection sc,
			string TablePrefix = null
			) => sc.AddAuthUserManager<
				IUserManager,
				EntityUserManager,
				SF.Auth.Users.Models.UserInternal,
				SF.Auth.Users.Models.UserEditable,
				SF.Auth.Users.UserQueryArgument,
				SF.Auth.Users.Entity.DataModels.User, 
				SF.Auth.Users.Entity.DataModels.UserCredential,
				SF.Auth.Users.Entity.DataModels.UserClaimValue,
				SF.Auth.Users.Entity.DataModels.UserRole
				>(
				TablePrefix
				);

		
		public static IServiceCollection AddAuthIdentityCredentialProviders(
			this IServiceCollection sc
			)
		{
			sc.AddManagedScoped<IUserCredentialProvider, PhoneNumberIdentityCredentialProvider>();
			sc.AddManagedScoped<IUserCredentialProvider, UserAccountIdentityCredentialProvider>();
			return sc;
		}

		public static IServiceCollection AddAuthIdentityService(
			this IServiceCollection sc
			)
		{
			sc.AddManagedScoped<IUserService, UserService>();
			return sc;
		}
		public static IServiceCollection AddAuthUserServices(this IServiceCollection sc)
		{
			sc.AddAuthUserManager();
			sc.AddAuthIdentityCredentialProviders();
			sc.AddAuthIdentityService();
			return sc;
		}
		public static IServiceInstanceInitializer<IUserCredentialStorage> NewAuthUserCredentialStorage<TUser, TUserCredential,TUserClaimValue,TUserRole>(
			this IServiceInstanceManager sim)
			 where TUser : SF.Auth.Users.Entity.DataModels.User<TUser, TUserCredential,TUserClaimValue,TUserRole>, new()
			where TUserCredential : SF.Auth.Users.Entity.DataModels.UserCredential<TUser, TUserCredential, TUserClaimValue, TUserRole>, new()
			where TUserClaimValue : SF.Auth.Users.Entity.DataModels.UserClaimValue<TUser, TUserCredential, TUserClaimValue, TUserRole>, new()
			where TUserRole : SF.Auth.Users.Entity.DataModels.UserRole<TUser, TUserCredential, TUserClaimValue, TUserRole>, new()
			=> sim.DefaultService<IUserCredentialStorage, EntityUserCredentialStorage<TUser, TUserCredential, TUserClaimValue,TUserRole>>(null);

		public static IServiceInstanceInitializer<IUserCredentialStorage> NewAuthIdentityCredentialStorage(this IServiceInstanceManager sim)
			=> sim.NewAuthUserCredentialStorage<
				SF.Auth.Users.Entity.DataModels.User, 
				SF.Auth.Users.Entity.DataModels.UserCredential,
				SF.Auth.Users.Entity.DataModels.UserClaimValue,
				SF.Auth.Users.Entity.DataModels.UserRole
				>();

		public static IServiceInstanceInitializer<TService> NewAuthUserManagerService<TService,TImplement,TInternal,TEditable,TQueryArgument, TUser, TUserCredential, TUserClaimValue, TUserRole>(this IServiceInstanceManager sim)
			where TService:IUserManager<TInternal,TEditable,TQueryArgument>
			where TImplement: EntityUserManager<TInternal, TEditable, TQueryArgument, TUser, TUserCredential, TUserClaimValue, TUserRole>,TService
			where TInternal: UserInternal
			where TEditable : UserEditable,new()
			where TQueryArgument : UserQueryArgument,new()
			 where TUser : SF.Auth.Users.Entity.DataModels.User<TUser, TUserCredential, TUserClaimValue, TUserRole>, new()
			where TUserCredential : SF.Auth.Users.Entity.DataModels.UserCredential<TUser, TUserCredential, TUserClaimValue, TUserRole>, new()
			where TUserClaimValue : SF.Auth.Users.Entity.DataModels.UserClaimValue<TUser, TUserCredential, TUserClaimValue, TUserRole>, new()
			where TUserRole : SF.Auth.Users.Entity.DataModels.UserRole<TUser, TUserCredential, TUserClaimValue, TUserRole>, new()
			=> sim.DefaultService<TService,TImplement >(null);

		public static IServiceInstanceInitializer<IUserManager> NewAuthUserManagerService(this IServiceInstanceManager sim)
			=> sim.NewAuthUserManagerService<
				SF.Auth.Users.IUserManager,
				SF.Auth.Users.Entity.EntityUserManager,
				SF.Auth.Users.Models.UserInternal,
				SF.Auth.Users.Models.UserEditable,
				SF.Auth.Users.UserQueryArgument,
				SF.Auth.Users.Entity.DataModels.User, 
				SF.Auth.Users.Entity.DataModels.UserCredential, 
				SF.Auth.Users.Entity.DataModels.UserClaimValue,
				SF.Auth.Users.Entity.DataModels.UserRole
				>();

		public static IServiceInstanceInitializer<IUserCredentialProvider> NewAuthPhoneNumberIdentityCredentialProvider(
			this IServiceInstanceManager sim,
			ConfirmMessageTemplateSetting Template=null
			)
			=> sim.Service<IUserCredentialProvider, PhoneNumberIdentityCredentialProvider>(
				new
				{
					ConfirmMessageSetting= Template??new ConfirmMessageTemplateSetting()
				}
				).WithIdent("手机号认证");
		public static IServiceInstanceInitializer<IUserCredentialProvider> NewAuthUserAccountIdentityCredentialProvider(
			this IServiceInstanceManager sim
			)
			=> sim.Service<IUserCredentialProvider, UserAccountIdentityCredentialProvider>(null)
			.WithIdent("账号认证");

		public static IServiceInstanceInitializer<IUserService> NewAuthIdentityServive(
			this IServiceInstanceManager sim,
			IServiceInstanceInitializer<IUserManager> UserManager=null,
			IServiceInstanceInitializer<IUserCredentialStorage> UserCredentialStorage = null,
			params IServiceInstanceInitializer<IUserCredentialProvider>[] UserCredentialProviders
			)
		{
			var chds = new List<IServiceInstanceInitializer>();
			chds.Add(UserCredentialStorage ?? sim.NewAuthIdentityCredentialStorage());
			if (UserCredentialProviders.Length== 0)
				chds.Add(sim.NewAuthUserAccountIdentityCredentialProvider());
			else
				chds.AddRange(UserCredentialProviders);
			chds.Add(UserManager ?? sim.NewAuthUserManagerService());

			return sim.DefaultService<IUserService, UserService>(
				new{},
				chds.ToArray()
				);
		}

	}
}