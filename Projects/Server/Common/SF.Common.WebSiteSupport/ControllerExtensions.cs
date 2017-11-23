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
using SF.Common.FrontEndContents;
using SF.Sys.Services;
using System.Threading.Tasks;

namespace SF.Sys.AspNetCore
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
