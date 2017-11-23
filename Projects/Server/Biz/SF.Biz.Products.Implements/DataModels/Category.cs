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
using SF.Metadata;
using SF.Data;
using SF.Entities;

namespace SF.Biz.Products.Entity.DataModels
{
	public class Category :
		Category<Product, ProductDetail, ProductType, Category, CategoryItem, PropertyScope, Property, PropertyItem, Item, ProductSpec>
	{ }
	[Table("BizProductCategory")]
    [Comment(GroupName = "产品服务", Name = "产品分类")]
    public class Category<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>:
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
        [Display(Name="分类ID")]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public long Id {get;set;}

		[Index]
		[MaxLength(20)]
        [Display(Name = "标签")]
        public string Tag { get; set; }

		[Index]
        [Display(Name = "父分类ID")]
        public long? ParentId { get; set; }

		[ForeignKey(nameof(ParentId))]
		public TCategory Parent { get; set; }

        [Display(Name = "排位")]
        public int Order { get; set; }

		[Required]
		[MaxLength(100)]
        [Display(Name = "名称")]
        public string Name{get;set;}

		[MaxLength(100)]
        [Display(Name = "标题")]
        public string Title { get; set; }

		[MaxLength(100)]
        [Display(Name = "描述")]
        public string Description { get; set; }

		[MaxLength(100)]
        [Display(Name = "图标")]
        public string Icon { get; set; }

		[MaxLength(100)]
        [Display(Name = "图片")]
        public string Image { get; set; }

        [MaxLength(200)]
        [Display(Name = "PC广告图链接")]
        public string BannerUrl { get; set; }

        [MaxLength(200)]
        [Display(Name = "PC广告图")]
        public string BannerImage { get; set; }

        [MaxLength(200)]
        [Display(Name = "移动广告图")]
        public string MobileBannerImage { get; set; }

        [MaxLength(200)]
        [Display(Name = "移动广告图链接")]
        public string MobileBannerUrl { get; set; }

		[InverseProperty(nameof(CategoryItem<TProduct,TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem, TProductSpec>.Category))]
		public ICollection<TCategoryItem> Items { get; set; }

		[InverseProperty(nameof(Category<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>.Parent))]
		public ICollection<TCategory> Children { get; set; }

		[Index]
        [Display(Name = "所用者ID")]
        public long OwnerUserId { get; set; }

        [Display(Name = "项目数量")]
        public int ItemCount { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreatedTime { get; set; }
        [Display(Name = "更新时间")]
        public DateTime UpdatedTime { get; set; }

        [Display(Name = "对象逻辑状态")]
        public EntityLogicState ObjectState { get; set; }
	}
}
