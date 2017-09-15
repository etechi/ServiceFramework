using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SF.Core.ServiceManagement;
using SF.Applications;
using SF.Core.Hosting;

namespace SF.AdminSiteCore2
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
			services.AddMvc();

			var ins = Core2App.Setup(Core.Hosting.EnvironmentType.Production, services)
				.With(sc=>
					sc.AddAspNetCoreFilePathStructure()
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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
