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

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.AspNetCore;
using SF.Services.Settings;
using Microsoft.AspNetCore.Authorization;
using SF.Management.BizAdmins;
using SF.Core.ServiceManagement;
using SF.Auth.Users;
using SF.Auth.Users;
using SF.Management.SysAdmins;

namespace Hygou.Site.Controllers 
{
	public class AdminModel
	{
		public long IdentityServiceId { get; set; }
		public string Type { get; set; }
	}
	public class AdminController : Controller
	{
		IServiceProvider ServiceProvider { get; }
		public AdminController(IServiceProvider ServiceProvider)
		{
			this.ServiceProvider = ServiceProvider;
			
		}
		long GetIdentityServiceId<I>()
			where I:class,IUserService
		{
			var us = ServiceProvider.Resolve<I>();
			var id = ((IManagedServiceWithId)us).ServiceInstanceId;
			var iis = ServiceProvider.Resolve<IUserService>(null, id);
			return ((IManagedServiceWithId)iis).ServiceInstanceId.Value;
		}
		public ActionResult Index()
		{
			return RedirectToAction("bizness");
		}
		[Authorize("admin-bizness")]
		public ActionResult Bizness()
		{
			return View(
				"Admin",
				new AdminModel
				{
					IdentityServiceId = GetIdentityServiceId<IBizAdminService>(),
					Type = "bizness"
				}
			);
		}
		public ActionResult BiznessSignin()
		{
			return Bizness();
		}
		//[Authorize("admin-system")]
		public ActionResult System()
		{
			return View(
				"Admin",
				new AdminModel
				{
					IdentityServiceId = GetIdentityServiceId<ISysAdminService>(),
					Type = "system"
				}
			);
		}
		public ActionResult SystemSignin()
		{
			return System();
		}
	}
}
