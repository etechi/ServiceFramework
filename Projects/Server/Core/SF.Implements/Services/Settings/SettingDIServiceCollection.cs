using SF.Core;
using SF.Core.Caching;
using SF.Metadata;
using SF.Core.Drawing;
using SF.Core.NetworkService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SF.Services.Media;
using SF.Core.Hosting;
using SF.Core.ServiceManagement.Management;
using SF.Services.Media.Storages;
using System.Reflection;
using SF.Services.Settings;

namespace SF.Core.ServiceManagement
{

	public static class SettingDIServiceCollectionExtension
	{
		
		public static IServiceCollection AddSystemSettings(
			this IServiceCollection sc
			)
		{
			sc.AddSetting<AppSiteSetting>();
			sc.AddSetting<CustomServiceSetting>();
			sc.AddSetting<DebugSetting>();
			sc.AddSetting<HttpSetting>();
			sc.AddSetting<SystemSetting>();
			return sc;
		}
	}
}
