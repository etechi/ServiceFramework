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

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using SF.Sys.Logging;
using SF.Sys.Hosting;
using SFShop.Data;
using SF.Sys.Services;
using SFShop.ServiceSetup;
using System;

namespace SFShop
{

	public static class SFShopApp 
	{
		public static ILogService LogService()
		{
			var ls = new LogService(new SF.Sys.Logging.MicrosoftExtensions.MSLogMessageFactory());
			ls.AddDebug();
			return ls;
		}
		public static IAppInstanceBuilder Setup(
			EnvironmentType EnvType,
			Microsoft.Extensions.DependencyInjection.IServiceCollection MSServices=null, 
			ILogService logService=null
			)
		{
			if (MSServices == null)
				MSServices = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

			MSServices.AddEFCoreDbContext<SFShopDbContext>(
				(IServiceProvider isp, DbContextOptionsBuilder options) =>
					options.UseSqlServer(isp.Resolve<DbConnection>())
				);

			var svcs = new SF.Sys.Services.ServiceCollection();
			svcs.AddServices(MSServices);

			var ls = logService ?? LogService();

			var builder = new AppInstanceBuilder(
				null,
				EnvType,
				svcs,
				ls
				)
				.With(sc=>sc.AddLogService(ls))
				.With((sc,envType)=>sc.AddSystemServices(EnvType))
				.With((sc, envType) => sc.AddCommonServices(EnvType))
				.With((sc, envType) => sc.AddBizServices(EnvType))
				.With((sc, envType) => sc.AddSFShopServices(EnvType))
				.With((sc, envType) => sc.AddIdentityService())
				.OnEnvType(e => e != EnvironmentType.Utils, (sp)=>
				{
					var ctx = sp.Resolve<SFShopDbContext>();
					Task.Run(async () =>
					{
						await ctx.Database.MigrateAsync();
					}).Wait();
					return null;
				});
			return builder;
		}
	}
}