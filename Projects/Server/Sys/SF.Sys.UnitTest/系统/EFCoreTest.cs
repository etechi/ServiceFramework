using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using SF.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using SF.Sys.ADT;
using SF.Sys.Threading;
using System.Threading.Tasks;
using SF.Sys.UnitTest;
using SF.Sys.Data;
using SF.Sys.Services;

namespace SF.UT.系统
{
	[TestClass]
	public class EFCoreTest : TestBase
	{
		class Item
		{
			public long Id { get; set; }
			public string Name { get; set; }
			public IEnumerable<Item> Items { get; set; }
		}
		[TestMethod]
		public async Task ContextCacheTest()
		{
			//await ScopedTestContext().Run(async sp =>
			//{
			//	var ctx = sp.Resolve<IDataContext>();
			//	await ctx.UseTransaction("test",async t=>
			//	 {
			//		 await ctx.Set<DataPharmacon>().AsQueryable().ToArrayAsync();
			//		 await ctx.Set<DataPharmacon>().AsQueryable().ToArrayAsync();
			//		 return 0;
			//	 });

			//});
			//await ScopedTestContext().Run(async sp =>
			//{
			//	var ctx = sp.Resolve<IDataContext>();
			//	await ctx.UseTransaction("test", async t =>
			//	{
			//		await ctx.Set<DataPharmacon>().AsQueryable().ToArrayAsync();
			//		await ctx.Set<DataPharmacon>().AsQueryable().ToArrayAsync();
			//		return 0;
			//	});

			//});

		}
		[TestMethod]
		public async Task InnerQueryTest()
		{
		
			//await NewServiceScope()
			//	.NewDataContext()
			//	.Use(async (ctx) =>
			//{
			//	var q = (from p in ctx.Set<DataPharmacon>().AsQueryable()
			//			 //let Items = from pp in p.Products
			//				//		 select new Item
			//				//		 {
			//				//			 Id = pp.Id,
			//				//			 Name = pp.Name
			//				//		 }
			//			 select p)
			//			 .Include(p => p.Products)
			//			 .Select(p=>new Item
			//			 {
			//				 Id=p.Id,
			//				 Name=p.Name,
			//				 Items=p.Products.Select(pp=>new Item
			//				 {
			//					 Id=pp.Id,
			//					 Name=pp.Name
			//				 })
			//			 })
			//			 ;

			//	var sql = q.GetUnderlingCommandTexts();
			//	var re = await q.ToArrayAsync();


			//});
			
		}
		
	}

}
