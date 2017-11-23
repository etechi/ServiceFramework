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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities;
using SF.Sys.Data;

namespace SF.Biz.Products.Entity.DataModels
{
	public class Product :
		Product<Product, ProductDetail, ProductType, Category, CategoryItem, PropertyScope, Property, PropertyItem, Item, ProductSpec>
	{ }
	/// <summary>
	/// 产品
	/// </summary>
	/// <typeparam name="TProduct"></typeparam>
	/// <typeparam name="TProductDetail"></typeparam>
	/// <typeparam name="TProductType"></typeparam>
	/// <typeparam name="TCategory"></typeparam>
	/// <typeparam name="TCategoryItem"></typeparam>
	/// <typeparam name="TPropertyScope"></typeparam>
	/// <typeparam name="TProperty"></typeparam>
	/// <typeparam name="TPropertyItem"></typeparam>
	/// <typeparam name="TItem"></typeparam>
	/// <typeparam name="TProductSpec"></typeparam>
	[Table("BizProduct")]
    public class Product<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>:
		IEntityWithId<long>
		where TProduct : Product<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TProductDetail : ProductDetail<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TProductType : ProductType<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TCategory : Category<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TCategoryItem : CategoryItem<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TPropertyScope : PropertyScope<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TProperty : Property<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TPropertyItem : PropertyItem<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
		where TItem : Item<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
        where TProductSpec : ProductSpec<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
    {
		/// <summary>
		/// Id
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public long Id{get; set;}

		/// <summary>
		/// 对象逻辑状态
		/// </summary>
		[Index("all_new",Order=1)]
		[Index("all_price", Order = 1)]
		[Index("all_visited", Order = 1)]
		[Index("all_order", Order = 1)]
		[Index("all_sell", Order = 1)]
		[Index("all_type_new", Order = 1)]
		[Index("all_type_price", Order = 1)]
		[Index("all_type_visited", Order = 1)]
		[Index("all_type_order", Order = 1)]
		[Index("all_type_sell", Order = 1)]        
        public EntityLogicState ObjectState { get; set; }

		[Index("all_type_new", Order = 2)]
		[Index("all_type_price", Order = 2)]
		[Index("all_type_visited", Order = 2)]
		[Index("all_type_order", Order = 2)]
		[Index("all_type_sell", Order = 2)]
		public long TypeId { get; set; }
		[ForeignKey(nameof(TypeId))]

		public TProductType Type { get; set; }

		/// <summary>
		/// 产品名称
		/// </summary>
		[MaxLength(100)]
		[Required]
        public string Name { get; set; }

		/// <summary>
		/// 产品标题
		/// </summary>
		[MaxLength(100)]
		[Required]
        public string Title { get; set; }

		/// <summary>
		/// 图片
		/// </summary>
		public string Image { get; set; }

		///<title>虚拟产品</title>
		/// <summary>
		/// 如卡密
		/// </summary>
        public bool IsVirtual { get; set; }

		/// <summary>
		/// 市场价
		/// </summary>
		public decimal MarketPrice { get; set; }

		/// <summary>
		/// 售价
		/// </summary>
		[Index("all_price", Order = 2)]
		[Index("all_type_price", Order = 3)]
        public decimal Price { get; set; }

		/// <summary>
		/// 禁止使用优惠券
		/// </summary>
		public bool CouponDisabled { get; set; }

		/// <summary>
		/// 访问次数
		/// </summary>
		[Index("all_visited", Order = 2)]
		[Index("all_type_visited", Order = 3)]        
        public int Visited { get; set; }
		/// <summary>
		/// 销售次数
		/// </summary>
		[Index("all_sell", Order = 2)]
		[Index("all_type_sell", Order = 3)]

        public int SellCount { get; set; }

		/// <summary>
		/// 排位
		/// </summary>
		[Index("all_order", Order = 2)]
		[Index("all_type_order", Order = 3)]
        public double Order { get; set; }

		/// <summary>
		/// 上架时间
		/// </summary>
		[Index("all_new", Order = 2)]
		[Index("all_type_new", Order = 3)]
        public DateTime? PublishedTime { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreatedTime { get; set; }

		/// <summary>
		/// 更新时间
		/// </summary>
		public DateTime UpdatedTime { get; set; }

		/// <summary>
		/// 所有者ID
		/// </summary>
		[Index]
        public long OwnerUserId { get; set; }

		[InverseProperty(nameof(Item<TProduct,TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem, TProductSpec>.Product))]
		public ICollection<TItem> Items { get; set; }

		[InverseProperty(nameof(PropertyItem<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>.Product))]
		public ICollection<TPropertyItem> PropertyItems { get; set; }

		[ForeignKey(nameof(Id))]
		public TProductDetail Detail { get; set; }

		/// <summary>
		/// 乐观锁时间戳
		/// </summary>
		[ConcurrencyCheck]
        [Timestamp]
        public byte[] TimeStamp { get; set; }

        [InverseProperty(nameof(ProductSpec<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem, TProductSpec>.Product))]
        public ICollection<TProductSpec> Specs { get; set; }

		/// <summary>
		/// 自动发货规格
		/// </summary>
		public long? VIADSpecId { get; set; }

	}
}
