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

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;
using System;
using SF.Sys.Logging;
using SF.Sys.Auth;
using System.Text;
using SF.Sys.Hosting;
using System.Threading.Tasks;
using SF.Sys.ServiceFeatures;
using SF.Sys.Services;
using Microsoft.AspNetCore.Routing;

namespace SF.Sys.AspNetCore
{
	public class ApplicationConfigure
	{
		public string ExceptionHandler { get; set; } = "/Home/Error";
		public Action<IRouteBuilder> RouteConfig { get; set; }
	}
	public static class ApplicationBuilderExtension
	{
		public static void StartServices(this IApplicationBuilder app)
		{
			
			var ins = app.ApplicationServices.Resolve<IAppInstance>();
			if (ins.EnvType != EnvironmentType.Utils)
			{
				
				var disposable = Task.Run(() =>
					  app.ApplicationServices.BootServices()
					).Result;

				var applicationLifetime = app.ApplicationServices.Resolve<IApplicationLifetime>();
				applicationLifetime.ApplicationStopping.Register(() =>
				{
					disposable.Dispose();
				});
			}
		}
		public static IApplicationBuilder ApplicationCommonConfigure(
		   this IApplicationBuilder app,
		   //IHostingEnvironment env,
		   Action<IRouteBuilder> RouteConfig
		   )
			=> app.ApplicationCommonConfigure(new ApplicationConfigure
			{
				RouteConfig=RouteConfig
			});
		public static IApplicationBuilder ApplicationCommonConfigure(
			this IApplicationBuilder app, 
			//IHostingEnvironment env,
			ApplicationConfigure cfg=null
			)
		{
			if (cfg == null)
				cfg = new ApplicationConfigure();
			var env = app.ApplicationServices.Resolve<IHostingEnvironment>();
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			}
			else
			{
				app.UseExceptionHandler(cfg.ExceptionHandler);
				
			}

			app.UseStaticFiles();
			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
			});
			app.UseAuthentication();
			//app.UseCookiePolicy(cookiePolicyOptions);

			app.UseRequestLogging();

			return app.UseMvc(routes =>
			{

				routes.MapRoute(
					name: "Media",
					template: "r/{id?}",
					defaults: new { mvc_scope = "api", controller = "media", action = "get" }
				);
				routes.MapRoute(
					name: "ServiceApi",
					template: "api/{controller}/{service}/{action}/{id?}",
					defaults: new { mvc_scope = "api" }
				);

				routes.MapRoute(
					name: "DefaultApi",
					template: "api/{controller}/{action}/{id?}",
					defaults: new { mvc_scope = "api" }
					);
				cfg?.RouteConfig?.Invoke(routes);
			});
		}
	}
}
