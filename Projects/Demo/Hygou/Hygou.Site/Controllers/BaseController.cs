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
using SF.Core.ServiceManagement;
using SF.Management.FrontEndContents;

namespace Hygou.Site.Controllers
{
	public class BaseController : Controller
	{
		public BaseController()
		{
		}


		//protected int CurrentUserId
		//{
		//	get
		//	{
		//		return User.Identity.GetId<int>();
		//	}
		//}
		//protected async Task<DataModels.User> LoadCurrentUser()
		//{
		//	var uid = CurrentUserId;
		//	if (uid == 0) return null;
		//	var UserManager = DIScope.Resolve<UserManager>();
		//	var user = await UserManager.FindByIdAsync(uid);
		//	ViewBag.CurrentUser = user;

		//          var BonusPointService = DIScope.Resolve<IBonusPointService>();
		//          ViewBag.BonusPointStatus = await BonusPointService.GetWithDefault(uid);

		//          try
		//          {
		//              var PromotionService = DIScope.Resolve<IPromotionService>();
		//              ViewBag.IsDailyAttanded = await PromotionService.IsDailyAttended(uid);
		//          }
		//          catch {
		//              ViewBag.IsDailyAttanded = false;
		//          }
		//          return user;
		//}
		public virtual string DefaultSiteIdent { get { return "main"; } }

		public async Task LoadUIPageBlocks(string page, string site = null)
		{
			//ServiceProtocol.Ensure.HasContent(page, nameof(page));

			var RenderContextCreator = HttpContext.RequestServices.Resolve<IRenderContextCreator>();

			long tmpl_id;
			Request.Cookies.TryGetValue("uim-tmpl",out var cookie);
			IPageRenderContext ctx;
			if (cookie != null && long.TryParse(cookie, out tmpl_id))
			{
				ctx = await RenderContextCreator.CreatePageContext(tmpl_id, page, null);
			}
			else
			{
				if (site == null) site = DefaultSiteIdent;
				ctx = await RenderContextCreator.CreatePageContext(site, page, null);
			}
			ViewBag.UIPageRenderContext = ctx;
		}

	}
}
