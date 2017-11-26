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

using Microsoft.AspNetCore.Mvc.Rendering;
using SF.Common.FrontEndContents;
using SF.Sys.Services;
using System;

namespace SF.Sys.AspNetCore.Mvc
{
	public static class UIBlockExtension
    {
		//internal static void Log(IServiceProvider ServiceProvider, string message, Exception e = null)
		//{
		//	var logservice = ServiceProvider.Resolve<ILogService>();
		//	if (logservice == null)
		//		return;
		//	var logger = logservice.GetLogger("UI管理器");
		//	logger.Error(e, message);
		//}
		
		public static void UIBlock(this IHtmlHelper htmlHelper, string blockName)
		{
			var req = htmlHelper.ViewContext.HttpContext.Request;
			var serviceProvider = htmlHelper.ViewContext.HttpContext.RequestServices;
			var pageContext = (IPageRenderContext)htmlHelper.ViewBag.UIPageRenderContext;
			if (pageContext == null)
				throw new NotSupportedException($"页面没有载入UI管理器数据,块名：{blockName} URI:{req.Path}");

			var block = pageContext.GetBlockRenderContext(blockName);
			if (block == null)
				throw new NotSupportedException($"UI管理器数据中找不到块：{blockName} URI:{req.Uri()}");

			var providerResolver = serviceProvider.Resolve<NamedServiceResolver<IRenderProvider>>();
			foreach (var ctn in block.BlockContents)
			{
				IRenderProvider provider = null;
				try
				{
					provider = providerResolver(ctn.RenderProvider);
					if (provider == null)
					{
						throw new NotSupportedException($"UI管理器找不到渲染提供者：UI块：{blockName} 渲染提供者:{ctn.RenderProvider} 视图:{ctn.RenderView} 内容区域:{ctn.Name}  URI:{req.Uri()}");
					}
				}
				catch(Exception e){
					throw new InvalidOperationException(
						$"UI管理器查找渲染提供者时发生异常：UI块：{blockName} 渲染提供者:{ctn.RenderProvider} 视图:{ctn.RenderView} 内容区域:{ctn.Name}  URI:{req.Uri()} 异常:{e.Message}",e);
				}
				try
				{
					provider.Render(htmlHelper, ctn.RenderView, ctn.RenderConfig, ctn, ctn.Data);
				}
				catch (Exception e){
					throw new InvalidOperationException(
						$"UI管理器渲染时发生异常：UI块：{blockName} 渲染提供者:{ctn.RenderProvider}  视图:{ctn.RenderView} 内容区域:{ctn.Name}  URI:{req.Uri()} 异常:{e.Message}", e);
				}
			}
		}
	}
}
