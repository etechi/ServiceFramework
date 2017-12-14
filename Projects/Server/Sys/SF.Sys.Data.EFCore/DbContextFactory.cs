


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SF.Sys.Data;
using SF.Sys.Hosting;
using SF.Sys.Services;
using System.Diagnostics;
using System.IO;

namespace SF.Sys.Data.EntityFrameworkCore
{
	public abstract class DbContextFactory : DbContextFactory<DbContext>
	{
		public DbContextFactory(IAppInstance AppInstance) : base(AppInstance)
		{
		}
	}

	public abstract class DbContextFactory<TDbContext> : IDesignTimeDbContextFactory<TDbContext>
		where TDbContext:DbContext
	{
		IAppInstance AppInstance { get; }
		public DbContextFactory(IAppInstance AppInstance)
		{
			this.AppInstance = AppInstance;
		}
		public TDbContext CreateDbContext(string[] args)
		{
			return AppInstance.ServiceProvider.Resolve<TDbContext>();
		}
	}


}
