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

using SF.Biz.Trades;
using SF.Sys.Entities;
using System;
using System.Collections.Generic;

namespace SF.Biz.Products
{
	public interface IItemCached :IEntityWithId<long>
	{
		long SellerId { get; }
		long ProductId { get; }
	}
	public interface IProductCached : IEntityWithId<long>
	{
        bool IsVirtual { get; }
        string Name { get; }
		string Title { get; }
		decimal MarketPrice { get; }
		decimal Price { get; }
		string Image { get; }
		string[] Tags { get; }
		DateTime? PublishedTime { get; }
		int Visited { get; }
		long MainItemId { get; }
		long ProviderId { get; }
        bool OnSale { get; }
        bool CouponDisabled { get;}
        string DeliveryProvider { get; }
    }

	public class ItemCached : IItemCached
	{
		public long Id { get; set; }
		public long SellerId { get; set; }
		public long ProductId { get; set; }
	}

	public class ProductCached : IProductCached
	{
		public long Id { get; set; }
        public bool IsVirtual { get; set; }
		public string Title { get; set; }
        public string Name { get; set; }
		public decimal MarketPrice { get; set; }
		public decimal Price { get; set; }
		public string Image { get; set; }
		public string[] Tags { get; set; }
		public DateTime? PublishedTime { get; set; }
		public int Visited { get; set; }
		public long MainItemId { get; set; }
		public long ProviderId { get; set; }
        public bool OnSale{ get; set; }
        public bool CouponDisabled { get; set; }
        public string DeliveryProvider { get; set; }
    }


    public interface IProductContentCached : IEntityWithId<long>
	{
		IEnumerable<IProductImage> Images { get; }
		IEnumerable<IProductDescItem> Descs { get; }
	}


	public class ProductContentCached : IProductContentCached
	{
		public long Id { get; set; }
		public IEnumerable<IProductImage> Images { get; set; }
		public IEnumerable<IProductDescItem> Descs { get; set; }
	}

	public interface ICategoryCached : IEntityWithId<long>
	{
		string Tag { get; }
		string Title { get; }
		string Description { get; }
		string Image { get; }
		string Icon { get; }
		int Order { get; }
		int ItemCount { get; }
        string BannerImage { get;  }
        string BannerUrl { get; }
        string MobileBannerImage { get; }
        string MobileBannerUrl { get; }
    }

    public class CategoryCached :
		ICategoryCached,
		IEntityWithId<long>
	{
		public long Id { get; set; }
		public string Tag { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string Image { get; set; }
		public string Icon { get; set; }

        public string BannerImage { get; set; }
        public string BannerUrl { get; set; }
        public string MobileBannerImage { get; set; }
        public string MobileBannerUrl { get; set; }

        public int Order { get; set; }
		public int ItemCount { get; set; }
	}

	public interface IItem 
	{
		long ItemId { get; }
		long SellerId { get; }
		long ProductId { get; }
		string Title { get; }
		string Image { get; }
		decimal MarketPrice { get; }
		decimal Price { get; }
		string[] Tags { get; }
		DateTime? PublishedTime { get; }
		int Visited { get; }
		long MainItemId { get; }
        bool IsVirtual { get; }
        bool CouponDisabled { get; }
		IEnumerable<IProductImage> Images { get; }
		IEnumerable<IProductDescItem> Descs { get; }
        bool OnSale { get; }

    }

	public class Item : IItem, ITradableItem
    {
		protected IItemCached ItemCached { get; }
		protected IProductCached ProductCached { get; }
		protected IProductContentCached Content { get; }
		public Item(IItemCached ItemCached, IProductCached ProductCached, IProductContentCached Content)
		{
			this.ItemCached = ItemCached;
			this.ProductCached = ProductCached;
			this.Content = Content;
		}
        public override string ToString()
        {
            return ItemId + ":" + ProductId + ":" + Title;
        }
        public virtual long ItemId { get { return ItemCached?.Id ?? 0; } }
		public virtual long SellerId { get { return ItemCached?.SellerId ?? ProductCached.ProviderId; } }
		public virtual long ProductId { get { return ProductCached.Id; } }
        public virtual bool IsVirtual { get { return ProductCached.IsVirtual; } }
        public virtual string Name { get { return ProductCached.Name; } }
        public virtual string Title { get { return ProductCached.Title; } }
		public virtual string Image { get { return ProductCached.Image; } }
		public virtual decimal MarketPrice { get { return ProductCached.MarketPrice; } }
		public virtual decimal Price { get { return ProductCached.Price; } }
		public virtual string[] Tags { get { return ProductCached.Tags; } }
		public virtual DateTime? PublishedTime { get { return ProductCached.PublishedTime; } }
		public virtual int Visited { get { return ProductCached.Visited; } }
		public virtual long MainItemId { get { return ProductCached.MainItemId; } }
		public virtual IEnumerable<IProductImage> Images { get { return Content?.Images ?? null; } }
		public virtual IEnumerable<IProductDescItem> Descs { get { return Content?.Descs ?? null; } }
        public virtual bool OnSale{ get { return ProductCached.OnSale; } }
        public virtual bool CouponDisabled { get { return ProductCached.CouponDisabled; } }

        string ITradableItem.Id => "Item-"+ItemId;


        int? ITradableItem.StockLeft => null;

        EntityLogicState ITradableItem.LogicState => EntityLogicState.Enabled;

        string ITradableItem.DeliveryProvider => ProductCached.DeliveryProvider;
    }
}
