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
using Microsoft.Extensions.DependencyInjection;
using SF.Core.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.AspNetCore.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;

namespace SF.AspNetCore
{
	public class ApplicationConfigure
	{
		public string ExceptionHandler { get; set; } = "/Home/Error";
	}
	public static class ApplicationBuilderExtension
	{
		public static IApplicationBuilder ApplicationCommonConfigure(
			this IApplicationBuilder app, 
			IHostingEnvironment env,
			ApplicationConfigure cfg=null
			)
		{
			if (cfg == null)
				cfg = new ApplicationConfigure();
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

			var mvc = app.UseMvc(routes =>
			{

				routes.MapRoute(
					name: "Media",
					template: "r/{id?}",
					defaults: new { scope = "api", controller = "media", action = "get" }
				);
				routes.MapRoute(
					name: "ServiceApi",
					template: "api/{controller}-{service}/{action}/{id?}",
					defaults: new { scope = "api" }
				);

				routes.MapRoute(
					name: "DefaultApi",
					template: "api/{controller}/{action}/{id?}",
					defaults: new { scope = "api" }
					);

			});

			return app;
		}
	}
}
