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

using System.Collections.Generic;
using System.Threading.Tasks;
using SF.Auth.IdentityServices.Managers;
using SF.Auth.IdentityServices.Models;
using SF.Auth.IdentityServices;
using SF.Auth.IdentityServices.Internals;
using SF.Auth.IdentityServices.UserCredentialProviders;
using System;
using System.Linq;
using SF.Sys.Services;
using SF.Sys.Services.Management;
using SF.Sys.Entities;
using SF.Sys.Linq;
using SF.Sys.Auth;
using SF.Auth.IdentityServices.Externals;
using System.Security.Claims;
using SF.Sys.Data;
using SF.Sys.Events;
using SF.Sys.NetworkService;
using SF.Sys.Reflection;
using System.Reflection;
using System.ComponentModel;
using SF.Sys.Comments;
using SF.Auth.IdentityServices.DataModels;
using SF.Sys.TimeServices;
using SF.Sys.Collections.Generic;
using SF.Sys.BackEndConsole;

namespace SF.Sys.Services
{
	
	public static class IdentityServiceDIExtension 
	{
		public static IServiceCollection AddNotImplementedAccessTokenHandler(this IServiceCollection sc)
		{
			sc.AddSingleton<IAccessTokenGenerator, NotImplementedIAccessTokenGenerator>();
			sc.AddSingleton<IAccessTokenValidator, NotImplementedIAccessTokenValidator>();
			return sc;
		}
		public static IServiceCollection AddIdentityServices(
			this IServiceCollection sc,
			string TablePrefix=null,
			bool VerifyCodeDisabled=false,
			string DefaultCredentialProvider=null,
			string DefaultUserIcon=null
			)
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
					.Add<IGrantManager, GrantManager>("AuthGrant", "授权", typeof(Grant))
					.Add<IUserManager, UserManager>("AuthUser", "用户", typeof(UserInternal),typeof(SF.Sys.Auth.User))
					.Add<IScopeManager, ScopeManager>("AuthScope", "授权范围", typeof(ScopeInternal))
				//.Add<IDocumentService, DocumentService>()
				);
			sc.AddSingleton<RoleGrantCache>();
			sc.AddSingleton<ServiceMethodAuthorizeCache>();
			sc.AddSingleton<IAuthService, AuthService>();
			sc.AddSingleton<IInterfaceAuthService>(sp => (AuthService)sp.Resolve<IAuthService>());

			sc.AddManagedScoped<IUserService, UserService>();
			sc.AddScoped(sp => (IAuthSessionService)sp.Resolve<IUserService>());
			
