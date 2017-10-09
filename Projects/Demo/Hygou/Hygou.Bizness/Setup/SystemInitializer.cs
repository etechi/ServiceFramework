using SF.Auth.Identities;
using SF.Biz.Products;
using SF.Core;
using SF.Core.Hosting;
using SF.Core.ServiceManagement;
using SF.Core.ServiceManagement.Management;
using SF.Management.FrontEndContents;
using SF.Users.Members;
using SF.Users.Members.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hygou.Setup
{
	public class SystemInitializer
	{
		public static async Task<MemberInternal> EnsureSysSeller(IServiceProvider sp)
		{
			var svc = sp.Resolve<IMemberService>();
			var u = await svc.MemberEnsure(
					sp,
					"sysseller",
					"系统卖家",
					"13011110002",
					"system",
					new[]{"admin", "seller", "provider" }
					);
			return u;
		}
		public static async Task Initialize(IServiceProvider ServiceProvider, EnvironmentType EnvType)
		{
			//scope.Resolve<IAuditService>().Disabled = true;

			//await InitRoles(scope);
			//var sysadmin = await EnsureSysAdmin(scope);
			var sysseller = await EnsureSysSeller(ServiceProvider);
			//await InitAccounting(scope);
			//await SysServiceInitializer.Initialize(scope, EnvType);


			//await InitExtAuth(scope);
			//await InitPayments(scope);

			//await ResetSettings(scope, sysadmin.Id, sysseller.Id, EnvType);
			long? ScopeId = null;
			var tailDocContents = await ServiceProvider.Invoke((IServiceInstanceManager sim)=>DocInitializer.DocEnsure(ServiceProvider,sim,ScopeId));
			
			var prdtypes = await ServiceProvider.Invoke((IProductTypeManager m)=>ProductTypeInitializer.Create(m));
			var colls = await ServiceProvider.Invoke(((ICategoryManager cm,IItemManager im, IServiceInstanceManager sim) arg) => 
				ProductCategoryInitializer.Create(arg.sim, arg.cm, arg.im, sysseller.Id, ScopeId, prdtypes));

			var prdctns = await ServiceProvider.Invoke((IContentManager cm)=> ProductContentInitializer.Create(cm, colls));

			await ServiceProvider.Invoke(((
				IServiceInstanceManager sim,
				IContentManager ContentManager,
				ISiteTemplateManager SiteTemplateManager,
				ISiteManager SiteManager,
				IItemService ItemService) arg)=>
				PCSiteInitializer.PCSiteEnsure(
					arg.sim,
					ScopeId,
					arg.ContentManager,
					arg.SiteTemplateManager,
					arg.SiteManager,
					arg.ItemService,
					prdctns, 
					colls, 
					tailDocContents.Item1, 
					tailDocContents.Item2,
					colls.MainProductCategoryId
					));
			//await scope.MobileSiteEnsure(prdctns, colls);

			//await InitDefaultAdminDeliveryAddress(scope, sysseller.Id);

			//await PromotionInitializer.Init(scope, EnvType);

			//await NotificationInitializer.Init(scope, EnvType);

			//await SecurityInitializer.Init(scope);
		}

	}
}
