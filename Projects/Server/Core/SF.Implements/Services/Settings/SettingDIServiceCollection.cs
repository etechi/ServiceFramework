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
		public static IServiceCollection AddSettingService(
			this IServiceCollection sc
			)
		{
			sc.AddManaged(
				typeof(ISettingService<>),
				typeof(SettingService<>),
				ServiceImplementLifetime.Scoped
				);

			return sc;
		}
	}
}
