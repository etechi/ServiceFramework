using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SF.IdentityService.UnitTest;
using SF.Sys.UnitTest;
using SF.Sys;
using SF.Common.UnitTest;
using SF.Services;
using SF.UT;
using SF.Sys.Hosting;

namespace SF.Common.UT.通知
{

	[TestClass]
	public class 全体通知 : TestBase
	{
		public 全体通知() : base(UnitTest.TestAppBuilder.Instance) { }
		public 全体通知(IAppInstanceBuilder builder) : base(builder) { }

		[TestMethod]
		public async Task 新建()
        {
			await (from sp in NewServiceScope().InitServices()
				   from user in sp.UserCreate()
				   select (sp, user)
				   )
				.Use(async (ctx) =>
				{
					var (sp, env) = ctx;
					await sp.EnsureStatusChanged(
						1, 1,
					async () =>
					{
						var re = await sp.CreateBroadcastNotification();
						await sp.EnsureNotification(re);
					}
					);
				});
        }
		[TestMethod]
		public async Task 新建X2()
        {
			await (from sp in NewServiceScope().InitServices()
				   from user in sp.UserCreate()
				   select (sp, user)
				   )
				.Use(async (ctx) =>
				{
					var (sp, env) = ctx;
					await sp.EnsureStatusChanged(
					 2, 2,
					async () =>
					{
						var re = await sp.CreateBroadcastNotification();
						await sp.EnsureNotification(re);
						re = await sp.CreateBroadcastNotification();
						await sp.EnsureNotification(re);
					}
					);
				 });
        }

		[TestMethod]
		public async Task 新建_阅读()
        {
			await (from sp in NewServiceScope().InitServices()
				   from user in sp.UserCreate()
				   select (sp, user)
				   )
				.Use(async (ctx) =>
				{
					var (sp, env) = ctx;
					await sp.EnsureStatusChanged(
					  1, 0,
					async () =>
					{
						var re = await sp.CreateBroadcastNotification();
						await sp.EnsureNotification(re);
						await sp.ReadNotification( new[] { re.Id });
					}
					);
				   });
        }
		[TestMethod]
		public async Task 新建_删除()
        {
			await (from sp in NewServiceScope().InitServices()
				   from user in sp.UserCreate()
				   select (sp, user)
				   )
				.Use(async (ctx) =>
				{
					var (sp, env) = ctx;
					await sp.EnsureStatusChanged(
					 0, 0,
					async () =>
					{
						var re = await sp.CreateBroadcastNotification();
						await sp.EnsureNotification(re);
						await sp.DeleteNotification(re.Id);
					}
				);
				});
        }
		[TestMethod]
		public async Task 新建_阅读_删除()
        {
			await (from sp in NewServiceScope().InitServices()
				   from user in sp.UserCreate()
				   select (sp, user)
				   )
				.Use(async (ctx) =>
				{
					var (sp, env) = ctx;
					await sp.EnsureStatusChanged(
						0, 0,
						async () =>
						{
							var re = await sp.CreateBroadcastNotification();
							await sp.EnsureNotification(re);
							await sp.ReadNotification(new[] { re.Id });
							await sp.DeleteNotification(re.Id);
						}
						);
				});
        }
    }
}
