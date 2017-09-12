using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SF.Core.Hosting;
using SF.Core.ServiceManagement;
using SF.Services.FrontEndContents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF
{
	public static class ControllerExtensions
	{
		public static async Task LoadUIPageBlocks(this Controller Controller, string page, string site = null)
		{
			Ensure.HasContent(page, nameof(page));
			var req = Controller.Request;
			var RenderContextCreator =  req.HttpContext.RequestServices.Resolve<IRenderContextCreator>();

			int tmpl_id;
			req.Cookies.TryGetValue("uim-tmpl",out var cookie);
			IPageRenderContext ctx;
			if (cookie.HasContent() && int.TryParse(cookie, out tmpl_id))
			{
				ctx = await RenderContextCreator.CreatePageContext(tmpl_id, page, null);
			}
			else
			{
				if (site == null) site = "main";
				ctx = await RenderContextCreator.CreatePageContext(site, page, null);
			}
			Controller.ViewBag.UIPageRenderContext = ctx;
		}

	}
}
