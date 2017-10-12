#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;
using Xunit;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
#if NETCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SF.Data.EntityFrameworkCore;
#else
using System.Data.Entity;

#endif

using SF.Data;
using System.Linq;
using SF.Core.ServiceManagement;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SF.UT.Data
{
#if !NETCORE
	using System;
namespace SF.UT.Data
{
	public class AppContext : SF.Data.DbContext
	{
		public AppContext() : this(new EFStartup().ConfigureService())
		{

		}
		public AppContext(IServiceProvider sp) : base(sp, "name = default")
		{

		}
	}
}

#endif
	class EFStartup {
		public IServiceProvider ConfigureService()
		{
			var isc = new SF.Core.ServiceManagement.ServiceCollection();
			isc.AddMicrosoftMemoryCacheAsLocalCache();
			isc.UseMemoryManagedServiceSource();
			isc.AddDataModules<DataModels.User, DataModels.Post>();
#if NETCORE
			isc.AddEFCoreDataEntity<DbContext>();
			//Services.AddTransient(tsp => new AppContext(tsp));
			//Services.AddEF6DataEntity<AppContext>();
			
#else
			isc.AddTransient<AppContext>(tsp => new AppContext(tsp));
			isc.AddEF6DataEntity<AppContext>();
#endif
			isc.AddDataContext(new SF.Data.DataSourceConfig
			{
				ConnectionString = "data source=.\\SQLEXPRESS;initial catalog=sftest;user id=sa;pwd=system;MultipleActiveResultSets=True;App=EntityFramework"
			});
			return isc.BuildServiceResolver();
		}
	}

	public class EFBasicTest
    {
		//class TestDBContext:System.Data.Entity.DbContext
		//{
		//	public TestDBContext(DbConnection conn) : base(conn, contextOwnsConnection: false)
		//	{

		//	}
		//	public DbSet<SF.Data.IdentGenerator.DataModels.IdentSeed> IdentSeeds { get; set; }
		//}

		//[Fact]
		//public async Task DBContextTransactionTest()
		//{
		//	using (var conn = new SqlConnection("data source=.\\SQLEXPRESS;initial catalog=sfadmin;user id=sa;pwd=system;MultipleActiveResultSets=True;"))
		//	{
		//		using (var context = new TestDBContext(conn))
		//		{
		//			await context.IdentSeeds.ToArrayAsync();

		//			await conn.OpenAsync();
		//			using (var tran = conn.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
		//			{
						
		//				context.Database.UseTransaction(tran);
		//				await context.IdentSeeds.ToArrayAsync();

		//				context.Database.UseTransaction(null);

		//				context.Database.UseTransaction(tran);
		//				var a = await context.IdentSeeds.FirstAsync();
		//				a.NextValue++;
		//				context.Entry(a).State = EntityState.Modified;
		//				await context.SaveChangesAsync();
		//				context.Database.UseTransaction(null);

		//				tran.Commit();
						
		//			}
		//		}
		//	}
		//}
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
