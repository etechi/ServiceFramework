using System;
using System.Collections.Generic;
using System.Linq;
using SF.Core.ServiceManagement;
using SF.Metadata;
using SF.Services.Test;
using SF.Core.TaskServices;
using SF.Core.Hosting;
using SF.Core.Logging;
using Microsoft.Extensions.Logging;
using System.Data.Entity.Migrations;
using System.Data.Entity.Infrastructure;
using SF.Management.MenuServices.Models;
using SF.Core.ServiceManagement.Management;
using System.Threading.Tasks;
using SF.Application.Migrations;

namespace SF.Applications
{
	
	public static class Net46App 
	{
		public static ILogService LogService()
		{
			var ls = new LogService(new Core.Logging.MicrosoftExtensions.MSLogMessageFactory());
			ls.AddDebug();
			return ls;
		}
		public static IAppInstanceBuilder Setup(EnvironmentType EnvType,ILogService logService=null)
		{
			var ls = logService ?? LogService();
			var builder = new SF.Core.Hosting.AppInstanceBuilder(
				null,
				EnvType,
				new SF.Core.ServiceManagement.ServiceCollection(),
				ls
				)
				.With(sc=>sc.AddLogService(ls))
				.With((sc,envType)=>AppCore.AddAppCoreServices(sc,envType))
				.With((sc,envType)=> ConfigServices(sc,envType))
				.OnEnvType(e => e != EnvironmentType.Utils, sp =>
				{
					var configuration = new Configuration();
					var migrator = new DbMigrator(configuration);
					migrator.Update();
					return null;
				});
			return builder;
		}


		static void ConfigServices(IServiceCollection Services,EnvironmentType EnvType)
		{
			Services.AddSystemMemoryCache();
			Services.AddSystemDrawing();
			Services.AddEF6DataEntity();
			//Services.AddTransient(tsp => new AppContext(tsp));
			//Services.AddEF6DataEntity<AppContext>();
			Services.AddDataContext(new SF.Data.DataSourceConfig
			{
				ConnectionString = "data source=.\\SQLEXPRESS;initial catalog=sfadmin;user id=sa;pwd=system;MultipleActiveResultSets=True;App=EntityFramework"
			});
		}
	
	}
}