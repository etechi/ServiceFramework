using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
	}
}
