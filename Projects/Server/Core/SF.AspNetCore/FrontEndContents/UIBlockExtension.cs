using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SF.AspNetCore;
using SF.Core.Logging;
using SF.Core.ServiceManagement;
using SF.Management.FrontEndContents;
using SF.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SF.AspNetCore.Mvc
{
    public static class UIBlockExtension
    {
		static void Log(IServiceProvider ServiceProvider, string message, Exception e = null)
		{
			var logservice = ServiceProvider.Resolve<ILogService>();
			if (logservice == null)
				return;
			var logger = logservice.GetLogger("UI管理器");
			logger.Error(e, message);
		}
		
		public static void UIBlock(this IHtmlHelper htmlHelper, string blockName)
		{
			var req = htmlHelper.ViewContext.HttpContext.Request;
			var serviceProvider = htmlHelper.ViewContext.HttpContext.RequestServices;
			var pageContext = (IPageRenderContext)htmlHelper.ViewBag.UIPageRenderContext;
			if (pageContext == null)
			{
				Log(serviceProvider ,$"页面没有载入UI管理器数据,块名：{blockName} URI:{req.Path}");
				return;
			}
			var block = pageContext.GetBlockRenderContext(blockName);
			if (block == null)
			{
				Log(serviceProvider ,$"UI管理器数据中找不到块：{blockName} URI:{req.Uri()}");
				return;
			}
			var providerResolver = serviceProvider.Resolve<NamedServiceResolver<IRenderProvider>>();
			foreach (var ctn in block.BlockContents)
			{
				IRenderProvider provider = null;
				try
				{
					provider = providerResolver(ctn.RenderProvider);
					if (provider == null)
					{
						Log(serviceProvider, $"UI管理器找不到渲染提供者：UI块：{blockName} 渲染提供者:{ctn.RenderProvider} 视图:{ctn.RenderView} 内容区域:{ctn.Name}  URI:{req.Uri()}");
						continue;
					}
				}
				catch(Exception e){
					Log(serviceProvider, $"UI管理器查找渲染提供者时发生异常：UI块：{blockName} 渲染提供者:{ctn.RenderProvider} 视图:{ctn.RenderView} 内容区域:{ctn.Name}  URI:{req.Uri()} 异常:{e.Message}",e);
					continue;
				}
				try
				{
					provider.Render(htmlHelper, ctn.RenderView, ctn.RenderConfig, ctn, ctn.Data);
				}
				catch (Exception e){
					Log(serviceProvider, $"UI管理器渲染时发生异常：UI块：{blockName} 渲染提供者:{ctn.RenderProvider}  视图:{ctn.RenderView} 内容区域:{ctn.Name}  URI:{req.Uri()} 异常:{e.Message}", e);
					continue;
				}
			}
		}
	}
}
