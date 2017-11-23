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
using System;
using System.Linq;
using SF.Sys.Settings;
using SF.Sys.Services;
using SF.Sys.Logging;

namespace SF.Sys.AspNetCore.Mvc
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
