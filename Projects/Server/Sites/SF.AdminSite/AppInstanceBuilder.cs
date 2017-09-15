using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SF.Core.ServiceManagement;
using SF.AspNet.DI;
using System.Web.Http;
using SF.AdminSite;
using SF.Metadata;
using SF.Services.Test;
using SF.Core.TaskServices;
using SF.Core.Hosting;
using SF.Core.Logging;
using System.Data.Entity.Migrations;
using System.Data.Entity.Infrastructure;
using SF.Management.MenuServices.Models;
using SF.Core.ServiceManagement.Management;
using System.Threading.Tasks;
using SF.Applications;

namespace SF.AdminSite
{
	
	public static class App 
	{
		public static IAppInstanceBuilder Builder()
		{
			var envType = EnvironmentTypeDetector.Detect();
			var builder = Net46App.Setup(
				envType,
				Net46App.LogService().AddAspNetTrace()
				)
				.With(sc => sc.UseAspNetFilePathStructure())
				.OnEnvType(
					et => et != EnvironmentType.Utils, 
					sc =>
					{
						 sc.RegisterMvcControllers(typeof(App).Assembly);
						 sc.RegisterWebApiControllers(GlobalConfiguration.Configuration);
						 sc.UseNetworkService();
						 sc.UseWebApiNetworkService(GlobalConfiguration.Configuration);
					}
				);
			return builder;
		}

	}
}