using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SF.AspNetCore.Mvc;
using SF.DI.Microsoft;
using SF.DI;
using SF.ServiceManagement;
using Microsoft.Extensions.DependencyInjection;
using SF.Annontations;

namespace ManagementConsole
{
	[NetworkService]
	public interface IAdd
	{
		int add(int a, int b);
	}
	public class Add : IAdd
	{
		public int offset { get; }
		public Add(int offset)
		{
			this.offset = offset;
		}
		public int add(int a, int b)
		{
			return a + b+offset;
		}
	}
	public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
			// Add framework services.
			var sc = services.GetDIServiceCollection();
			//sc.AddTransient<IAdd, Add>();
			sc.UseMemoryManagedServiceSource();

			var msc = new ManagedServiceCollection();
			msc.AddScoped<IAdd, Add>();
			sc.UseManagedService(msc);

			sc.AddNetworkServices();

            services.AddMvc();

			var sp=services.BuildServiceProvider();

			var mcs=(MemoryServiceSource)sp.GetService<IServiceConfigLoader>();
			mcs.SetConfig<IAdd, Add>("default", new { offset = 10 });
			mcs.SetDefaultService<IAdd>("default");

			return sp;
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
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
