using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SF.Core.ServiceManagement;
using SF.Core.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Newtonsoft.Json.Serialization;

namespace Hygou
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
			services
				.AddMvc()
				.AddJsonOptions(
					s => SF.Core.Serialization.Newtonsoft.JsonSerializer.ApplySetting(
						s.SerializerSettings,
						new SF.Core.Serialization.JsonSetting
						{
							IgnoreDefaultValue = true,
							WithType = false
						})
				);

			var ins = HygouApp.Setup(SF.Core.Hosting.EnvironmentType.Production, services)
							.With(sc =>
								sc.AddAspNetCore()
								)
							.OnEnvType(
								t=>t!=EnvironmentType.Utils,
								sc=>
								{
									sc.AddNetworkService();
									sc.AddAspNetCoreServiceInterface();
								}
								)
							.Build();
			return ins.ServiceProvider;
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
			});
			var mvc=app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "Media",
					template: "r/{id?}",
					defaults: new { scope = "api" , controller = "media", action = "get" }
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
				routes.MapRoute(
					name: "admin",
					template: "Admin/{action}/{*id}",
					defaults:new { controller = "Admin" }
					);

				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
			
		}
    }
}
