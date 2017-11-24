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

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.AspNetCore;
using SF.Services.Settings;
using SF.Biz.Products;
using SF.Entities;

namespace SFShop.Site.Controllers 
{
	public class ProductController : BaseController
	{

		public IItemService ItemService { get; }
		public HygouSetting Setting { get; }
		public ProductController(
			IItemService ItemService,
			 ISettingService<HygouSetting> Setting
			)
		{
			this.Setting = Setting.Value;
			this.ItemService = ItemService;
		}
		//async Task<ViewResult> ShowCurrentRoundProduct(IItem item)
		//{
		//	if (item.CurRound.Round > 1)
		//	{
		//		var history = await this.CFRoundService.Value.GetRound(
		//			item.ItemId,
		//			item.CurRound.Round - 1
		//			);
		//		base.ViewBag.HistoryRound = history;
		//	}
		//	return View("HistoryRoundProduct.cshtml", item);
		//}

		public async Task<ActionResult> Product(int ProductId)
		{
			var product = await this.ItemService.GetProductDetail(ProductId);
			if (product == null)
				return Redirect("/");
			return Redirect("/item/" + product.MainItemId);
		}

		//async Task<Product> EnsureCurRoundExists(Product product)
		//{
		//	if (product.CurRound != null && product.CurRound.State == DataModels.CFState.Selling)
		//		return product;
		//	await this.CFRoundManager.Value.EnsureSellingRound(product.Id, (int)product.Price);
		//	return await ProductService.Value.GetProductAsync(product.Id, true);
		//}
		//如果提供轮次，ItemId为产品ID
		//[OutputCache(Duration = 1)]
		public async Task<ActionResult> Item(long ItemId)
		{
			await this.LoadUIPageBlocks("产品详细");
			
			var product = await this.ItemService.GetProductDetail(ItemId);
			if (product == null)
				return Redirect("/");
			//if (product.CurRound.Round != Round.Value || product.CurRound.Selled == product.CurRound.Total)
			//{
			//	var round = await this.CFRoundService.Value.GetRound(ItemId, Round.Value);
			//	if (round == null)
			//		return Redirect("/");
			//	base.ViewBag.Round = round;

			//	var tradeitems = await this.CFRoundService.Value.GetRoundTimeLineTradeItems(ItemId, Round.Value, 107);
			//	ViewBag.TradeItems = tradeitems;
			//	return View("HistoryRoundProduct", await this.ItemService.GetItemDetail(product.MainItemId));
			//}

			ItemId = product.MainItemId;
				
			var item = await this.ItemService.GetItemDetail(ItemId);
			if (item == null)
				return Redirect("/");

			//if (item.CurRound.LimitPerRoundPerUser.HasValue && CurrentUserId == 0)
			//	return Redirect("/user/signin?jump=/item/" + ItemId);

			ViewBag.Title = item.Title;
			return View("CurrentRoundProduct", item);
		}
		//[OutputCache(Duration = 1)]
		public async Task<ViewResult> Category(long? CategoryId = null)
		{
			if (CategoryId.HasValue && CategoryId.Value == 0) CategoryId = null;

			await this.LoadUIPageBlocks("产品分类");

			var cats = await ItemService.ListCategories(
				this.Setting.MainProductCategoryId,
				null
				);
			ViewBag.Categories = cats.Items;
			ViewBag.CurCatId = CategoryId;
			ViewBag.MainCategoryId = this.Setting.MainProductCategoryId;

			ViewBag.Title = cats.Items.Where(t => t.Id == (CategoryId ?? 0)).FirstOrDefault()?.Title ?? "所有产品";
			var items = await this.ItemService.ListCategoryItems(
				CategoryId ?? this.Setting.MainProductCategoryId,
				true,
				null,
				new Paging
				{
					Count = 40,
					SortMethod = "hot",
					SortOrder = SortOrder.Desc
				}
				);
			return View("Category", items);
		}

		//[OutputCache(Duration = 1)]
		public async Task<ActionResult> Collection(int CategoryId)
		{
			await this.LoadUIPageBlocks("产品集列表");
			var cat = await this.ItemService.GetCategory(CategoryId);
			if (cat == null)
				return Redirect("/");
			var items = await this.ItemService.ListCategoryItems(
				CategoryId,
				true,
				null,
				new Paging
				{
					Count = 40,
					SortMethod = "hot",
					SortOrder = SortOrder.Desc
				}
				);
			ViewBag.Title = cat.Title;
			ViewBag.Collection = cat;
			return View("Collection", items);

		}

		[HttpPost]
		public async Task<ActionResult> Search(string key)
		{
			key = new string(key.Where(c => char.IsLetterOrDigit(c)).ToArray());
			if (string.IsNullOrEmpty(key))
			{
				return Redirect("/cat");
			}
			await this.LoadUIPageBlocks("产品分类");
			var cats = await ItemService.ListCategories(
				this.Setting.MainProductCategoryId,
				null
				);
			ViewBag.Title = key + "搜索结果";
			ViewBag.Categories = cats.Items;
			ViewBag.Keyword = key;
			ViewBag.MainCategoryId = this.Setting.MainProductCategoryId;
			var products = await this.ItemService.ListCategoryItems(
				this.Setting.MainProductCategoryId,
				true,
				key,
				new Paging
				{
					Count = 40,
					SortMethod = "hot",
					SortOrder = SortOrder.Desc
				}
				);
			return View("Category", products);

		}

	}
}
