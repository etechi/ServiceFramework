using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.AspNetCore;
using SF.Services.Settings;

namespace Hygou.Site.Controllers 
{
	public class HomeController : BaseController
	{
		static System.Text.RegularExpressions.Regex _reg_mobile_browser = new System.Text.RegularExpressions.Regex(
			"(iphone|ios|android|mini|mobile|mobi|Nokia|Symbian|iPod|iPad|Windows\\s+Phone|MQQBrowser|wp7|wp8|UCBrowser7|UCWEB|360\\s+Aphone\\s+Browser)",
			System.Text.RegularExpressions.RegexOptions.IgnoreCase
			);

		ISettingService<HttpSetting> HttpSetting { get; }

		public HomeController(ISettingService<HttpSetting> HttpSetting)
        {
            this.HttpSetting = HttpSetting;
        }

        public async Task<ActionResult> Index()
		{
            if(HttpSetting.Value.HttpsMode && Request.Scheme == "http")
            {
                var ub = new UriBuilder(Request.Uri());
                ub.Scheme = "https";
                ub.Port = 443;
                return Redirect(ub.Uri.ToString());
            }

            if (_reg_mobile_browser.Match(Request.Headers.UserAgent()).Success)
            {
				var uri = "/m";
				if (Request.QueryString.HasValue)
				{
					var query = Request.QueryString.Value;
					if (!string.IsNullOrEmpty(query) && query != "?")
						uri += query;
				}
                return Redirect(uri);
            }
			ViewBag.ShowCatMenu = true;
			await this.LoadUIPageBlocks("首页");
			return View();
		}
		public ActionResult Admin()
		{
			return View();
		}
	}
}
