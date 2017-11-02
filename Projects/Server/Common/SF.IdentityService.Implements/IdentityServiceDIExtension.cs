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
using SF.Entities;
using System.Linq;
using SF.Services.Security;

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
			sc.AddManagedScoped<IUserCredentialStorage, UserCredentialStorage>();
			sc.AddTransient<IUserStorage>(sp => sp.Resolve<IUserManager>());
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
			sc.InitServices("初始化认证授权服务", NewSerivces,null,100000);
			return sc;
		}

		static async Task NewSerivces(IServiceProvider ServiceProvider, IServiceInstanceManager sim, long? ScopeId)
		{
			await sim.DefaultService<IUserService, UserService>(new{Setting = new { }})
				.WithDisplay("用户服务")
				.Ensure(ServiceProvider, ScopeId);

			await sim.DefaultService<IUserManager, UserManager>(null)
				.WithDisplay("用户")
				.WithMenuItems("身份和权限")
				.Ensure(ServiceProvider, ScopeId);

			await sim.DefaultService<IRoleManager, RoleManager>(null)
				.WithDisplay("用户角色")
				.WithMenuItems("身份和权限")
				.Ensure(ServiceProvider, ScopeId);
			await sim.DefaultService<IResourceManager, ResourceManager>(null)
				.WithDisplay("资源")
				.WithMenuItems("身份和权限")
				.Ensure(ServiceProvider, ScopeId);
			await sim.DefaultService<IOperationManager, OperationManager>(null)
				.WithDisplay("操作")
				.WithMenuItems("身份和权限")
				.Ensure(ServiceProvider, ScopeId);
			await sim.DefaultService<IClientManager, ClientManager>(null)
				.WithDisplay("客户端")
				.WithMenuItems("身份和权限")
				.Ensure(ServiceProvider, ScopeId);
			await sim.DefaultService<IClientConfigManager, ClientConfigManager>(null)
				.WithDisplay("客户端配置")
				.WithMenuItems("身份和权限")
				.Ensure(ServiceProvider, ScopeId);

			await sim.DefaultService<IClaimTypeManager, ClaimTypeManager>(null)
				.WithDisplay("凭证申明类型")
				.WithMenuItems("身份和权限")
				.Ensure(ServiceProvider, ScopeId);

			await sim.DefaultService<IUserCredentialStorage, UserCredentialStorage>(null)
				.Ensure(ServiceProvider, ScopeId);

			await sim.DefaultServiceWithIdent<IUserCredentialProvider, LocalUserCredentialProvider>("local", null)
				.Ensure(ServiceProvider, ScopeId);

			await sim.DefaultServiceWithIdent<IUserCredentialProvider, PhoneNumberUserCredentialProvider>("phone", null)
				.Ensure(ServiceProvider, ScopeId);

			//初始化默认申明类型
			var ClaimTypeManager = ServiceProvider.Resolve<IClaimTypeManager>();
			var predefinedClaimTypes = new[] {
				("acc","本地账号"),
				("sub","本地ID"),
				("phone","电话"),
				("address","地址"),
				("wx.open","微信开放平台"),
				("wx.mp","微信公众号"),
				("wx.uid","微信统一ID")
			};
			foreach (var ct in predefinedClaimTypes)
			{
				await ClaimTypeManager.EnsureEntity(
					await ClaimTypeManager.QuerySingleEntityIdent(new ClaimTypeQueryArgument { Id = ObjectKey.From( ct.Item1) }),
					() => new ClaimType() { Id=ct.Item1},
					ict =>
					{
						ict.Name = ct.Item2;
					}
					);
			}




			//初始化操作
			var OperationManager = ServiceProvider.Resolve<IOperationManager>();
			foreach (var o in ServiceProvider.Resolve<IEnumerable<SF.Auth.Permissions.IOperation>>()
				.WithFirst(
					new SF.Auth.Permissions.Operation(SF.Auth.Permissions.Operations.Read, "查看", "查看对象"),
					new SF.Auth.Permissions.Operation(SF.Auth.Permissions.Operations.Create, "新建", "新建对象"),
					new SF.Auth.Permissions.Operation(SF.Auth.Permissions.Operations.Update, "修改", "修改对象"),
					new SF.Auth.Permissions.Operation(SF.Auth.Permissions.Operations.Remove, "删除", "删除对象")
					))
			{
				await OperationManager.EnsureEntity(
					new ObjectKey<string> { Id = o.Id },
					() => new OperationInternal
					{
						Id = o.Id
					},
					e =>
					{
						e.Name = o.Name;
						e.Title = o.Name;
						e.Description = o.Description;
					}
					);
			}
			//初始化资源
			var ResourceManager = ServiceProvider.Resolve<IResourceManager>();
			var resPermissions = ServiceProvider.Resolve<IEnumerable<SF.Auth.Permissions.IResource>>();
			foreach (var o in resPermissions)
			{
				await ResourceManager.EnsureEntity(
					new ObjectKey<string> { Id = o.Id },
					() => new ResourceEditable
					{
						Id = o.Id
					},
					e =>
					{
						e.Name = o.Name;
						e.Title = o.Name;
						e.Description = o.Description;
						e.Operations = o.AvailableOperations.Select(oi => new ResourceOperationInternal
						{
							OperationId = oi

						});
					}
					);
			}

			var allGrants = (from r in resPermissions
							 from o in r.AvailableOperations
							 select new Grant
							 {
								 OperationId = o,
								 ResourceId = r.Id
							 }
							   ).ToArray();

			var ClientConfigManager =ServiceProvider.Resolve<IClientConfigManager>();
			var cliCfg=await ClientConfigManager.EnsureEntity(
				await ClientConfigManager.QuerySingleEntityIdent(new ClientConfigQueryArgument { Name = "无限访问" }),
				() => new ClientConfigEditable{},
				e =>
				{
					e.Name = "无限访问";
					e.Grants = allGrants;
					e.AllowedGrantTypes = new[]
					{
						"ClientCredentials"
					};
				}
				);
			var ClientManager = ServiceProvider.Resolve<IClientManager>();
			await ClientManager.EnsureEntity(
				await ClientManager.QuerySingleEntityIdent(new ClientQueryArgument { Name = "内部系统" }),
				() => new ClientInternal
				{
				},
				e =>
				{
					e.Name = "内部系统";
					e.Title= "内部系统";
					e.Secret = "123456";
					e.ClientConfigId = cliCfg.Id;
				}
				);

			var RoleManager = ServiceProvider.Resolve<IRoleManager>();
			await RoleManager.EnsureEntity(
				await RoleManager.QuerySingleEntityIdent(new RoleQueryArgument { Id=new ObjectKey<string> { Id = "superadmin" } }),
				() => new RoleEditable
				{
					Id="superadmin"				
				},
				e =>
				{
					e.Name = "超级管理员";
					e.Grants = allGrants;
					
				}
			);

			var UserManager = ServiceProvider.Resolve<IUserManager>();
			var PasswordHasher= ServiceProvider.Resolve<IPasswordHasher>();
			var stamp = Bytes.Random(16);
			var superadmin = await UserManager.UserEnsure(
				"acc",
				"superadmin",
				"system",
				"超级管理员",
				new[] {"superadmin"}
				);

		}
	}
}

