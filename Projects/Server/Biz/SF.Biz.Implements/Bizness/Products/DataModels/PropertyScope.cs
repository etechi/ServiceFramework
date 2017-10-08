using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;
using SF.Data;
using SF.Entities;

namespace SF.Biz.Products.Entity.DataModels
{
	public class PropertyScope :
		   PropertyScope<Product, ProductDetail, ProductType, Category, CategoryItem, PropertyScope, Property, PropertyItem, Item, ProductSpec>
	{ }

	[Table("BizProductPropertyScope")]
    [Comment(GroupName = "产品服务", Name = "产品属性分区")]
    public class PropertyScope<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
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
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public long Id{get; set;}

		[Index("name", IsUnique =true,Order =1)]
        [Display(Name = "产品类型ID")]
        public long TypeId { get; set; }

		[ForeignKey(nameof(TypeId))]
		public TProductType Type { get; set; }

		[Required]
		[Index("name", IsUnique = true, Order = 2)]
		[MaxLength(50)]
        [Display(Name = "产品属性分区名称")]
        public string Name { get; set; }

		[MaxLength(200)]
        [Display(Name = "产品属性分区图片")]
        public string Image { get; set; }

		[MaxLength(200)]
        [Display(Name = "产品属性分区图标")]
        public string Icon { get; set; }

		[Index]
        [Display(Name = "排位")]
        public int Order { get; set; }

        [Display(Name = "对象逻辑状态")]
        public EntityLogicState ObjectState { get; set; }

		[InverseProperty(nameof(Property<TProduct,TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem, TProductSpec>.Scope))]
		public ICollection<TProperty> Properties { get; set; }
	}
}
