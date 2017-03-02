using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SF.Data.Storage.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SF.AdminSiteCore
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
			var connection = @"Host=localhost;Database=sfadmin;Username=postgres;Password=system";
			services.AddDbContext<AppContext>(
				(isp, options) =>
				options.LoadDataModels(isp).UseNpgsql(connection)
				);

			services.AddMvc().AddJsonOptions(opt =>
			{
				opt.SerializerSettings.ContractResolver = SF.Core.Serialization.Newtonsoft.FixedContractResolver.Instance;
				opt.SerializerSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.IgnoreAndPopulate;
			});
			var app = AppInstanceBuilder.Build(
				EnvironmentTypeDetector.Detect(services),
				null, 
				new SF.Core.DI.MicrosoftExtensions.DIServiceCollection(services)
				);
			services.AddSingleton(app);
			return app.ServiceProvider;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

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
					   name: "media",
					   defaults: new
					   {
						   controller = "Media",
						   action = "Get",
						   scope="api"
					   },
					   template: "r/{id}");
				routes.MapRoute(
					   name: "api",
					   defaults: new
					   {
						   scope = "api"
					   },
					   template: "api/{controller}/{action}"
					   );
			
				routes.MapRoute(
					name: "admin",
					defaults: new
					{
						controller = "Home",
						action = "Index"
					},
					template: "{*id}");

				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
        }
    }
}
