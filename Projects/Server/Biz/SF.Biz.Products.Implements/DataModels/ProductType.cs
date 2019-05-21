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
	public class ProductType :
		ProductType<Product, ProductDetail, ProductType, Category, CategoryItem, PropertyScope, Property, PropertyItem, Item, ProductSpec>
	{ }
	/// <summary>
	/// 产品类型
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
	[Table("BizProductType")]
    
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
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public long Id { get; set; }

		/// <summary>
		/// 产品类型名称
		/// </summary>
		[Index(IsUnique = true)]
		[Required]
		[MaxLength(100)]
		public string Name { get; set; }

		/// <summary>
		/// 产品类型标题
		/// </summary>
		[Required]
		[MaxLength(100)]

        public string Title { get; set; }

		/// <summary>
		/// 单位
		/// </summary>
		[MaxLength(20)]        
        public string Unit { get; set; }

		/// <summary>
		/// 排位
		/// </summary>
		[Index]
        public int Order { get; set; }

        /// <summary>
        /// 发货类型
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string DeliveryProvider { get; set; }

        /// <summary>
        /// 对象逻辑状态
        /// </summary>
        public EntityLogicState ObjectState { get; set; }

		/// <summary>
		/// 图标
		/// </summary>
		[MaxLength(100)]
        public string Icon { get; set; }

		/// <summary>
		/// 图片
		/// </summary>
		[MaxLength(100)]
        public string Image { get; set; }
		
		[InverseProperty(nameof(PropertyScope<TProduct,TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec >.Type))]
		public ICollection<TPropertyScope> PropertyScopes { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreatedTime { get; set; }
		/// <summary>
		/// 更新时间
		/// </summary>
		public DateTime UpdatedTime { get; set; }

		/// <summary>
		/// 产品数量
		/// </summary>
		public int ProductCount { get; set; }
	}
}
