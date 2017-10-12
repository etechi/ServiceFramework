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

using System;
using System.Collections.Generic;
using System.Linq;
using SF.Core.ServiceManagement;
using SF.Metadata;
using SF.Core.TaskServices;
using SF.Core.Hosting;
using SF.Core.Logging;
using Microsoft.Extensions.Logging;
using SF.Management.MenuServices.Models;
using SF.Core.ServiceManagement.Management;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using SF.Data.EntityFrameworkCore;
using Hygou.Setup;
using SF.Services.Settings;

namespace Hygou
{	
	public static class HygouDIExtensions
	{
		public static IServiceCollection AddHygouServices(this IServiceCollection sc,EnvironmentType envType)
		{
			sc.AddSetting<HygouSetting>();
			sc.AddInitializer("data", "初始化Hygou数据", sp => SystemInitializer.Initialize(sp, envType));
			sc.AddInitializer("product", "初始化Hygou产品", sp => SampleImporter.ImportSamples(sp));
			return sc;
		}
	
	}
}