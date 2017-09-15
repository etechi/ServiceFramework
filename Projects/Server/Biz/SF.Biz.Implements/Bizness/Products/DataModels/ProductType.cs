using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;
using SF.Entities;
using SF.Data;

namespace SF.Biz.Products.Entity.DataModels
{

	[Table("BizProductType")]
    [Comment(GroupName = "产品服务", Name = "产品类型")]
    public class ProductType<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>:
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
        [Display(Name="ID")]
		public long Id { get; set; }

		[Index(IsUnique = true)]
		[Required]
		[MaxLength(100)]
        [Display(Name="产品类型名称")]
		public string Name { get; set; }

		[Required]
		[MaxLength(100)]
        [Display(Name = "产品类型标题")]
        public string Title { get; set; }

		[MaxLength(20)]
        [Display(Name = "单位")]
        public string Unit { get; set; }

		[Index]
        [Display(Name = "排位")]
        public int Order { get; set; }

        [Display(Name = "对象逻辑状态")]
        public EntityLogicState ObjectState { get; set; }

		[MaxLength(100)]
        [Display(Name = "图标")]
        public string Icon { get; set; }
		[MaxLength(100)]
        [Display(Name = "图片")]
        public string Image { get; set; }
		
		[InverseProperty(nameof(PropertyScope<TProduct,TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec >.Type))]
		public ICollection<TPropertyScope> PropertyScopes { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreatedTime { get; set; }
        [Display(Name = "更新时间")]
        public DateTime UpdatedTime { get; set; }

        [Display(Name = "产品数量")]
        public int ProductCount { get; set; }
	}
}
