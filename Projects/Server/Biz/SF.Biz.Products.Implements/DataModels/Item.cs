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
	public class Item :
		Item<Product, ProductDetail, ProductType, Category, CategoryItem, PropertyScope, Property, PropertyItem, Item, ProductSpec>
	{ }

	/// <summary>
	/// 商品
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
	[Table("BizProductItem")]
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
		/// <summary>
		/// ID
		/// </summary>
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public long Id { get; set; }

		/// <summary>
		/// 产品ID
		/// </summary>
		[Index]
        public long ProductId { get; set; }

		[ForeignKey(nameof(ProductId))]
		public TProduct Product { get; set; }

		/// <summary>
		/// 分类标签
		/// </summary>
		public string CategoryTags { get; set; }

		/// <summary>
		/// 卖家ID
		/// </summary>
		[Index]
        public long SellerId { get; set; }

		/// <summary>
		/// 源商品ID
		/// </summary>
		[Index]
        public long? SourceItemId { get; set; }

		[ForeignKey(nameof(SourceItemId))]
		public TItem SourceItem { get; set; }

		/// <summary>
		/// 源等级
		/// </summary>
		public int SourceLevel { get; set; }

		/// <summary>
		/// 价格
		/// </summary>
        public decimal? Price { get; set; }
		///<title>图片</title>
		/// <summary>
		/// 默认使用产品图片
		/// </summary>
		[MaxLength(100)]
        public string Image { get; set; }

		/// <summary>
		/// 标题
		/// </summary>
		[MaxLength(100)]
        public string Title { get; set; }

		[InverseProperty(nameof(SourceItem))]
		public ICollection<TItem> ChildItems { get; set; }

		[InverseProperty(nameof(CategoryItem<TProduct, TProductDetail, TProductType, TCategory, TCategoryItem, TPropertyScope, TProperty, TPropertyItem, TItem,TProductSpec>.Item))]
		public ICollection<TCategoryItem> CategoryItems { get; set; }

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
	}
}
