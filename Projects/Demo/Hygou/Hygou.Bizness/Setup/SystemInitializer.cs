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
using SF.Biz.Products;
using SF.Core;
using SF.Core.Hosting;
using SF.Core.ServiceManagement;
using SF.Core.ServiceManagement.Management;
using SF.Management.FrontEndContents;
using SF.Services.Settings;
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
		public static async Task<long> EnsureSysAdmin(IServiceProvider sp)
		{
			var u = await scope.UserEnsure("sysadmin", "系统管理员", "13011110001", "temsys", new[] { "sysadmin", "admin", "special" });

			await scope.UserEnsure("超级管理员01", "超级管理员01", "13011120001", "system123", new[] { "sysadmin", "admin", "special" });
			await scope.UserEnsure("超级管理员02", "超级管理员02", "13011120002", "system123", new[] { "sysadmin", "admin", "special" });
			await scope.UserEnsure("超级管理员03", "超级管理员03", "13011120003", "system123", new[] { "sysadmin", "admin", "special" });

			await scope.UserEnsure("次级管理员01", "次级管理员01", "13011130001", "system123", new[] { "sysadmin", "admin" });
			await scope.UserEnsure("次级管理员02", "次级管理员02", "13011130002", "system123", new[] { "sysadmin", "admin" });
			await scope.UserEnsure("次级管理员03", "次级管理员04", "13011130003", "system123", new[] { "sysadmin", "admin" });
			await scope.UserEnsure("次级管理员04", "次级管理员05", "13011130004", "system123", new[] { "sysadmin", "admin" });
			await scope.UserEnsure("次级管理员05", "次级管理员06", "13011130005", "system123", new[] { "sysadmin", "admin" });

			return u;
		}

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
			long? ScopeId = null;
			//scope.Resolve<IAuditService>().Disabled = true;

			//await InitRoles(scope);
			//var sysadmin = await EnsureSysAdmin(scope);
			var sysseller = await EnsureSysSeller(ServiceProvider);

			await ServiceProvider.Invoke(async (IServiceInstanceManager sim) =>
				await sim.UpdateSetting<HygouSetting>(
				   ScopeId,
				   s =>
				   {
					   s.DefaultSellerId = sysseller.Id;
				   })
				   );
			//await InitAccounting(scope);
			//await SysServiceInitializer.Initialize(scope, EnvType);


			//await InitExtAuth(scope);
			//await InitPayments(scope);

			//await ResetSettings(scope, sysadmin.Id, sysseller.Id, EnvType);
			var tailDocContents = await ServiceProvider.Invoke((IServiceInstanceManager sim)=>DocInitializer.DocEnsure(ServiceProvider,sim,ScopeId));
			
			var prdtypes = await ServiceProvider.Invoke((IProductTypeManager m)=>ProductTypeInitializer.Create(m));
			var colls = await ServiceProvider.Invoke(((IProductCategoryManager cm,IProductItemManager im, IServiceInstanceManager sim) arg) => 
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
