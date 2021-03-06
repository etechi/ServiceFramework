﻿#region Apache License Version 2.0
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using SF.Sys.Linq;
using Microsoft.Extensions.Configuration;
using System.IO;
using SF.Sys.Hosting;

namespace SF.Sys.Services
{
	public static class MSConfigrationExtension
	{
		public class Configuration : Settings.IConfiguration
		{
			public IConfiguration Config { get; }
			public Configuration(IConfiguration Config)
			{
				this.Config = Config;
			}
			public string GetValue(string Path)
			{
				return Config[Path];
			}
		}

		public static IServiceCollection AddConfiguration(this IServiceCollection sc)
		{
			return sc.AddSingleton<Settings.IConfiguration, Configuration>();
		}
		public static IServiceCollection AddMSConfiguration(this IServiceCollection sc)
		{
			sc.AddSingleton(sp => {
				//var pr = sp.Resolve<IFilePathResolver>();

				var configuration =(IConfiguration) new ConfigurationBuilder()
					.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
					.AddJsonFile("appsettings.json")
					.Build();
				return configuration;
			});
			return sc;
		}
	}
}
