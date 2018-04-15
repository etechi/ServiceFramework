
using System;
using System.Threading.Tasks;
using SF.Auth.IdentityServices.Managers;
using SF.Sys.Hosting;
using SF.Sys.Services;
using SF.Sys.Entities;
using SF.Sys.Services.Management;
using SF.Sys.Settings;
using SF.Common.FrontEndContents;
using System.Collections.Generic;
using SF.Sys;

namespace SFShop.Setup
{
	public class SystemInitializer
	{
		//public static async Task<long> EnsureAdmin(IServiceProvider sp)
		//{
		//	//var svc = sp.Resolve<IAdminManager>();
		//	//var u = await svc.AdminEnsure(sp,"sysadmin", "系统管理员", "13000010001", "system123", new[] { "sysadmin","admin"});
		//	//return u.Id;
		//	var re=await sp.Resolve<IUserManager>().QuerySingleEntityIdent(
		//		new UserQueryArgument {
		//			MainCredential = "superadmin",
		//			MainClaimTypeId = "acc"
		//		});
		//	return re.Id;
		//}

		public static async Task ServiceInitialize(IServiceProvider ServiceProvider, EnvironmentType EnvType)
		{
			long? ScopeId = null;
			await ServiceProvider.WithServices((IServiceInstanceManager sim) =>
				DocInitializer.DocServiceEnsure(ServiceProvider, sim, ScopeId)
				);

		}
		public static async Task DataInitialize(
			IServiceProvider ServiceProvider,
			EnvironmentType EnvType,
			IReadOnlyDictionary<string, string> Args
			)
		{
			long? ScopeId = null;
			//scope.Resolve<IAuditService>().Disabled = true;

			//await InitRoles(scope);
			await TextMessageInitializer.DataInitialize(ServiceProvider, EnvType);

			//var sysadmin = await EnsureAdmin(ServiceProvider);

			//await ServiceProvider.Invoke(async (IServiceInstanceManager sim) =>
			//	await sim.UpdateSetting<AppSetting>(
			//	   ScopeId,
			//	   s =>
			//	   {
			//		   s.DefaultSellerId = sysseller;
			//	   })
			//	   );
			//await InitAccounting(scope);
			//await SysServiceInitializer.Initialize(scope, EnvType);


			//await InitExtAuth(scope);
			//await InitPayments(scope);

			//await ResetSettings(scope, sysadmin.Id, sysseller.Id, EnvType);
			var tailDocContents = await ServiceProvider.WithServices(
				(IServiceInstanceManager sim) =>
					DocInitializer.DocDataEnsure(ServiceProvider, sim, ScopeId)
				);


			//await ServiceProvider.WithServices(((
			//	IServiceInstanceManager sim,
			//	IContentManager ContentManager,
			//	ISiteTemplateManager SiteTemplateManager,
			//	ISiteManager SiteManager) arg) =>
			//	PCSiteInitializer.PCSiteEnsure(
			//		arg.sim,
			//		ScopeId,
			//		arg.ContentManager,
			//		arg.SiteTemplateManager,
			//		arg.SiteManager,
			//		tailDocContents.Item1,
			//		tailDocContents.Item2,
			//		0,
			//		0,
			//		0,
			//		0
			//		));
			//await scope.MobileSiteEnsure(prdctns, colls);

			//await InitDefaultAdminDeliveryAddress(scope, sysseller.Id);

			//await PromotionInitializer.Init(scope, EnvType);

			//await NotificationInitializer.Init(scope, EnvType);

			//await SecurityInitializer.Init(scope);

			if (Args == null || !Args.ContainsKey("host"))
				throw new PublicArgumentException("必须指定host参数");

			await ServiceProvider.WithServices(
				async (IServiceInstanceManager sim) =>
				{
					var host = Args["host"];
					await sim.UpdateSetting<HttpSetting>(null, s =>
					{
						s.Domain = host;
					});
					if (Args.TryGetValue("ext-ident-postfix", out var ExtIdentPostfix))
						await sim.UpdateSetting<SystemSetting>(null, s =>
						{
							s.ExtIdentPostfix = ExtIdentPostfix ?? "";
						});
				});
		}

	}
}
