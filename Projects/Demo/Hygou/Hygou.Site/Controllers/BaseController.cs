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
