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
	
	public class AppInstanceBuilder : SF.Applications.AppInstanceBuilder
	{
		public AppInstanceBuilder() : this(EnvironmentTypeDetector.Detect())
		{ }
		public AppInstanceBuilder(EnvironmentType EnvType) : base(EnvType)
		{ }
		protected override ILogService OnCreateLogService()
		{
			var ls = base.OnCreateLogService();
			ls.AddAspNetTrace();
			return ls;
		}

		protected override void OnConfigServices(IServiceCollection Services)
		{
			base.OnConfigServices(Services);

			Services.UseAspNetFilePathStructure();
			if (EnvType != EnvironmentType.Utils)
			{
				Services.RegisterMvcControllers(GetType().Assembly);
				Services.RegisterWebApiControllers(GlobalConfiguration.Configuration);
				Services.UseNetworkService();
				Services.UseWebApiNetworkService(GlobalConfiguration.Configuration);
			}
		}
	}
}