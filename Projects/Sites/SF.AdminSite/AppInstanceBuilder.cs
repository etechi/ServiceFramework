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
using Microsoft.Extensions.Logging;
using System.Data.Entity.Migrations;
using System.Data.Entity.Infrastructure;
using SF.Management.MenuServices.Models;
using SF.Core.ServiceManagement.Management;
using System.Threading.Tasks;

namespace SF.AdminSite
{
	
	public static class App 
	{
		public static IAppInstanceBuilder Builder()
		{
			var envType = EnvironmentTypeDetector.Detect();
			var builder = Applications.App.Builder(
				envType,
				Applications.App.LogService().AddAspNetTrace()
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