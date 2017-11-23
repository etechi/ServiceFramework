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
using SF.Data;
using SF.Metadata;
using SF.Entities;

namespace SF.Biz.Products.Entity.DataModels
{
	public class Product :
		Product<Product, ProductDetail, ProductType, Category, CategoryItem, PropertyScope, Property, PropertyItem, Item, ProductSpec>
	{ }
	[Table("BizProduct")]
    [Comment(GroupName = "产品服务", Name = "产品")]
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
        [Key]
        [Display(Name="Id")]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public long Id{get; set;}

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
        [Display(Name="对象逻辑状态")]
        public EntityLogicState ObjectState { get; set; }

		[Index("all_type_new", Order = 2)]
		[Index("all_type_price", Order = 2)]
		[Index("all_type_visited", Order = 2)]
		[Index("all_type_order", Order = 2)]
		[Index("all_type_sell", Order = 2)]
		public long TypeId { get; set; }
		[ForeignKey(nameof(TypeId))]

		public TProductType Type { get; set; }

		[MaxLength(100)]
		[Required]
        [Display(Name = "产品名称")]
        public string Name { get; set; }

		[MaxLength(100)]
		[Required]
        [Display(Name = "产品标题")]
        public string Title { get; set; }

        [Display(Name = "图片")]
        public string Image { get; set; }

        [Display(Name = "虚拟产品",Description ="如卡密")]
        public bool IsVirtual { get; set; }

        [Display(Name = "市场价")]
        public decimal MarketPrice { get; set; }

		[Index("all_price", Order = 2)]
		[Index("all_type_price", Order = 3)]
        [Display(Name = "售价")]
        public decimal Price { get; set; }

        [Display(Name = "禁止使用优惠券")]
        public bool CouponDisabled { get; set; }

        [Index("all_visited", Order = 2)]
		[Index("all_type_visited", Order = 3)]
        [Display(Name = "访问次数")]
        public int Visited { get; set; }

		[Index("all_sell", Order = 2)]
		[Index("all_type_sell", Order = 3)]
        [Display(Name = "销售次数")]
        public int SellCount { get; set; }

		[Index("all_order", Order = 2)]
		[Index("all_type_order", Order = 3)]
        [Display(Name = "排位")]
        public double Order { get; set; }

		[Index("all_new", Order = 2)]
		[Index("all_type_new", Order = 3)]
        [Display(Name = "上架时间")]
        public DateTime? PublishedTime { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreatedTime { get; set; }
        [Display(Name = "更新时间")]
        public DateTime UpdatedTime { get; set; }

		[Index]
        [Display(Name = "所有者ID")]
        public long OwnerUserId { get; set; }

		[InverseProperty(nameof(Item<TProduct,TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem, TProductSpec>.Product))]
		public ICollection<TItem> Items { get; set; }

		[InverseProperty(nameof(PropertyItem<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>.Product))]
		public ICollection<TPropertyItem> PropertyItems { get; set; }

		[ForeignKey(nameof(Id))]
		public TProductDetail Detail { get; set; }

        [Display(Name = "乐观锁时间戳")]
        [ConcurrencyCheck]
        [Timestamp]
        public byte[] TimeStamp { get; set; }

        [InverseProperty(nameof(ProductSpec<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem, TProductSpec>.Product))]
        public ICollection<TProductSpec> Specs { get; set; }

		[Display(Name = "自动发货规格")]
		public long? VIADSpecId { get; set; }

	}
}
