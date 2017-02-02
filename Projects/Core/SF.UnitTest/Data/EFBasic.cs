using System;
using Xunit;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
#if NETCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SF.Data.Entity.EntityFrameworkCore;
#else
using System.Data.Entity;

#endif
using SF.Core.DI.MicrosoftExtensions;
using SF.Core.DI;
using SF.Data.Entity;
using System.Linq;

namespace SF.UT.Data
{
	
	public class AppContext : SF.Data.Entity.DbContext
	{
		public AppContext():this(new EFStartup().ConfigureService())
		{

		}
		public AppContext(IServiceProvider sp) : base(sp,"name = default")
		{

		}
	}
	class EFStartup {
		public IServiceProvider ConfigureService()
		{
			var isc = SF.Core.DI.MicrosoftExtensions.DIServiceCollection.Create();


			isc.UseDataModules<DataModels.User, DataModels.Post>();
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
			return isc.BuildServiceProvider();
		}
	}

	public class EFBasicTest
    {
		[Fact]
		public void Test()
		{
			var sp = new EFStartup().ConfigureService();
			var sf = sp.Resolve<IDIScopeFactory>();
			using(var s = sf.CreateScope())
			{
				var isp = s.ServiceProvider;
				var ac = isp.Resolve<IDataSet<DataModels.User>>();
				ac.Add(new DataModels.User { Id = "aa", FirstName = "c", LastName = "y" });
				ac.Context.SaveChanges();

				var re=ac.AsQueryable(true) .ToArrayAsync().Result;
				Assert.Equal(1, re.Length);
				Assert.Equal("c", re[0].FirstName);
			}
			using (var s = sf.CreateScope())
			{
				var isp = s.ServiceProvider;
				var ac = isp.Resolve<AppContext>();
				var re = ac.Set<DataModels.User>().ToArrayAsync().Result;
				Assert.Equal(1, re.Length);
				Assert.Equal("c", re[0].FirstName);
			}
		}
	}
}
