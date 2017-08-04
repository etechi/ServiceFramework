using System;
using Xunit;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
#if NETCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SF.Data.Storage.EntityFrameworkCore;
#else
using System.Data.Entity;

#endif

using SF.Core.DI;
using SF.Data.Storage;
using System.Linq;
using SF.Core.ServiceManagement;

namespace SF.UT.Data
{
#if !NETCORE
	public class AppContext : SF.Data.Storage.DbContext
	{
		public AppContext():this(new EFStartup().ConfigureService())
		{

		}
		public AppContext(IServiceProvider sp) : base(sp,"name = default")
		{

		}
	}
#endif
	class EFStartup {
		public IServiceProvider ConfigureService()
		{
			var isc = new ServiceCollection();
			isc.UseSystemMemoryCache();
			isc.UseMemoryManagedServiceSource();
			isc.AddDataModules<DataModels.User, DataModels.Post>();
#if NETCORE
			sc.AddEntityFrameworkInMemoryDatabase();
			sc.AddDbContext<DbContext>((asp,op) =>
				{
					op.UseInMemoryDatabase().LoadDataModels(asp);

					//((IDbContextOptionsBuilderInfrastructure)op).AddOrUpdateExtension(new DataExtension());
					//op.Options.WithExtension();
				},Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient
				);
			isc.UseEFCoreDataEntity<DbContext>();
#else
			isc.AddTransient<AppContext>(tsp => new AppContext(tsp));
			isc.UseEF6DataEntity<AppContext>();
#endif
			isc.UseDataContext();
			return isc.BuildServiceResolver();
		}
	}

	public class EFBasicTest
    {
		[Fact]
		public void Test()
		{
			var sp = new EFStartup().ConfigureService();
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
