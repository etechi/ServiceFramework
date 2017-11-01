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
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using SF.Data.EntityFrameworkCore;

namespace Hygou
{
	
	public static class HygouApp 
	{
		public static ILogService LogService()
		{
			var ls = new LogService(new SF.Core.Logging.MicrosoftExtensions.MSLogMessageFactory());
			ls.AddDebug();
			return ls;
		}
		public static IAppInstanceBuilder Setup(EnvironmentType EnvType,Microsoft.Extensions.DependencyInjection.IServiceCollection Services=null, ILogService logService=null)
		{
			if (Services == null)
				Services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

			Services.AddDbContext<HygouDbContext>(
				(isp, options) =>
					options.LoadDataModels(isp).UseSqlServer(isp.Resolve<DbConnection>()),
				ServiceLifetime.Transient
				);

			var svcs = new SF.Core.ServiceManagement.ServiceCollection();
			svcs.AddServices(Services);

			var ls = logService ?? LogService();

			var builder = new SF.Core.Hosting.AppInstanceBuilder(
				null,
				EnvType,
				svcs,
				ls
				)
				.With(sc=>sc.AddLogService(ls))
				.With((sc,envType)=>sc.AddSystemServices(EnvType))
				.With((sc, envType) => sc.AddCommonServices(EnvType))
				.With((sc, envType) => sc.AddBizServices(EnvType))
				.With((sc, envType) => sc.AddPromotionServices(EnvType))
				.With((sc, envType) => sc.AddIdentityService())
				.With((sc,envType)=> ConfigServices(sc,envType))
				.OnEnvType(e => e != EnvironmentType.Utils, (sp)=>
				{
					var ctx = sp.Resolve<HygouDbContext>();
					Task.Run(() => ctx.Database.MigrateAsync()).Wait();
					return null;
				});
			return builder;
		}


		static void ConfigServices(SF.Core.ServiceManagement.IServiceCollection Services,EnvironmentType EnvType)
		{
			Services.AddSystemDrawing();
			Services.AddMediaService(
				EnvType,
				true,
				new Dictionary<string, string>
				{
					{"s0","root://StaticResources" },
					{"p0","root://StaticResources/产品数据/产品图片" }
				});
			Services.AddMicrosoftMemoryCacheAsLocalCache();
			Services.AddHygouServices(EnvType);
			Services.AddEFCoreDataEntity((sp, conn) => sp.Resolve<HygouDbContext>());
			Services.AddDataContext(new SF.Data.DataSourceConfig
			{
				ConnectionString = "data source=.\\SQLEXPRESS;initial catalog=sf-demo-hygou;user id=sa;pwd=system;MultipleActiveResultSets=True;App=EntityFramework"
			});


			//Services.AddSystemDrawing();
			//Services.AddEF6DataEntity();
			//Services.AddTransient(tsp => new AppContext(tsp));
			//Services.AddEF6DataEntity<AppContext>();

		}
	
	}
}