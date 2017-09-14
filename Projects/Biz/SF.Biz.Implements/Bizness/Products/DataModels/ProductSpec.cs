using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;
using SF.Entities;
using SF.Data;

namespace SF.Biz.Products.Entity.DataModels
{

	[Table("app_biz_product_spec")]
    [Comment(GroupName = "产品服务", Name = "产品规格")]
    public class ProductSpec<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>:
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
		public long Id{get; set;}

        [Index]
        public long ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public TProduct Product { get; set; }


        [Required]
        [MaxLength(100)]
        [Display(Name = "名称")]
        public string Name { get; set; }

        [MaxLength(200)]

        [Display(Name = "图片")]
        public string Image { get; set; }

        [Display(Name = "描述")]
        public string Desc { get; set; }

        [Display(Name = "逻辑状态")]
        public EntityLogicState ObjectState { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreatedTime { get; set; }

        [Display(Name = "修改时间")]
        public DateTime UpdatedTime { get; set; }

        [Display(Name = "排位")]
        public int Order { get; set; }

		[Display(Name = "自动发货规格")]
		public long? VIADSpecId { get; set; }


	}
}
