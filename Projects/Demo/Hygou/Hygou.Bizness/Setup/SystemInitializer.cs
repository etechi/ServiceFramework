﻿using SF.Biz.Products;
using SF.Core.Hosting;
using SF.Core.ServiceManagement;
using SF.Core.ServiceManagement.Management;
using SF.Management.FrontEndContents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hygou.Setup
{
	public class SystemInitializer
	{
		public static async Task<DataModels.User> EnsureSysSeller(IDIScope scope)
		{
			var u = await scope.UserEnsure("sysseller", "系统卖家", "13011110002", "system", new[] { "admin", "seller", "provider" });
			return u;
		}
		public static async Task Initialize(IServiceProvider ServiceProvider, EnvironmentType EnvType)
		{
			//scope.Resolve<IAuditService>().Disabled = true;

			//await InitRoles(scope);
			//var sysadmin = await EnsureSysAdmin(scope);
			var sysseller = await EnsureSysSeller(scope);
			//await InitAccounting(scope);
			//await SysServiceInitializer.Initialize(scope, EnvType);


			//await InitExtAuth(scope);
			//await InitPayments(scope);

			//await ResetSettings(scope, sysadmin.Id, sysseller.Id, EnvType);
			long? ScopeId = 0;
			var tailDocContents = await ServiceProvider.Invoke((IServiceInstanceManager sim)=>DocInitializer.DocEnsure(ServiceProvider,sim,ScopeId));
			
			var prdtypes = await ServiceProvider.Invoke((IProductTypeManager m)=>ProductTypeInitializer.Create(m));
			var colls = await ServiceProvider.Invoke((ICategoryManager cm,IItemManager im)=> ProductCategoryInitializer.Create(cm, im, sysseller.Id, prdtypes));

			var prdctns = await ServiceProvider.Invoke((IContentManager cm)=> ProductContentInitializer.Create(cm, colls));

			await ServiceProvider.Invoke((
				IServiceInstanceManager sim,
				IContentManager<Content> ContentManager,
				ISiteTemplateManager SiteTemplateManager,
				ISiteManager SiteManager,
				IItemService ItemService)=>
				PCSiteInitializer.PCSiteEnsure(
					sim,
					ScopeId,
					ContentManager,
					SiteTemplateManager,
					SiteManager,
					ItemService,
					prdctns, 
					colls, 
					tailDocContents.Item1, 
					tailDocContents.Item2
					));
			//await scope.MobileSiteEnsure(prdctns, colls);

			//await InitDefaultAdminDeliveryAddress(scope, sysseller.Id);

			//await PromotionInitializer.Init(scope, EnvType);

			//await NotificationInitializer.Init(scope, EnvType);

			//await SecurityInitializer.Init(scope);
		}

	}
}