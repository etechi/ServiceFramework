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
    public static class HtmlHelperExtension
    {
        static readonly Type DIScopeKey = typeof(IServiceScope);

      
		static void Log(IServiceProvider ServiceProvider,string message,Exception e=null)
		{
			var logservice = ServiceProvider.Resolve<ILogService>();
			if (logservice == null)
				return;
			var logger = logservice.GetLogger("UI管理器");
			logger.Error(e,message);
		}
		public static T Setting<T>(this IHtmlHelper htmlHelper)
		{
			var serviceProvider = htmlHelper.ViewContext.HttpContext.RequestServices;
			return serviceProvider.Resolve<ISettingService<T>>().Value;
		}

		public static string HttpRoot(this IHtmlHelper htmlHelper)
		{
			var httpSetting = htmlHelper.Setting<HttpSetting>();
			return httpSetting.HttpRoot ?? "/";
		}

		public static string ResBase(this IHtmlHelper htmlHelper,bool Image=true)
		{
			var httpSetting = htmlHelper.Setting<HttpSetting>();
			return (Image?httpSetting.ImageResBase:null)?? httpSetting.ResBase ?? httpSetting.HttpRoot ?? "/";
		}

		
		public static string Res(this IHtmlHelper htmlHelper, string url, bool Image = true)
		{
			var ss = htmlHelper.Setting<HttpSetting>();
			if (url[0] == '~')
				url = url.Substring(2);
			else if (url[0] == '/')
				url = url.Substring(1);
			return ((Image ? ss.ImageResBase ?? ss.ResBase : ss.ResBase) ?? ss.HttpRoot ?? "/") + url;
		}
		public static string Media(this IHtmlHelper htmlHelper, string id, string format = null)
		{
			if (id == null)
				return string.Empty;
			if (id.Contains('/'))
				return id;
			var url = "/r/" + id;
			if (format != null)
				url += "?format=" + format;
			return htmlHelper.Res(url);
		}
		public static Uri GetCurrentUrl(this IHtmlHelper htmlHelper)
		{
			return htmlHelper.ViewContext.HttpContext.Request.Uri();
		}
	}

}
