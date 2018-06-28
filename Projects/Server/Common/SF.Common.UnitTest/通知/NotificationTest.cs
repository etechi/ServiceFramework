
using SF.Sys.Hosting;
using System;
using System.Threading.Tasks;
using SF.Sys.Services;
using SF.Sys.UnitTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SF.IdentityService.UnitTest;
using SF.Common.Notifications.Management;
using SF.Common.Notifications.Senders;
using SF.Sys.TaskServices;
using SF.Sys;
using SF.Common.Notifications.Front;
using System.Linq;
using SF.Sys.Reminders;
using SF.Sys.Entities;
using SF.Sys.Data;
using SF.Common.UnitTest;
using SF.UT;

namespace SF.Common.UT.通知
{
	[TestClass]
	public class 通知基本测试 : TestBase
	{
		public 通知基本测试() : base(UnitTest.TestAppBuilder.Instance) { }
		public 通知基本测试(IAppInstanceBuilder builder) : base(builder) { }

		[TestMethod]
		public async Task 即时消息()
		{
			await (from sp in NewServiceScope().InitServices()
				   from user in sp.UserCreate()
				   select (sp,user)
				   )
				.Use(async (ctx)=>
				{
					var (sp, env) = ctx;
					var rm = sp.Resolve<IReminderManager>();
					await rm.ClearAllReminders();

					var nm =sp.Resolve<INotificationManager>();
					var tte = sp.Resolve<ITimedTaskService>();
					var timerService = sp.Resolve<ITimerService>();
					var dp = sp.Resolve<IDebugNotificationSendProvider>();

					var task = timerService.WaitFor(()=>Task.FromResult(dp.LastArgument!=null),10);
					var re = await nm.CreateNotification(
						SF.Common.Notifications.NotificationMode.Normal,
						new[] { env.User.Id },
						"测试",
						new System.Collections.Generic.Dictionary<string, object>
						{
						{"Name","Name"},
						{"Title","Title" },
						{"Content","Content" },
						{"Arg1" ,"Arg1"},
						{"Arg2" ,"Arg2"}
						}
						);
					await task;
					var arg = dp.LastArgument;
					Assert.IsNotNull(arg);
					var oarg=arg.GetArguments();
					Assert.AreEqual("TitleTemplate:Title", arg.Title);
					Assert.AreEqual("ContentTemplate:Content", arg.Content);
					Assert.AreEqual("{\"Arg1\":\"Arg1:Arg1\",\"Arg2\":\"Arg2:Arg2\"}", Json.Stringify(arg.GetArguments()));
					var ns = sp.Resolve<INotificationService>();
					var res =await ns.Query(new SF.Common.Notifications.Front.NotificationQueryArgument { Mode=SF.Common.Notifications.NotificationMode.Normal });
					var n= res.Items.Single();
					Assert.AreEqual("Name:Name", n.Name);
					Assert.IsTrue(n.HasBody);
					var res1 = await ns.Query(new SF.Common.Notifications.Front.NotificationQueryArgument { });
					Assert.IsTrue(res1.Items.Any());
					//Assert.AreEqual("Content:Content", n.Content);
				}
			);
		}
	}


}
