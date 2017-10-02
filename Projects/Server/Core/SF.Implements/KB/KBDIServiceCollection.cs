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
using SF.KB.Mime;
using SF.KB.Mime.Providers;
using SF.KB.PhoneNumbers;
using SF.KB.PhoneNumbers.Providers;
using SF.KB.DeviceDetector.Providers;
using SF.Clients;

namespace SF.Core.ServiceManagement
{
	public static class KBDIServiceCollectionExtension
	{
		public static IServiceCollection AddDefaultMimeResolver(
			this IServiceCollection sc
			)
		{
			sc.AddSingleton<IMimeResolver, DefaultMimeResolver>();
			return sc;
		}
		public static IServiceCollection AddDefaultPhoneNumberValidator(
		   this IServiceCollection sc
		   )
		{
			sc.AddSingleton<IPhoneNumberValidator, DefaultPhoneNumberValidator>();
			return sc;
		}
		public static IServiceCollection AddDefaultDeviceDetector(
		   this IServiceCollection sc
		   )
		{
			sc.AddSingleton<IClientDeviceTypeDetector, DefaultDeviceTypeDetector>();
			return sc;
		}
		public static IServiceCollection AddDefaultKBServices(this IServiceCollection sc)
		{
			sc.AddDefaultMimeResolver();
			sc.AddDefaultPhoneNumberValidator();
			sc.AddDefaultDeviceDetector();
			return sc;
		}
	}
}
