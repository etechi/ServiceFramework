using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using SF.Core.DI;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SF.AdminSiteCore
{
    public class Program
    {
		public static string ConnectionString { get; set; }
        public static void Main(string[] args)
        {
			var builder = new ConfigurationBuilder();
			builder.AddCommandLine(args);
			var config = builder.Build();

			//if (args.Length == 1)
			//ConnectionString = args[0];
			var host = new WebHostBuilder()
				.UseConfiguration(config)
				.UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();
			if (args.Length == 1 && args[0] == "db-update")
			{
				host.Services.Resolve<AppContext>().Database.Migrate();
				return;
			}

				
            host.Run();
        }
    }
}
