using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using SF.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using SF.Sys.ADT;
using System.Threading.Tasks;
using SF.Sys.UnitTest;
using SF.Sys.TaskServices;
using SF.Sys.Services;
using SF.Sys.Reminders;
using SF.Sys.Data;
using SF.Sys.TimeServices;

namespace SF.UT.系统
{
	[TestClass]
	public class TimerTest : TestBase
	{
		class timer : SF.Sys.TaskServices.Timer
		{
			public int index { get; }
			public timer(int index)
			{
				this.index = index;
			}
			public override void OnTimer()
			{
				Console.WriteLine($"timer {index} begin " + DateTime.Now);
				System.Threading.Thread.Sleep(3000);
				Console.WriteLine($"timer {index} end " + DateTime.Now);
			}
			public override void OnCancelled()
			{
				base.OnCancelled();
			}
		}
		class TestRemindable : IRemindable
		{
			public async Task Remind(IRemindContext Context)
			{
				var index = Context.Data;
				Console.WriteLine($"timer {index} begin " + DateTime.Now);
				await Task.Delay(3000);
				Console.WriteLine($"timer {index} end " + DateTime.Now);
			}
		}
		[TestMethod]
		public async Task ReminderBasicTest()
		{
			AppInstanceBuilder.Services.AddRemindable<TestRemindable>();
			await base.NewServiceScope().Use(async sp =>
			{
				var ig = sp.Resolve<IIdentGenerator>();
				var rs = sp.Resolve<IRemindService>();
				var ts = sp.Resolve<ITimeService>();

				await rs.Setup(new RemindSetupArgument
				{
                    BizSource=new Sys.Entities.TrackIdent(nameof(ReminderBasicTest),"test", await ig.GenerateAsync("test")),
					Name = "test",
					RemindableName = typeof(TestRemindable).FullName,
					RemindData = "1",
					RemindTime = ts.Now.AddSeconds(1),
				});
				await rs.Setup(new RemindSetupArgument
				{
                    BizSource = new Sys.Entities.TrackIdent(nameof(ReminderBasicTest), "test", await ig.GenerateAsync("test")),
                    Name = "test",
					RemindableName = typeof(TestRemindable).FullName,
					RemindData = "2",
					RemindTime = ts.Now.AddSeconds(3),
				});
				await rs.Setup(new RemindSetupArgument
				{
                    BizSource = new Sys.Entities.TrackIdent(nameof(ReminderBasicTest), "test", await ig.GenerateAsync("test")),
                    Name = "test",
					RemindableName = typeof(TestRemindable).FullName,
					RemindData = "3",
					RemindTime = ts.Now.AddSeconds(6),
				});
				await Task.Delay(20000);
			});
		}
		[TestMethod]
		public async Task TimerServiceBasicTest()
		{

			await base.NewServiceScope().Use(async sp =>
			{
				var ts = sp.Resolve<ITimerService>();

				ts.Add(0, new timer(1));
				ts.Add(1, new timer(2));
				ts.Add(0, new timer(3));
				ts.Add(1, new timer(4));
				await Task.Delay(10000);
			});
		}
		
		class item : TimerQueue.Timer
		{
			public int Target;
		}
		[TestMethod]
		public void TimerAll()
		{
			
			Action action = () => { };
			var timer = new TimerQueue(5,8);
			var rand = new Random(1);

			for(var i=0;i< timer.MaxPeriod*10; i++)
			{
				var c = rand.Next(100);
				for (var x = 0; x < c; x++)
				{
					var period = rand.Next(timer.MaxPeriod);
					timer.Add(period,new item { Target = i + period });
				}
				var vals = timer.NextTick();
				foreach(var v in vals)
				{
					Assert.AreEqual(i, ((item)v).Target);
				}
			}

		}
		
	}

}
