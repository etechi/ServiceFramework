using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SF.AdminSite
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
			config.Routes.MapHttpRoute(
				name: "Media",
				routeTemplate: "r/{id}",
				defaults: new { controller = "media", action = "get", id = RouteParameter.Optional }
			);

			config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "sapi/{controller}/{service}/{action}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);
		}
    }
}
