﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SF.AdminSite
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.MapRoute(
				name: "SysAdmin",
				url: "sysadmin/{*id}",
				defaults: new { controller = "SysAdmin", action = "Index", id = UrlParameter.Optional }
			);
			routes.MapRoute(
				name: "Default",
				url: "{*id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
