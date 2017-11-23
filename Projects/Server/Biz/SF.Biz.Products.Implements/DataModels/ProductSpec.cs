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
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;
using SF.Entities;
using SF.Data;

namespace SF.Biz.Products.Entity.DataModels
{
	public class ProductSpec :
		ProductSpec<Product, ProductDetail, ProductType, Category, CategoryItem, PropertyScope, Property, PropertyItem, Item, ProductSpec>
	{ }
	[Table("BizProductSpec")]
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
