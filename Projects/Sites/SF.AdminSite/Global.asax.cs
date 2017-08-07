using SF.AspNet.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SF.Core.DI;
using SF.Core.ServiceManagement;

[assembly: PreApplicationStartMethod(typeof(SF.AdminSite.MvcApplication), nameof(SF.AdminSite.MvcApplication.RegisterDIHttpModule))]

namespace SF.AdminSite
{
    public class MvcApplication : SF.AspNet.HttpApplication
	{
		public static void RegisterDIHttpModule()
		{
			DIHttpModule.Register();
		}

		protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);

			StartAppInstance(
				App.Builder().Build(), 
				GlobalConfiguration.Configuration
				);


			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
		protected void Application_Stop()
		{
			StopAppInstance();
		}
    }
}
