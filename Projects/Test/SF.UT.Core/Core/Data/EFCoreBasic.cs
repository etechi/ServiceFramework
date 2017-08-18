using System;
using Xunit;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SF.Data.Storage.EntityFrameworkCore;
using SF.Core.DI;
using SF.Data.Storage;
using System.Linq;
using SF.Core.ServiceManagement;
using SF.UT.Data.DataModels;

namespace SF.UT.Data
{
	class EFCoreContext : Microsoft.EntityFrameworkCore.DbContext
	{
		public EFCoreContext(DbContextOptions options):base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Ignore<Location>();
		}
	}

	class EFCoreStartup {
		public IServiceProvider ConfigureService()
		{
			var isc = (SF.Core.ServiceManagement.IServiceCollection) new Core.ServiceManagement.ServiceCollection();
			isc.AddSystemMemoryCache();
			isc.UseMemoryManagedServiceSource();
			isc.AddDataModules<DataModels.User, DataModels.Post>();

			var msc = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

			msc.AddEntityFrameworkSqlServer();
			msc.AddDbContext<EFCoreContext>((asp,op) =>
				{
					op.UseSqlServer(
					//System.Configuration.ConfigurationManager.ConnectionStrings["default"].ConnectionString
					"data source=.\\SQLEXPRESS;initial catalog=sftest;user id=sa;pwd=system;MultipleActiveResultSets=True;App=EntityFramework"
					).LoadDataModels(asp);

					//((IDbContextOptionsBuilderInfrastructure)op).AddOrUpdateExtension(new DataExtension());
					//op.Options.WithExtension();
				},Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient
				);
			msc.UseEFCoreDataEntity<EFCoreContext>();

			isc.AddServices(msc);
			isc.AddDataContext(new SF.Data.DataSourceConfig
			{
				ConnectionString= "data source=.\\SQLEXPRESS;initial catalog=sftest;user id=sa;pwd=system;MultipleActiveResultSets=True;App=EntityFramework"
			});
			return isc.BuildServiceResolver();
		}
	}

	public class EFCoreBasicTest
    {
		[Fact]
		public void Test()
		{
			var sp = new EFCoreStartup().ConfigureService();
			sp.WithScope(isp =>
			{
				var ac = isp.Resolve<IDataSet<DataModels.User>>();
				ac.RemoveRange(ac.AsQueryable(false).ToArray());
				ac.Context.SaveChanges();
			});
			sp.WithScope(isp =>
			{
				var ac = isp.Resolve<IDataSet<DataModels.User>>();
				ac.Add(new DataModels.User
				{
					Id = "aa",
					FirstName = "c",
					LastName = "y",
					Location = new DataModels.Location { City = "city", Address = "addr" }
				});
				ac.Context.SaveChanges();

				var re = ac.AsQueryable(true).ToArrayAsync().Result;
				Assert.Equal(1, re.Length);
				Assert.Equal("c", re[0].FirstName);
				Assert.Equal("addr", re[0].Location.Address);
				Assert.Equal("city", re[0].Location.City);
			});
			sp.WithScope(isp =>
			{
				var ac = isp.Resolve<IDataContext>();
				var re = ac.Set<DataModels.User>().AsQueryable().ToArrayAsync().Result;
				Assert.Equal(1, re.Length);
				Assert.Equal("c", re[0].FirstName);
			});
		}
	}
}
