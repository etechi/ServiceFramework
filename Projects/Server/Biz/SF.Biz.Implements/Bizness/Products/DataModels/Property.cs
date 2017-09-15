using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;
using SF.Data;
using SF.Entities;

namespace SF.Biz.Products.Entity.DataModels
{

	[Table("BizProductProperty")]
    [Comment(GroupName = "产品服务", Name = "产品属性")]
    public class Property<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>
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
        [Display(Name="ID")]
		public long Id { get; set; }

		[Index]
        [Display(Name = "分区ID")]
        public long ScopeId { get; set; }

		[Index("name", IsUnique = true, Order = 1)]
        [Display(Name = "产品类型ID")]
        public long TypeId { get; set; }

		[Index("name", IsUnique = true, Order = 2)]
		[Index("order", Order = 1)]
        [Display(Name = "父属性ID")]
        public long? ParentId { get; set; }

		[Index("name", IsUnique = true, Order = 3)]
		[Required]
		[MaxLength(50)]
        [Display(Name = "属性名")]
        public string Name { get; set; }

        [Display(Name = "属性图标")]
        public string Icon { get; set; }
        [Display(Name = "属性图片")]

        public string Image { get; set; }

        [Display(Name = "对象逻辑状态")]

        public EntityLogicState ObjectState { get; set; }


		[Index("order", Order = 2)]
        [Display(Name = "排位")]
        public int Order { get; set; }


		[ForeignKey(nameof(ParentId))]
		public TProperty Parent { get; set; }

		[InverseProperty(nameof(Property<TProduct,TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem,TItem, TProductSpec>.Parent))]
		public ICollection<TProperty> Children { get; set; }

		[InverseProperty(nameof(PropertyItem<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>.Property))]
		public ICollection<TPropertyItem> ProductItems { get; set; }

		[ForeignKey(nameof(ScopeId))]
		public TPropertyScope Scope { get; set; }

		[ForeignKey(nameof(TypeId))]
		public TProductType Type { get; set; }
	}
}
