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

using SF.Biz.Products;
using SF.Core;
using SF.Core.ServiceManagement.Management;
using SF.Entities;
using SF.Management.FrontEndContents;
using SF.Management.FrontEndContents.Friendly;
using SF.Management.FrontEndContents.SiteConfigModels;
using SF.Services.Settings;
using System.Linq;
using System.Threading.Tasks;

namespace SFShop.Setup
{
	public static class PCSiteInitializer
	{
		public static async Task PCSiteEnsure(
			IServiceInstanceManager sim,
			long? ScopeId,
			IContentManager ContentManager,
			ISiteTemplateManager SiteTemplateManager,
			ISiteManager SiteManager,
			IItemService ItemService,
			ProductContentInitializer prdctns,
			ProductCategoryInitializer collection,
			long PageTailDocListContentId,
            long PageTailLinkListContentId,
			long MainCategoryId
			)
		{
			var head_carousel=await ContentManager.ContentEnsure(
				"PC页面内容",
				"PC首页幻灯片",
				null,
				new[] {
				new ContentItem{Image=StaticRes.File+"-pc-carousel-1-jpg",Uri="/activity/register"},
				new ContentItem{Image=StaticRes.File+"-pc-carousel-2-jpg",Uri="/activity/recharge"},
				//new ContentItem{Image=StaticResRoot.Value+"-pc-carousel-3-jpg"},
				//new ContentItem{Image=StaticResRoot.Value+"-pc-carousel-4-jpg"},
				//new ContentItem{Image=StaticResRoot.Value+"-pc-carousel-5-jpg"},
				//new ContentItem{Image=StaticResRoot.Value+"-pc-carousel-6-jpg"},
				//new ContentItem{Image=StaticResRoot.Value+"-pc-carousel-7-jpg"},
				//new ContentItem{Image=StaticResRoot.Value+"-pc-carousel-8-jpg"},
				//new ContentItem{Image=StaticResRoot.Value+"-pc-carousel-9-jpg"},
				});
            await sim.UpdateSetting<FriendlyContentSetting>(
				ScopeId,
				s =>
				{
					s.PCHomePageSliderId = head_carousel.Id;
				});

			var banner_1 = await ContentManager.ContentEnsure(
                "PC广告位",
                "PC首页广告位1",
                null,
                new[] { new ContentItem { Image = StaticRes.File + "-pc-banners-1-gif", Uri = "/activity/register" } },
                "PC首页广告位，位于第1,第2产品分类之间"
                );
			var banner_2 = await ContentManager.ContentEnsure(
                "PC广告位",
                "PC首页广告位2",
                null,
                new[] { new ContentItem { Image = StaticRes.File + "-pc-banners-2-jpg", Uri = "/activity/recharge" } },
                "PC首页广告位，位于第3,第4产品分类之间"
                );
			var banner_3 = await ContentManager.ContentEnsure(
                "PC广告位",
                "PC首页广告位3",
                null,
                new[] { new ContentItem { Image = StaticRes.File + "-pc-banners-2-jpg", Uri = "/activity/recharge" } },
                "PC首页广告条，位于第5,第6产品分类之间"
                );
			var banner_4 = await ContentManager.ContentEnsure(
                "PC广告位",
                "PC首页广告位4",
                null,
                new[] { new ContentItem { Image = StaticRes.File + "-pc-banners-2-jpg", Uri = "/activity/recharge" } },
                "PC首页广告位，位于第7,第8产品分类之间"
                );
			await sim.UpdateSetting<FriendlyContentSetting>(
			   ScopeId,
			   s =>
			   {
				   s.PCAdCategory = "PC广告位";
			   });
			

			var cats = await ItemService.ListCategories(MainCategoryId, null);
			var items = new[] {
				//new ContentItem{Title1="奢侈品区", Icon="yg-icons-1-png", Image="yg-icons-1b-png",Uri="/cat/6"},
				//new ContentItem{Title1="汽车专区", Icon="yg-icons-2-png", Image="yg-icons-2b-png",Uri="/cat/8"},
				//new ContentItem{Title1="手机数码", Icon="yg-icons-3-png", Image="yg-icons-3b-png",Uri="/cat/2"},
				//new ContentItem{Title1="家用电器", Icon="yg-icons-4-png", Image="yg-icons-4b-png",Uri="/cat/5"},
				//new ContentItem{Title1="金银珠宝", Icon="yg-icons-5-png", Image="yg-icons-5b-png",Uri="/cat/1"},
				//new ContentItem{Title1="云购超市", Icon="yg-icons-6-png", Image="yg-icons-6b-png",Uri="/cat/4"},
				//new ContentItem{Title1="服饰百货", Icon="yg-icons-7-png", Image="yg-icons-7b-png",Uri="/cat/3"},
				//new ContentItem{Title1="其他商品", Icon="yg-icons-8-png", Image="yg-icons-8b-png",Uri="/cat/7"},
                new ContentItem{Title1="话费充值", Title2="移动 联通 电信",Icon=StaticRes.File+"-pc-prdcats-recharge40-png",Uri="/cat/7"},
                new ContentItem{Title1="手机",Title2="三星 iPhone 华为", Icon=StaticRes.File+"-pc-prdcats-phone40-png",Uri="/cat/2"},
                new ContentItem{Title1="数码",Title2="佳能 尼康 索尼", Icon=StaticRes.File+"-pc-prdcats-camera40-png",Uri="/cat/5"},
                new ContentItem{Title1="汽车", Title2="奔驰 大众 宝马", Icon=StaticRes.File+"-pc-prdcats-car40-png",Uri="/cat/8"},
                new ContentItem{Title1="黄金",Title2="金条 金元宝", Icon=StaticRes.File+"-pc-prdcats-gold40-png",Uri="/cat/1"},
                };
			foreach (var it in items)
				it.Uri = "/cat/" + cats.Items.Single(c => c.Title == (it.Title1.Length==2?it.Title1+"专区":it.Title1)).Id;
			var cat_menu = await ContentManager.ContentEnsure(
				"PC页面内容",
				"PC页面头部产品分类菜单",
				null,
				items
				);

            await sim.UpdateSetting<FriendlyContentSetting>(
				ScopeId,
				s =>
				{
					s.PCHeadProductCategoryMenuId = cat_menu.Id;
				});

            var main_menu = await ContentManager.ContentEnsure(
				"PC页面内容",
				"PC页面头部主菜单",
				null,
				new[] {
				new ContentItem{Title1="首页", Uri="/"},
				//new ContentItem{Title1="十元", Uri="/col/"+collection.C10.Id},
				//new ContentItem{Title1="百元", Uri="/col/"+collection.C100.Id},
				//new ContentItem{Title1="限购", Uri="/col/"+collection.Limit.Id},
				new ContentItem{Title1="揭晓", Uri="/open"},
				new ContentItem{Title1="晒单", Uri="/shared"},
				new ContentItem{Title1="新手", Uri="/help/doc/1"}
				});
			await sim.UpdateSetting<FriendlyContentSetting>(
			   ScopeId,
			   s =>
			   {
				   s.PCHeadMenuId = main_menu.Id;
			   });
			

            var tpl = await SiteTemplateManager.SiteTemplateEnsure(
				"PC网站",
				new SiteModel
				{
					name = "PC网站",
					pages = new[]
					{
#region 页面公共内容
						new PageModel
						{
							ident="页面公共内容",
							name="页面公共内容",
							blocks=new[]
							{
								new BlockModel
								{
									ident="头部产品分类菜单",
									contents=new []
									{
										new BlockContentModel
										{
											name="分类菜单",
											content=cat_menu.Id,
											render="razor",
											view="~/Views/Renders/Utils/HeaderCategoryMenu.cshtml"
										}
									}
								},
								new BlockModel
								{
									ident="头部主菜单",
									contents=new []
									{
										new BlockContentModel
										{
											name="主菜单",
											content=main_menu.Id,
											render="razor",
											view="~/Views/Renders/Utils/HeaderMainMenu.cshtml"
										}
									}
								},
								new BlockModel
								{
									ident="尾部文档列表",
									contents=new []
									{
										new BlockContentModel
										{
											name ="文档列表",
											content=PageTailDocListContentId,
											render="razor",
											view="~/Views/Renders/Utils/TailDocList.cshtml"
										}
									}
								},
                                new BlockModel
                                {
                                    ident="尾部链接列表",
                                    contents=new []
                                    {
                                        new BlockContentModel
                                        {
                                            name ="文档列表",
                                            content=PageTailLinkListContentId,
                                            render="razor",
                                            view="~/Views/Renders/Utils/TailLinkList.cshtml"
                                        }
                                    }
                                }
                            }
						},
#endregion

#region 首页
						new PageModel
						{
							ident="首页",
							name="首页",
							includes=new [] {"页面公共内容"},
							blocks=new []
							{
								new BlockModel
								{
									ident="幻灯片区域",
									contents=new []
									{
										new BlockContentModel
										{
											name ="幻灯片",
											content=head_carousel.Id,
											render="razor",
											view="~/Views/Renders/Utils/Carousel.cshtml"
										}
									}
								},
								new BlockModel
								{
									ident="主体内容",
									contents=new []
									{
										new BlockContentModel
										{
											name ="最新揭晓",
											content=prdctns.AllRounds.Id,
											render="razor",
											view="~/Views/Renders/Products/OpenWaitingRoundList.cshtml",
											contentConfig=Json.Stringify(new {args=new{ Paging=new Paging{Count=5 } } }),
											title1="最新揭晓",
                                            title2="揭晓",
                                            title3="1F"
                                        },
										new BlockContentModel
										{
											name ="2楼",
                                            title1="话费充值",
											content=prdctns.CategoriesProducts[0].Id,
											render="razor",
											view="~/Views/Renders/Products/ProductList.cshtml",
											contentConfig=Json.Stringify(new {args=new{ Paging=new Paging{Count=4 } } }),
                                            title2="充值",
                                            title3="2F"
										},
										new BlockContentModel
										{
											name ="广告1",
											content=banner_1.Id,
											render="razor",
											view="~/Views/Renders/Utils/Banner.cshtml",
										},
										new BlockContentModel
										{
											name ="3楼",
                                            title1="手机专区",
                                            content=prdctns.CategoriesProducts[1].Id,
											render="razor",
											view="~/Views/Renders/Products/ProductList.cshtml",
											contentConfig=Json.Stringify(new {args=new{ Paging=new Paging{Count=4 } } }),
											title3 ="3F",
                                            title2="手机"
										},
										new BlockContentModel
										{
											name ="4楼",
                                            title1="数码专区",
                                            content=prdctns.CategoriesProducts[2].Id,
											render="razor",
											view="~/Views/Renders/Products/ProductList.cshtml",
											contentConfig=Json.Stringify(new { args=new{Paging=new Paging{Count=4 } } }),
                                            title2="数码",
											title3 ="4F"
										},
										new BlockContentModel
										{
											name ="广告2",
											content=banner_2.Id,
											render="razor",
											view="~/Views/Renders/Utils/Banner.cshtml",
										},
										new BlockContentModel
										{
											name ="5楼",
                                            title1="汽车专区",
                                            content=prdctns.CategoriesProducts[3].Id,
											render="razor",
											view="~/Views/Renders/Products/ProductList.cshtml",
											contentConfig=Json.Stringify(new {args=new{ Paging=new Paging{Count=4 } } }),
                                            title2="汽车",
                                            title3 ="5F"
                                            
                                        },
                                        new BlockContentModel
                                        {
                                            name ="6楼",
                                            title1="黄金专区",
                                            content=prdctns.CategoriesProducts[4].Id,
                                            render="razor",
                                            view="~/Views/Renders/Products/ProductList.cshtml",
                                            contentConfig=Json.Stringify(new { args=new{Paging=new Paging{Count=4 } } }),
                                            title2="黄金",
                                            title3 ="6F"
                                        },
										//new BlockContentModel
										//{
										//	name ="广告3",
										//	content=banner_3.Id,
										//	render="razor",
										//	view="~/Views/Renders/Utils/Banner.cshtml",
										//},
										//new BlockContentModel
										//{
										//	name ="6楼",
										//	content=prdctns.CategoriesProducts[5].Id,
										//	render="razor",
										//	view="~/Views/Renders/Products/ProductList.cshtml",
										//	contentConfig="{\"_pl\":4}",
										//	title3 ="6F"
										//},
										//new BlockContentModel
										//{
										//	name ="7楼",
										//	content=prdctns.CategoriesProducts[6].Id,
										//	render="razor",
										//	view="~/Views/Renders/Products/ProductList.cshtml",
										//	contentConfig="{\"_pl\":4}",
										//	title3 ="7F"
										//},
										//new BlockContentModel
										//{
										//	name ="广告4",
										//	content=banner_4.Id,
										//	render="razor",
										//	view="~/Views/Renders/Utils/Banner.cshtml",
										//},
										//new BlockContentModel
										//{
										//	name ="8楼",
										//	content=prdctns.CategoriesProducts[7].Id,
										//	render="razor",
										//	view="~/Views/Renders/Products/ProductList.cshtml",
										//	contentConfig="{\"_pl\":4}",
										//	title3 ="8F"
										//},
										//new BlockContentModel
										//{
										//	name ="新品上架",
										//	content=prdctns.AllProducts.Id,
										//	contentConfig="{\"_pm\":\"new\",\"_pl\":8}",
										//	render="razor",
										//	view="~/Views/Renders/Products/ProductList.cshtml",
										//	viewConfig= "{\"hideStatus\":true}",
										//	title3 ="9F",
										//	title1="新品上架"
										//}
									}
								}
							}
						},
						#endregion
						#region 产品详细

						new PageModel
						{
							ident="产品详细",
							name="产品详细",
							includes=new [] {"页面公共内容"},
							blocks=new []
							{
								new BlockModel
								{
									ident="推荐区域",
									contents=new []
									{
										new BlockContentModel
										{
											name ="推荐",
											content=prdctns.AllProducts.Id,
											contentConfig="{\"_pm\":\"new\",\"_pl\":8}",
											render="razor",
											view="~/Views/Renders/Products/ProductList.cshtml",
											title3 ="9F"
										} 
									}
								}
							}
						},
#endregion
#region 产品分类

						new PageModel
						{
							ident="产品分类",
							name="产品分类",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[0]
						},
#endregion
#region 产品集列表

						new PageModel
						{
							ident="产品集列表",
							name="产品集列表",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[0]
						},
#endregion
#region 用户页面
						new PageModel
						{
							ident="用户注册",
							name="用户注册",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[0]
						},
						new PageModel
						{
							ident="用户登录",
							name="用户登录",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[0]
						},
						new PageModel
						{
							ident="用户找回密码",
							name="用户找回密码",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[0]
						},
                        new PageModel
                        {
                            ident="绑定手机号",
                            name="绑定手机号",
                            includes=new [] {"页面公共内容"},
                            blocks=new BlockModel[0]
                        },
                        new PageModel
						{
							ident="用户首页",
							name="用户首页",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[0]
						},
						new PageModel
						{
							ident="用户夺宝记录",
							name="用户夺宝记录",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[0]
						},
						new PageModel
						{
							ident="用户充值",
							name="用户充值",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[0]
						},
						new PageModel
						{
							ident="充值扫码",
							name="充值扫码",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[0]
						},
						new PageModel
						{
							ident="充值状态",
							name="充值状态",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[0]
						},
						new PageModel
						{
							ident="用户充值成功",
							name="用户充值成功",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[0]
						},
						new PageModel
						{
							ident="用户充值记录",
							name="用户充值记录",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[0]
						},
						new PageModel
						{
							ident="用户中奖记录",
							name="用户中奖记录",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[0]
						},
						new PageModel
						{
							ident="用户晒单列表",
							name="用户晒单列表",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[0]
						},
						new PageModel
						{
							ident="用户晒单页面",
							name="用户晒单页面",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[0]
						},
						new PageModel
						{
							ident="用户收货地址",
							name="用户收货地址",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[0]
						},
						new PageModel
						{
							ident="用户安全中心",
							name="用户安全中心",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[0]
						}   ,
						new PageModel
						{
							ident="用户通知中心",
							name="用户通知中心",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[0]
						}   ,

                        new PageModel
                        {
                            ident="用户积分记录",
                            name="用户积分记录",
                            includes=new [] {"页面公共内容"},
                            blocks=new BlockModel[0]
                        }   ,
                        new PageModel
                        {
                            ident="用户优惠券记录",
                            name="用户优惠券记录",
                            includes=new [] {"页面公共内容"},
                            blocks=new BlockModel[0]
                        }   ,
                        new PageModel
                        {
                            ident="用户专属活动",
                            name="用户专属活动",
                            includes=new [] {"页面公共内容"},
                            blocks=new BlockModel[0]
                        }   ,
                        new PageModel
                        {
                            ident="用户好友邀请",
                            name="用户好友邀请",
                            includes=new [] {"页面公共内容"},
                            blocks=new BlockModel[0]
                        }   ,
#endregion

#region 购物车
					
						new PageModel
						{
							ident="购物车",
							name="购物车",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[0]
						}   ,
#endregion
#region 交易
					
						new PageModel
						{
							ident="用户夺宝成功页",
							name="用户夺宝成功页",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[0]
						}   ,
#endregion
#region 夺宝
					
						new PageModel
						{
							ident="最新揭晓",
							name="最新揭晓",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[] {
								//new BlockModel
								//{
								//	ident="即将揭晓",
								//	contents=new []
								//	{
								//		new BlockContentModel
								//		{
								//			content=prdctns.AllProducts.Id,
								//			render="razor",
								//			view="~/Views/Renders/Products/ProductList.cshtml",
								//			contentConfig="{\"_pl\":10,\"_ps\":\"soon\"}"
								//		},
								//	}
								//}
							}
						}   ,
						new PageModel
						{
							ident="全部晒单",
							name="全部晒单",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[] {}
						}   ,
#endregion

#region 其他用户页面
					
						new PageModel
						{
							ident="其他用户夺宝记录",
							name="其他用户夺宝记录",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[] {}
						}   ,

						new PageModel
						{
							ident="其他用户晒单列表",
							name="其他用户晒单列表",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[] {}
						}   ,
						new PageModel
						{
							ident="其他用户晒单详细",
							name="其他用户晒单详细",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[] {}
						}   ,

						new PageModel
						{
							ident="其他用户中奖记录",
							name="其他用户中奖记录",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[] {}
						}   ,
						#endregion

						#region 帮助中心
						new PageModel
						{
							ident="帮助中心",
							name="帮助中心",
							includes=new [] {"页面公共内容"},
							blocks=new BlockModel[] {}
						}   ,
                        new PageModel
                        {
                            ident="用户反馈",
                            name="用户反馈",
                            includes=new [] {"页面公共内容"},
                            blocks=new BlockModel[] {}
                        }   ,
						#endregion
					}
				}
				);
			await SiteManager.SiteEnsure("main", "PC主站", tpl.Id);




		}

	}
}
