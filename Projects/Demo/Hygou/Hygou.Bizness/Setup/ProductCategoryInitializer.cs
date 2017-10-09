using SF.Biz.Products;
using SF.Core.ServiceManagement.Management;
using SF.Entities;
using SF.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hygou.Setup
{
	public class ProductCategoryInitializer
	{
		//public Bizness.Products.Models.ProductCategoryEditable C100 { get; private set; }
		//public CategoryEditable C10 { get; private set; }
		//public Bizness.Products.Models.ProductCategoryEditable Limit { get; private set; }
		//public Bizness.Products.Models.ProductCategoryEditable NewUser { get; private set; }

		public CategoryInternal SpecialCategoryRoot { get; private set; }
		public CategoryInternal TypedCategoryRoot { get; private set; }
		public CategoryInternal[] TypedCategorys { get; private set; }
		public long MainProductCategoryId { get; private set; }
		public static async Task<ProductCategoryInitializer> Create(
			IServiceInstanceManager ServiceInstanceManager,
			ICategoryManager CategoryManager,
			IItemManager ItemManager,
			long sellerId,
			long? ScopeId,
			ProductTypeInitializer types
			)
		{
			var special = await CategoryManager.EnsureEntity(
					await CategoryManager.QuerySingleEntityIdent(new CategoryQueryArgument { Name= "特别分类" }),
					(CategoryInternal c) =>
					{
						c.Name = "特别分类";
						c.Title = "特别分类";
						c.ObjectState = EntityLogicState.Enabled;
						c.Children = CategoryManager.FillTreeIdentByName(
							ObjectKey.From(c.Id),
							new[]
						{
							new CategoryInternal
							{
								Name="十元",
								Title="十元",
								Image = StaticRes.File+"-pc-cols-1-jpg",
								Tag = "10",
								ObjectState=EntityLogicState.Enabled,
							},
							new CategoryInternal
							{
								Name="上线充值活动",
								Title="上线充值活动",
								Tag = "recharge-activity",
								ObjectState=EntityLogicState.Enabled,
							},
							//new Bizness.Products.Models.ProductCategoryEditable
							//{
							//	Name="百元",
							//	Title="百元",
							//	Image = "yg-cols-100-jpg",
							//	Tag="100",
							//	ObjectState=ServiceProtocol.ObjectManager.LogicObjectState.Enabled,
							//},
							//new Bizness.Products.Models.ProductCategoryEditable
							//{
							//	Name="限购",
							//	Title="限购",
							//	Image = "yg-cols-limit-jpg",
							//	Tag = "limit",
							//	ObjectState=ServiceProtocol.ObjectManager.LogicObjectState.Enabled,
							//},
							//new Bizness.Products.Models.ProductCategoryEditable
							//{
							//	Name="新手",
							//	Title="新手",
							//	Image = "yg-cols-newuser-jpg",
							//	Tag = "new",
							//	ObjectState=ServiceProtocol.ObjectManager.LogicObjectState.Enabled,
							//}
						},
						(pid, i) => new CategoryQueryArgument { ParentId = i.ParentId, Name = i.Name },
						ic => ic.Children,
						(ic, i) => ic.Id = i.Id
						).Result;
					});
			var standard=await CategoryManager.EnsureEntity(
				await CategoryManager.QuerySingleEntityIdent(new CategoryQueryArgument { Name = "标准分类" }),
				(CategoryInternal c) =>
				{
					c.Name = "标准分类";
					c.Title = "标准分类";
					c.ObjectState = EntityLogicState.Enabled;
					c.Children = CategoryManager.FillTreeIdentByName(
							ObjectKey.From(c.Id), new[] {
								new CategoryInternal{Order=1, Name="话费充值",Title="话费充值", Description="移动 联通 电信",Icon=StaticRes.File+"-pc-prdcats-recharge18-png",Image=StaticRes.File+"-pc-prdcats-recharge64-png"},
								new CategoryInternal{Order=2, Name="手机",Title="手机专区",Description="三星 iPhone 华为", Icon=StaticRes.File+"-pc-prdcats-phone18-png", Image=StaticRes.File+"-pc-prdcats-phone64-png"},
								new CategoryInternal{Order=3, Name="数码",Title="数码专区",Description="佳能 尼康 索尼", Icon=StaticRes.File+"-pc-prdcats-camera18-png", Image=StaticRes.File+"-pc-prdcats-camera64-png"},
								new CategoryInternal{Order=4, Name="汽车",Title="汽车专区", Description="奔驰 大众 宝马", Icon=StaticRes.File+"-pc-prdcats-car18-png", Image=StaticRes.File+"-pc-prdcats-car64-png"},
								new CategoryInternal{Order=5, Name="黄金",Title="黄金专区",Description="金条 金元宝", Icon=StaticRes.File+"-pc-prdcats-gold18-png", Image=StaticRes.File+"-pc-prdcats-gold64-png"},
							},
						(pid, i) => new CategoryQueryArgument { ParentId = i.ParentId, Name = i.Name },
						ic => ic.Children,
						(ic, i) => ic.Id = i.Id
						).Result;
					//Children=types.Types.Select(
					//	t=>new Bizness.Products.Models.ProductCategoryEditable
					//	{
					//		Name=t.Title,
					//		Title=t.Title,
					//		Image=t.Image,
					//		Icon=t.Icon,
					//		ObjectState=ServiceProtocol.ObjectManager.LogicObjectState.Enabled,
					//	}
					//	).ToArray()

				});
			//var special = cats.Where(c => c.Name == "特别分类").Single();
			//var c100 = cats.Where(c => c.Name == "百元").Single();
			//var c10 = cats.Where(c => c.Name == "十元").Single();
			//var recharge = cats.Where(c => c.Name == "上线充值活动").Single();
			//var climit = cats.Where(c => c.Name == "限购").Single();
			//var cnew = cats.Where(c => c.Name == "新手").Single();

			//var standard = cats.Where(c => c.Name == "标准分类").Single();
			var ctypes = (await CategoryManager.QueryAsync(new CategoryQueryArgument { ParentId = standard.Id },Paging.All)).Items.ToArray();
				

			//var ctx = scope.Resolve<IDataContext>();

            var setCatProduct= new Func<string, string,Task>(async (cat,typeName) =>
            {
                var type = ctypes.First(c => c.Name == cat);
                var ptype = types.Types.Where(t => t.Name == typeName).Single();
				var ids = await ItemManager.QueryIdentsAsync(new ItemQueryArgument { TypeId = ptype.Id }, Paging.All);
				//await ctx.ReadOnly<DataModels.ProductItem>()
    //                .Where(i => i.SourceItemId == null && i.Product.TypeId == ptype.Id)
    //                .Select(i => i.Id).ToArrayAsync();
                await CategoryManager.SetItems(type.Id, ids.Items.Select(i=>i.Id).ToArray());
            });
            await setCatProduct("话费充值", "话费充值");
            await setCatProduct("手机", "手机");
            await setCatProduct("数码", "数码");
            await setCatProduct("汽车", "汽车");
            await setCatProduct("黄金", "黄金");


			//         foreach (var type in ctypes)
			//{
			//	var ptype = types.Types.Where(t => t.Name == type.Name).Single();
			//	var ids = await ctx.ReadOnly<DataModels.ProductItem>()
			//		.Where(i => i.SourceItemId == null && i.Product.TypeId == ptype.Id)
			//		.Select(i => i.Id).ToArrayAsync();
			//	await scope.ProductCategorySetItems(type.Id, ids);
			//}


			//         var pids100=await ctx.ReadOnly<DataModels.ProductItem>()
			//	.Where(p => p.SourceItemId==null && p.Product.CFPriceUnitExpect == 100)
			//	.Select(p => p.Id).ToArrayAsync();

			//await scope.ProductCategorySetItems(c100.Id, pids100);


			//var pids10 = await ctx.ReadOnly<DataModels.ProductItem>()
			//	.Where(p => p.SourceItemId == null && p.Product.CFPriceUnitExpect == 10)
			//	.Select(p => p.Id).ToArrayAsync();

			//await scope.ProductCategorySetItems(c10.Id, pids10);

			//var activityProducts = new[]
			//{
			//    "中国移动50元话费充值卡",
			//    "中国移动100元话费充值卡",
			//    "中国移动500元话费充值卡",
			//    "中国电信500元话费充值卡",
			//    "OPPO R9s 全网通4G+64GB 双卡双待手机 玫瑰金",
			//    "华为P9 全网通 4GB+64GB版 玫瑰金 移动联通电信4G手机 双卡双待",
			//    "iPhone 7 128GB 土豪金 移动联通电信4G手机",
			//    "iPhone 7 128GB 玫瑰金 移动联通电信4G手机",
			//    "凯迪拉克ATS-L 2016款 28T 精英型",
			//    "奥迪A4L 2017款40TFSI 时尚型",
			//    "奔驰C200 2016款C200L4MATIC 运动版",
			//    "斯巴鲁  傲虎 2.5运动导航版"
			//};

			//var pidsRecharge = await ctx.ReadOnly<DataModels.ProductItem>()
			//    .Where(p => activityProducts.Contains(p.Product.Title))
			//    .Select(p=>new { name = p.Product.Title, id = p.Id })
			//    .Take(12).ToDictionaryAsync(p => p.name, p => p.id);

			//await scope.ProductCategorySetItems(recharge.Id, activityProducts.Select(p=>pidsRecharge.Get(p)).Where(i=>i>0).ToArray());

			//var pids_limit = await ctx.ReadOnly<DataModels.ProductItem>()
			//	.OrderByDescending(p=>p.Id)
			//	.Take(100)
			//	.Select(p => p.Id).ToArrayAsync();

			//await scope.ProductCategorySetItems(climit.Id, pids_limit);


			//var pids_newuser = await ctx.ReadOnly<DataModels.ProductItem>()
			//	.OrderByDescending(p => p.Price)
			//	.Take(100)
			//	.Select(p => p.Id).ToArrayAsync();
			//await scope.ProductCategorySetItems(cnew.Id, pids_newuser);


			await ServiceInstanceManager.UpdateSetting<HygouSetting>(
			   ScopeId,
			   s =>
			   {
				   s.MainProductCategoryId = standard.Id;
			   });
			return new ProductCategoryInitializer
			{
				//C10 = c10,
				//C100 = c100,
				//Limit = climit,
				//NewUser = cnew,
				MainProductCategoryId= standard.Id,
				SpecialCategoryRoot =special,
				TypedCategoryRoot= standard,
				TypedCategorys=ctypes
			};
		}

	}
}
