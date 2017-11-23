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
using SF.Sys.Entities;
using SF.Sys.Data;

namespace SF.Biz.Products.Entity.DataModels
{
	public class ProductSpec :
		ProductSpec<Product, ProductDetail, ProductType, Category, CategoryItem, PropertyScope, Property, PropertyItem, Item, ProductSpec>
	{ }
	/// <summary>
	/// 产品规格
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
	[Table("BizProductSpec")]
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
		public long Id{get; set;}

        [Index]
        public long ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public TProduct Product { get; set; }

		/// <summary>
		/// 名称
		/// </summary>
		[Required]
        [MaxLength(100)]
        public string Name { get; set; }

		/// <summary>
		/// 图片
		/// </summary>
		[MaxLength(200)]
        public string Image { get; set; }

		/// <summary>
		/// 描述
		/// </summary>
		public string Desc { get; set; }

		/// <summary>
		/// 逻辑状态
		/// </summary>
		public EntityLogicState ObjectState { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreatedTime { get; set; }

		/// <summary>
		/// 修改时间
		/// </summary>
		public DateTime UpdatedTime { get; set; }

		/// <summary>
		/// 排位
		/// </summary>
		public int Order { get; set; }

		/// <summary>
		/// 自动发货规格
		/// </summary>
		public long? VIADSpecId { get; set; }


	}
}
