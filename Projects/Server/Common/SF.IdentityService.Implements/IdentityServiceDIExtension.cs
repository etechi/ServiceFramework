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

using SF.Metadata;
using System.Collections.Generic;
using System.Threading.Tasks;
using SF.Core.ServiceManagement;
using SF.Auth.IdentityServices.Managers;
using SF.Auth.IdentityServices.Models;
using SF.Auth.IdentityServices;
using SF.Core.ServiceManagement.Management;
using SF.Auth.IdentityServices.Internals;
using SF.Auth.IdentityServices.UserCredentialProviders;
using System;

namespace SF.Core.ServiceManagement
{
	public static class IdentityServiceDIExtension 
	{
		public static IServiceCollection AddIdentityService(this IServiceCollection sc)
		{
			//文章
			sc.EntityServices(
				"identity",
				"身份与授权",
				d => d.Add<IClaimTypeManager, ClaimTypeManager>("AuthClaimType", "凭证申明类型", typeof(ClaimType))
					.Add<IClientConfigManager, ClientConfigManager>("AuthClientConfig", "客户端配置", typeof(ClientConfigInternal))
					.Add<IClientManager, ClientManager>("AuthClient", "客户端", typeof(ClientInternal))
					.Add<IOperationManager, OperationManager>("AuthOperation","操作",typeof(OperationInternal))
					.Add<IResourceManager, ResourceManager>("AuthResource", "资源", typeof(ResourceInternal))
					.Add<IRoleManager, RoleManager>("AuthRole", "身份", typeof(Role))
					.Add<IUserManager, UserManager>("AuthUser", "用户", typeof(UserInternal))
				//.Add<IDocumentService, DocumentService>()
				);

			sc.AddManagedScoped<IUserService, UserService>();

			sc.AddManagedScoped<IUserCredentialProvider, PhoneNumberUserCredentialProvider>();
			sc.AddManagedScoped<IUserCredentialProvider, LocalUserCredentialProvider>();
			//sc.GenerateEntityManager("DocumentCategory");
			//sc.GenerateEntityManager("Document");

			//sc.AddAutoEntityType(
			//	(TablePrefix ?? "") + "Doc",
			//	false,
			//	typeof(Document),
			//	typeof(DocumentInternal),
			//	typeof(DocumentEditable),
			//	typeof(Category),
			//	typeof(CategoryInternal)
			//	);


			sc.AddDataModules(
				"SysAuth",
				typeof(SF.Auth.IdentityServices.DataModels.ClaimType),
				typeof(SF.Auth.IdentityServices.DataModels.Client),
				typeof(SF.Auth.IdentityServices.DataModels.ClientConfig),
				typeof(SF.Auth.IdentityServices.DataModels.ClientClaimValue),
				typeof(SF.Auth.IdentityServices.DataModels.ClientGrant),
				typeof(SF.Auth.IdentityServices.DataModels.Operation),
				typeof(SF.Auth.IdentityServices.DataModels.OperationRequiredClaim),
				typeof(SF.Auth.IdentityServices.DataModels.Resource),
				typeof(SF.Auth.IdentityServices.DataModels.ResourceRequiredClaim),
				typeof(SF.Auth.IdentityServices.DataModels.ResourceSupportedOperation),
				typeof(SF.Auth.IdentityServices.DataModels.Role),
				typeof(SF.Auth.IdentityServices.DataModels.RoleClaimValue),
				typeof(SF.Auth.IdentityServices.DataModels.RoleGrant),
				typeof(SF.Auth.IdentityServices.DataModels.User),
				typeof(SF.Auth.IdentityServices.DataModels.UserClaimValue),
				typeof(SF.Auth.IdentityServices.DataModels.UserCredential),
				typeof(SF.Auth.IdentityServices.DataModels.UserRole)
				);

			//sc.AddAutoEntityTest(NewDocumentManager);
			//sc.AddAutoEntityTest(NewDocumentCategoryManager);
			sc.InitServices("初始化认证授权服务", NewSerivces);
			return sc;
		}

		static async Task NewSerivces(IServiceProvider ServiceProvider, IServiceInstanceManager sim, long? ScopeId)
		{
			await sim.NewIdentityService().Ensure(ServiceProvider, ScopeId);
		}

		public static IServiceInstanceInitializer NewIdentityService(
			this IServiceInstanceManager manager
			)
		{
			var svc = manager.DefaultService<IUserService, UserService>(
				null,
				null,
				manager.DefaultService<IUserManager, UserManager>(null),
				manager.DefaultService<IRoleManager, RoleManager>(null),
				manager.DefaultService<IResourceManager, ResourceManager>(null),
				manager.DefaultService<IOperationManager, OperationManager>(null),
				manager.DefaultService<IClientManager, ClientManager>(null),
				manager.DefaultService<IClientConfigManager, ClientConfigManager>(null),
				manager.DefaultService<IClaimTypeManager, ClaimTypeManager>(null),
				manager.DefaultServiceWithIdent<IUserCredentialProvider, LocalUserCredentialProvider>("local",null),
				manager.DefaultServiceWithIdent<IUserCredentialProvider, PhoneNumberUserCredentialProvider>("phone", null)
				)
				.WithDisplay("身份和权限")
				.WithMenuItems("身份和权限");
			return svc;
		}
	}
}

