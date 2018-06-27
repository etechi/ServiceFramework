using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using SF.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using SF.Sys.ADT;
using SF.Sys.Threading;
using System.Threading.Tasks;

namespace SF.UT.系统
{
	[TestClass]
	public class SyncQueueTest
	{

		[TestMethod]
		public async Task SyncQueueSum()
		{

			var sq = new ObjectSyncQueue<int>();
			var rand = new Random();
			int Next(int max)
			{
				lock (rand)
					return rand.Next(max);
			}
			int value = 0;
			await Task.WhenAll(
				Enumerable.Range(0, 100)
				.Select(i => Task.Run(async () =>
				{
					for (var j = 0; j < 10; j++)
					{
						await sq.Queue(10, async () =>
						{
							var v = value;
							await Task.Delay(Next(3));
							value = v + 1;
						});
						await Task.Delay(Next(10));
					}
				})).ToArray());
			Assert.AreEqual(1000, value);
		}

		[TestMethod]
		public async Task SyncScopeSum()
		{

			var sq = new SyncScope();
			var rand = new Random();
			int Next(int max)
			{
				lock (rand)
					return rand.Next(max);
			}
			int value = 0;
			await Task.WhenAll(
				Enumerable.Range(0, 100)
				.Select(i => Task.Run(async () =>
				{
					for (var j = 0; j < 10; j++)
					{
						await sq.Sync(async () =>
						{
							var v = value;
							await Task.Delay(Next(3));
							value = v + 1;
						});
						await Task.Delay(Next(10));
					}
				})).ToArray());
			Assert.AreEqual(1000, value);
		}

	}

}
