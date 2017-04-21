﻿using SF.Core;
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
using SF.Core.ManagedServices;
using SF.Core.ManagedServices.Admin;
using SF.Core.Hosting;
using SF.KB.Mime;
using SF.KB.Mime.Providers;
using SF.KB.PhoneNumbers;
using SF.KB.PhoneNumbers.Providers;

namespace SF.Core.DI
{
	public static class KBDIServiceCollectionExtension
	{
		public static IDIServiceCollection UseDefaultMimeResolver(
			this IDIServiceCollection sc
			)
		{
			sc.AddSingleton<IMimeResolver, DefaultMimeResolver>();
			return sc;
		}
		public static IDIServiceCollection UseDefaultPhoneNumberValidator(
		   this IDIServiceCollection sc
		   )
		{
			sc.AddSingleton<IPhoneNumberValidator, DefaultPhoneNumberValidator>();
			return sc;
		}
	}
}