			sc.AddManagedScoped<IUserCredentialProvider, PhoneNumberUserCredentialProvider>();
			sc.AddManagedScoped<IUserCredentialProvider, LocalUserCredentialProvider>();
			sc.AddManagedScoped<IUserCredentialProvider, AdminAccountCredentialProvider>();
			sc.AddManagedScoped<IUserCredentialStorage, UserCredentialStorage>();
			sc.AddTransient<IUserStorage>(sp => sp.Resolve<IUserManager>());
			sc.AddSingleton<IUserProfileService, UserProfileService>();
			sc.AddSingleton<IClientExtAuthService, ClientExtAuthService>();
			sc.AddScoped<IClientExtAuthService, ClientExtAuthService>();
			sc.AddScoped<IPageExtAuthService, PageExtAuthService>();
			sc.AddManagedScoped<IExternalAuthorizationProvider, TestOAuth2Provider>();

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
				TablePrefix??"Auth",
				typeof(SF.Auth.IdentityServices.DataModels.DataClaimType),
				typeof(SF.Auth.IdentityServices.DataModels.DataClient ),
				typeof(SF.Auth.IdentityServices.DataModels.DataClientConfig),
				typeof(SF.Auth.IdentityServices.DataModels.DataClientClaimValue),
				typeof(SF.Auth.IdentityServices.DataModels.DataClientScope),
				typeof(SF.Auth.IdentityServices.DataModels.DataScope),
				typeof(SF.Auth.IdentityServices.DataModels.DataScopeResource),
				typeof(SF.Auth.IdentityServices.DataModels.DataOperation),
				typeof(SF.Auth.IdentityServices.DataModels.DataOperationRequiredClaim),
				typeof(SF.Auth.IdentityServices.DataModels.DataResource),
				typeof(SF.Auth.IdentityServices.DataModels.DataResourceRequiredClaim),
				typeof(SF.Auth.IdentityServices.DataModels.DataResourceSupportedOperation),
				typeof(SF.Auth.IdentityServices.DataModels.DataRole),
				typeof(SF.Auth.IdentityServices.DataModels.DataRoleClaimValue),
				typeof(SF.Auth.IdentityServices.DataModels.DataRoleGrant),
				typeof(SF.Auth.IdentityServices.DataModels.DataGrant),
				typeof(SF.Auth.IdentityServices.DataModels.DataGrantItem),
				typeof(SF.Auth.IdentityServices.DataModels.DataUser),
				typeof(SF.Auth.IdentityServices.DataModels.DataUserClaimValue),
				typeof(SF.Auth.IdentityServices.DataModels.DataUserCredential),
				typeof(SF.Auth.IdentityServices.DataModels.DataUserRole)
				);

			//sc.AddAutoEntityTest(NewDocumentManager);
			//sc.AddAutoEntityTest(NewDocumentCategoryManager);
			sc.InitServices(
				"初始化认证授权服务", 
				(sp,sim,scope)=>NewSerivces(
					sp,
					sim,
					scope, 
					VerifyCodeDisabled,
					DefaultCredentialProvider,
					DefaultUserIcon
					), 
				null,
				1000
				);

			sc.AddInitializer("data", "初始化基础数据", DataInit);

			return sc;
		}

		static async Task NewSerivces(
			IServiceProvider ServiceProvider, 
			IServiceInstanceManager sim, 
			long? ScopeId,
			bool VerifyCodeDisabled,
			string DefaultCredentialProvider,
			string DefaultUserIcon
			)
		{
			await sim.DefaultService<IUserService, UserService>(
				new
				{
					Setting = new UserServiceSetting
					{
						VerifyCodeDisabled = VerifyCodeDisabled,
						DefaultIdentityCredentialProvider = DefaultCredentialProvider,
						DefaultIcon= DefaultUserIcon
					}
				}
				)
				.WithDisplay("用户服务")
				.Ensure(ServiceProvider, ScopeId);

			await sim.DefaultService<IUserManager, UserManager>(null)
				.WithDisplay("用户")
				.WithConsolePages("系统管理/身份和权限")
				.Ensure(ServiceProvider, ScopeId);

			await sim.DefaultService<IRoleManager, RoleManager>(null)
				.WithDisplay("用户角色")
				.WithConsolePages("系统管理/身份和权限")
				.Ensure(ServiceProvider, ScopeId);
			await sim.DefaultService<IGrantManager, GrantManager>(null)
				.WithDisplay("授权管理")
				.WithConsolePages("系统管理/身份和权限")
				.Ensure(ServiceProvider, ScopeId);
			await sim.DefaultService<IResourceManager, ResourceManager>(null)
				.WithDisplay("资源")
				.WithConsolePages("系统管理/身份和权限")
				.Ensure(ServiceProvider, ScopeId);
			await sim.DefaultService<IOperationManager, OperationManager>(null)
				.WithDisplay("操作")
				.WithConsolePages("系统管理/身份和权限")
				.Ensure(ServiceProvider, ScopeId);

			await sim.DefaultService<IScopeManager, ScopeManager>(null)
				.WithDisplay("授权范围")
				.WithConsolePages("系统管理/身份和权限")
				.Ensure(ServiceProvider, ScopeId);

			await sim.DefaultService<IClientManager, ClientManager>(null)
				.WithDisplay("客户端")
				.WithConsolePages("系统管理/身份和权限")
				.Ensure(ServiceProvider, ScopeId);
			await sim.DefaultService<IClientConfigManager, ClientConfigManager>(null)
				.WithDisplay("客户端配置")
				.WithConsolePages("系统管理/身份和权限")
				.Ensure(ServiceProvider, ScopeId);

			await sim.DefaultService<IClaimTypeManager, ClaimTypeManager>(null)
				.WithDisplay("凭证申明类型")
				.WithConsolePages("系统管理/身份和权限")
				.Ensure(ServiceProvider, ScopeId);

			await sim.DefaultService<IUserCredentialStorage, UserCredentialStorage>(null)
				.Ensure(ServiceProvider, ScopeId);

			await sim.DefaultServiceWithIdent<IUserCredentialProvider, LocalUserCredentialProvider>(PredefinedClaimTypes.LocalAccount, null)
				.Ensure(ServiceProvider, ScopeId);
			await sim.DefaultServiceWithIdent<IUserCredentialProvider, AdminAccountCredentialProvider>(PredefinedClaimTypes.AdminAccount, null)
				.Ensure(ServiceProvider, ScopeId);
			await sim.DefaultServiceWithIdent<IUserCredentialProvider, PhoneNumberUserCredentialProvider>(PredefinedClaimTypes.Phone, null)
				.Ensure(ServiceProvider, ScopeId);

			
			//var allGrants = (from r in resPermissions
			//				 from o in r.AvailableOperations
			//				 select new Grant
			//				 {
			//					 OperationId = o,
			//					 ResourceId = r.Id
			//				 }
			//	 ).ToArray();


			
			

			await sim.Service<IExternalAuthorizationProvider, TestOAuth2Provider>(new { })
				.WithIdent("test")
				.Ensure(ServiceProvider, ScopeId);

		
		}
		static async Task DataInit(IServiceProvider ServiceProvider)
		{
			
			//初始化默认申明类型
			var ClaimTypeManager = ServiceProvider.Resolve<IClaimTypeManager>();
			var predefinedClaimTypes = new[] {
				(PredefinedClaimTypes.AdminAccount,"管理员账号"),
				(PredefinedClaimTypes.LocalAccount,"本地账号"),
				(PredefinedClaimTypes.Subject,"本地ID"),
				(PredefinedClaimTypes.Phone,"电话"),
				(PredefinedClaimTypes.Address,"地址"),
				(PredefinedClaimTypes.EMail,"电子邮件"),
				(PredefinedClaimTypes.Name,"姓名"),
				(PredefinedClaimTypes.Icon,"图标"),
				(PredefinedClaimTypes.Image,"头像"),
				(PredefinedClaimTypes.Country,"国家"),
				(PredefinedClaimTypes.Province,"省份"),
				(PredefinedClaimTypes.City,"城市"),
				(PredefinedClaimTypes.Sex,"性别"),

				(PredefinedClaimTypes.WeiXinOpenPlatformId,"微信开放平台"),
				(PredefinedClaimTypes.WeiXinMPId,"微信公众号"),
				(PredefinedClaimTypes.WeiXinUnionId,"微信统一ID"),
				(PredefinedClaimTypes.TestId,"测试ID")
			};
			foreach (var ct in predefinedClaimTypes)
			{
				await ClaimTypeManager.EnsureEntity(
					await ClaimTypeManager.QuerySingleEntityIdent(new ClaimTypeQueryArgument { Id = ObjectKey.From(ct.Item1) }),
					() => new ClaimType() { Id = ct.Item1 },
					ict =>
					{
						ict.Name = ct.Item2;

					}
					);
			}

			//////初始化操作
			////var OperationManager = ServiceProvider.Resolve<IOperationManager>();
			////foreach (var o in ServiceProvider.Resolve<IEnumerable<Sys.Auth.Permissions.IOperation>>()
			////	.WithFirst(
			////		new Sys.Auth.Permissions.Operation(Sys.Auth.Permissions.Operations.Read, "查看", "查看对象"),
			////		new Sys.Auth.Permissions.Operation(Sys.Auth.Permissions.Operations.Create, "新建", "新建对象"),
			////		new Sys.Auth.Permissions.Operation(Sys.Auth.Permissions.Operations.Update, "修改", "修改对象"),
			////		new Sys.Auth.Permissions.Operation(Sys.Auth.Permissions.Operations.Remove, "删除", "删除对象")
			////		))
			////{
			////	await OperationManager.EnsureEntity(
			////		new ObjectKey<string> { Id = o.Id },
			////		() => new OperationInternal
			////		{
			////			Id = o.Id
			////		},
			////		e =>
			////		{
			////			e.Name = o.Name;
			////			e.Title = o.Name;
			////			e.Description = o.Description;
			////		}
			////		);
			////}
			//////初始化资源
			////var ResourceManager = ServiceProvider.Resolve<IResourceManager>();
			////var resPermissions = ServiceProvider.Resolve<IEnumerable<Sys.Auth.Permissions.IResource>>();
			////foreach (var o in resPermissions)
			////{
			////	await ResourceManager.EnsureEntity(
			////		new ObjectKey<string> { Id = o.Id },
			////		() => new ResourceEditable
			////		{
			////			Id = o.Id
			////		},
			////		e =>
			////		{
			////			e.Name = o.Name;
			////			e.Title = o.Name;
			////			e.Description = o.Description;
			////			e.SupportedOperations = o.AvailableOperations.Select(oi => new ResourceOperationInternal
			////			{
			////				OperationId = oi

			////			});
			////		}
			////		);
			////}



			//foreach (var idres in predefinedClaimTypes)
			//{
			//	await ResourceManager.Ensure<IResourceManager, string, ResourceEditable>(
			//		"id:"+idres.Item1,
			//		r =>
			//		{
			//			r.Name =
			//			r.Title = idres.Item2;
			//			r.IsIdentityResource = true;
			//			r.RequiredClaims = new[] { new ResourceRequiredClaim { ClaimTypeId = idres.Item1 } };
			//		});
			//}
			//await ResourceManager.Ensure<IResourceManager, string, ResourceEditable>(
			//		"profile",
			//		r =>
			//		{
			//			r.Title =
			//			r.Name = "用户信息";

			//			r.IsIdentityResource = true;
			//			r.RequiredClaims = predefinedClaimTypes.Select(id => new ResourceRequiredClaim { ClaimTypeId = id.Item1 }).ToArray();
			//		});

			var scopeManager = ServiceProvider.Resolve<IScopeManager>();
			//var allResourceIds = (await ResourceManager.QueryAllAsync()).Select(i => i.Id).ToArray();
			var allResourceScope = await scopeManager.Ensure<IScopeManager, string, ScopeEditable>(
				"all",
				e =>
				{
					e.Name = "所有资源";
					//e.Resources = allResourceIds;//.Select(i => new ScopeResource { ResourceId = i });
				});


			var ClientConfigManager = ServiceProvider.Resolve<IClientConfigManager>();
			var allResConfig = await ClientConfigManager.EnsureEntity(
				await ClientConfigManager.QuerySingleEntityIdent(new ClientConfigQueryArgument { Name = "所有资源" }),
				() => new ClientConfigEditable { },
				e =>
				{
					e.Name = "所有资源";
					e.Scopes = new[]
					{
						new ClientScope{ScopeId=allResourceScope.Id}
					};
					e.AllowedGrantTypes = new[]
					{
						"password",
						"code"
					};
					e.RequireClientSecret = true;
					e.RequireConsent = true;
					e.AllowRememberConsent = true;
					e.FrontChannelLogoutSessionRequired = true;
					e.BackChannelLogoutSessionRequired = true;
					e.IdentityTokenLifetime = 300;
					e.AccessTokenLifetime = 86400;
					e.AuthorizationCodeLifetime = 300;
					e.AbsoluteRefreshTokenLifetime = 2592000;
					e.SlidingRefreshTokenLifetime = 1296000;
				}
				);
			var customerConfig = await ClientConfigManager.EnsureEntity(
				await ClientConfigManager.QuerySingleEntityIdent(new ClientConfigQueryArgument { Name = "客户终端" }),
				() => new ClientConfigEditable { },
				e =>
				{
					e.Name = "客户终端";
					e.Scopes = new[]
					{
						new ClientScope{ScopeId=allResourceScope.Id}
					};
					e.AllowedGrantTypes = new[]
					{
						"code"
					};
					e.RequireClientSecret = true;
					e.RequireConsent = true;
					e.AllowRememberConsent = true;
					e.FrontChannelLogoutSessionRequired = true;
					e.BackChannelLogoutSessionRequired = true;
					e.IdentityTokenLifetime = 300;
					e.AccessTokenLifetime = 86400;
					e.AuthorizationCodeLifetime = 300;
					e.AbsoluteRefreshTokenLifetime = 2592000;
					e.SlidingRefreshTokenLifetime = 1296000;
				}
				);

			var ClientManager = ServiceProvider.Resolve<IClientManager>();
			await ClientManager.ClientEnsure("local.internal", "内部系统", allResConfig.Id, "pass");
			await ClientManager.ClientEnsure("admin.console", "管理控制台", allResConfig.Id, "pass");

			await ClientManager.ClientEnsure("browser.pc", "PC浏览器", customerConfig.Id, "pass");
			await ClientManager.ClientEnsure("browser.wx", "微信浏览器", customerConfig.Id, "pass");
			await ClientManager.ClientEnsure("browser.wap", "移动端浏览器", customerConfig.Id, "pass");
			await ClientManager.ClientEnsure("app.android", "安卓", customerConfig.Id, "pass");
			await ClientManager.ClientEnsure("app.ios", "IOS", customerConfig.Id, "pass");
			await ClientManager.ClientEnsure("app.other", "其他浏览器", customerConfig.Id, "pass");


			var RoleManager = ServiceProvider.Resolve<IRoleManager>();

			await RoleManager.RoleEnsure(
				"superadmin",
				"超级管理员",
				true
			);
			await RoleManager.RoleEnsure(
				"admin",
				"管理员",
				true
			);


			var UserManager = ServiceProvider.Resolve<IUserManager>();
			var superadmin = await UserManager.UserEnsure(
				PredefinedClaimTypes.AdminAccount,
				"superadmin",
				"system",
				"超级管理员",
				new[] { "superadmin", "admin" }
				);

			await InitGrantAndRoles(ServiceProvider);
			foreach (var role in PredefinedRoles.Items)
			{
				if (!await RoleManager.ExistsAsync(
					new RoleQueryArgument { Id = ObjectKey.From(role) })
					)
					continue;

				for (var i = 0; i < 5; i++)
				{
					await UserManager.UserEnsure(
						PredefinedClaimTypes.AdminAccount,
						role + (i+1),
						"system",
						role,
						new[] { "admin", role }
						);
				}
			}

		}

		private static async Task InitGrantAndRoles(IServiceProvider ServiceProvider)
		{
			//创建默认角色,并授权
			var roles = new Dictionary<string, NamedItems<string>>();
			var permissions = new List<NamedItems<(string svc, string method)>>();

			var svcs = ServiceProvider.Resolve<IServiceMetadata>();
			var SigninRequired = new HashSet<(string type, string method)>();

			foreach (var svc in svcs.Services.Values)
			{
				//仅处理网络接口
				if (!svc.ServiceType.IsAnyRelatedTypeDefined(typeof(NetworkServiceAttribute)))
					continue;
				//服务上的默认授权
				var daas = svc.ServiceType
					.AllRelatedTypes()
					.SelectMany(t => t.GetCustomAttributes<DefaultAuthorizeAttribute>(true))
					.GroupBy(aa => aa.RoleIdent)
					.Select(g => g.First())
					.ToArray();

				var methods = new Dictionary<string, NamedItems<(string svc, string method)>>();

				//服务名称
				var svcName = svc.ServiceName;
				var svcTitle = svc.ServiceType.Comment().Title.LeftBefore("管理");
				//处理服务上方法默认授权
				foreach (var method in
					svc.ServiceType.AllRelatedTypes().SelectMany(it => it.AllPublicInstanceMethods())
					)
				{
					var methodName = method.Name;

					var mdaas = method.GetCustomAttributes<DefaultAuthorizeAttribute>(true);
					if (mdaas != null)
					{
						foreach (var r in mdaas.Where(mdaa => mdaa.RoleIdent != null))
							AddItem(methods, r.RoleIdent, r.RoleName, (svcName, methodName));
					}
					if (daas != null && daas.Any() || mdaas != null && mdaas.Any())
						SigninRequired.Add((svcName, method.Name));

					var rd = method.GetCustomAttribute<ReadOnlyAttribute>(true);
					if (rd != null && rd.IsReadOnly)
						AddItem(methods, "#readonly", "浏览", (svcName, methodName));
				}
				foreach (var m in methods)
				{
					m.Value.Name = svcTitle + m.Value.Name;
					permissions.Add(m.Value);

					if (m.Key.StartsWith("#"))
					{
						foreach (var daa in daas.Where(d => d.RoleIdent != null))
							AddItem(roles, daa.RoleIdent, daa.RoleName, m.Value.Name);
					}
					else
						AddItem(roles, m.Key, m.Value.Name, m.Value.Name);
				}

				var managerPermissionName = svcTitle + "管理";
				var ni = new NamedItems<(string, string)> { Name = managerPermissionName };
				ni.Add((svcName, "*"));
				permissions.Add(ni);

				foreach (var daa in daas.Where(d => d.RoleIdent != null))
					AddItem(roles, daa.RoleIdent, daa.RoleName,daa.ReadOnly? svcTitle+ "浏览": managerPermissionName);

			}
			var ds = ServiceProvider.Resolve<IDataScope>();
			var idg = ServiceProvider.Resolve<IIdentGenerator>();
			var timeService = ServiceProvider.Resolve<ITimeService>();
			var now = timeService.Now;
			var permissionIdMap = new Dictionary<string, long>();

			await ds.Use("初始化权限", async ctx =>
			{
				var permissionSet = ctx.Set<DataGrant>();
				var itemSet = ctx.Set<DataGrantItem>();
				var dbPermissions = await permissionSet.AsQueryable()
						.Include(pi => pi.Items)
						.ToDictionaryAsync(pi => pi.Name);
				foreach (var e in permissions)
				{
					var m = dbPermissions.Get(e.Name);
					if (m == null)
					{
						var id = await idg.GenerateAsync<DataGrant>();
						permissionIdMap.Add(e.Name, id);
						permissionSet.Add(new DataGrant
						{
							Id = id,
							Name = e.Name,
							Items = e.Select(pi => new DataGrantItem
							{
								GrantId = id,
								ServiceId = pi.svc,
								ServiceMethodId = pi.method 
							}).ToArray(),
							CreatedTime = now,
							UpdatedTime = now
						});
					}
					else
					{
						permissionIdMap.Add(e.Name, m.Id);
						m.UpdatedTime = now;
						itemSet.Merge(
							m.Items,
							e,
							(oi, ei) => oi.ServiceId == ei.svc && oi.ServiceMethodId == ei.method ,
							ei => new DataGrantItem { GrantId = m.Id, ServiceId = ei.svc, ServiceMethodId = ei.method }
							);
						permissionSet.Update(m);

					}
				}

				var roleSet = ctx.Set<DataRole>();
				var dbRoles = await roleSet.AsQueryable()
						.Include(pi => pi.Grants)
						.ToDictionaryAsync(pi => pi.Id);

				foreach (var r in roles)
				{
					var dbr = dbRoles.Get(r.Key);
					if (dbr == null)
					{
						roleSet.Add(new DataRole
						{
							Id = r.Key,
							Name = r.Value.Name ?? r.Key,
							Grants = r.Value.Select(pi => new DataRoleGrant
							{
								DstGrantId = permissionIdMap[pi],
								RoleId = r.Key
							}).ToArray(),
							CreatedTime = now,
							UpdatedTime = now
						});
					}
					else
					{
						dbr.Name = r.Value.Name ?? r.Key;
						dbr.UpdatedTime = now;
						ctx.Set<DataRoleGrant>().Merge(
							dbr.Grants,
							r.Value,
							(o, e) => o.DstGrantId == permissionIdMap[e],
							e => new DataRoleGrant
							{
								RoleId = dbr.Id,
								DstGrantId = permissionIdMap[e]
							}
							);
						ctx.Update(dbr);
					}
				}

				await ctx.SaveChangesAsync();

			});
		}

		class NamedItems<T>:HashSet<T>
		{
			public string Name { get; set; }
		}
		static void AddItem<T>(
			Dictionary<string, NamedItems<T>> dic,
			string key,
			string name,
			T item
			)
		{
			if (!dic.TryGetValue(key, out var l))
				dic[key] = l = new NamedItems<T>() { Name = name };
			l.Add(item);
		}

	}
}

