


using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SF.Sys.Data;
using SF.Sys.Hosting;
using SF.Sys.Services;
using System.Diagnostics;
using System.IO;

namespace SF.Sys.Services
{
	public static class DefaultDataSourceConfig
	{
		public static IServiceCollection AddAppSettingDefaultDataSourceConfig(this IServiceCollection sc)
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build();
			sc.AddSingleton(sp =>
				new DataSourceConfig
				{
					ConnectionString = configuration["ConnectionStrings:Default"]
				}
			);
			return sc;
		}
	}


}
