using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using SF.DI;
using SF.Services.Management;
using SF.Services.Metadata;
using SF.Serialization;
using SF.Data.Entity.EntityFrameworkCore;

namespace DBSetup

{
	public class AppContext : DbContext
	{
		public AppContext(DbContextOptions<AppContext> options)
			: base(options)
		{ }

	}
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			var connection = @"data source=localhost\SQLEXPRESS;initial catalog=efcofetest2;user id=sa;pwd=system;MultipleActiveResultSets=True;App=EntityFramework";
			services.AddDbContext<AppContext>(
				(isp,options) => 
				options.LoadDataModels(isp).UseSqlServer(connection)
				);

			// Add framework services.
			var sc = services.GetDIServiceCollection();
			
			//sc.UseNewtonsoftJson();

			////sc.AddTransient<IAdd, Add>();
			//sc.UseMemoryManagedServiceSource();

			var msc = new ManagedServiceCollection(sc);
			msc.SetupServices();
			msc.UseEFCoreIdentGenerator("App");
			msc.UseEFCoreUser("App");

			//sc.UseServiceMetadata();

			sc.UseEFCoreDataEntity<AppContext>();

			var sp = services.BuildServiceProvider();


			return sp;
		}
	}
	class Program
	{
		
		static void Main(string[] args)
		{

		}
	}
}