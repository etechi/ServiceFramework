using System;
using Xunit;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using SF.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SF.DI;
using SF.Data.Entity.EntityFrameworkCore;
using SF.Data.Entity;
using System.Linq;

namespace SF.UT.Data
{
	
	
	public class EFBasicTest
    {
		[Fact]
		public void Test()
		{
			var sc = new ServiceCollection();
			
			//sc.AddEntityFrameworkInMemoryDatabase();
			sc.AddDbContext<DbContext>((asp,op) =>
				{
					op.UseInMemoryDatabase().LoadDataModels(asp);
						
					//((IDbContextOptionsBuilderInfrastructure)op).AddOrUpdateExtension(new DataExtension());
					//op.Options.WithExtension();
				},Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient
				);
			


			var isc = sc.GetDIServiceCollection();
			isc.UseDataModules<DataModels.User, DataModels.Post>();
			isc.UseEFCoreDataEntity<DbContext>();

			var sp = sc.BuildServiceProvider();
			var sf = sp.GetRequiredService<IServiceScopeFactory>();
			using(var s = sf.CreateScope())
			{
				var isp = s.ServiceProvider;
				var ac = isp.GetRequiredService<IDataSet<DataModels.User>>();
				ac.Add(new DataModels.User { Id = "aa", FirstName = "c", LastName = "y" });
				ac.Context.SaveChanges();

				var re=ac.AsQueryable(true) .ToArrayAsync().Result;
				Assert.Equal(1, re.Length);
				Assert.Equal("c", re[0].FirstName);
			}
			using (var s = sf.CreateScope())
			{
				var isp = s.ServiceProvider;
				var ac = isp.GetRequiredService<DbContext>();
				var re = ac.Set<DataModels.User>().ToArrayAsync().Result;
				Assert.Equal(1, re.Length);
				Assert.Equal("c", re[0].FirstName);
			}
		}
	}
}
