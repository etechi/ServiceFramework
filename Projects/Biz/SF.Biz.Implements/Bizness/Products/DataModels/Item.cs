using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;
using SF.Entities;
using SF.Data;

namespace SF.Biz.Products.Entity.DataModels
{

	[Table("BizProductItem")]
    [Comment(GroupName = "产品服务", Name = "商品")]
    public class Item<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>:
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
        [Display(Name ="ID")]
		public long Id { get; set; }

		[Index]
        [Display(Name = "产品ID")]
        public long ProductId { get; set; }

		[ForeignKey(nameof(ProductId))]
		public TProduct Product { get; set; }

        [Display(Name = "分类标签")]
        public string CategoryTags { get; set; }

		[Index]
        [Display(Name = "卖家ID")]
        public long SellerId { get; set; }

		[Index]
        [Display(Name = "源商品ID")]
        public long? SourceItemId { get; set; }

		[ForeignKey(nameof(SourceItemId))]
		public TItem SourceItem { get; set; }

        [Display(Name = "源等级")]
        public int SourceLevel { get; set; }

        [Display(Name = "价格", Description = "默认使用产品图片")]
        public decimal? Price { get; set; }
		[MaxLength(100)]
        [Display(Name = "图片",Description ="默认使用产品图片")]
        public string Image { get; set; }

		[MaxLength(100)]
        [Display(Name = "标题", Description = "默认使用产品标题")]
        public string Title { get; set; }

		[InverseProperty(nameof(SourceItem))]
		public ICollection<TItem> ChildItems { get; set; }

		[InverseProperty(nameof(CategoryItem<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>.Item))]
		public ICollection<TCategoryItem> CategoryItems { get; set; }

        [Display(Name = "逻辑状态")]
        public EntityLogicState ObjectState { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreatedTime { get; set; }
        [Display(Name = "修改时间")]
        public DateTime UpdatedTime { get; set; }
	}
}
