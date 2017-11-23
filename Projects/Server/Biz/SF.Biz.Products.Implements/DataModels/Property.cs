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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Data;
using SF.Sys.Entities;

namespace SF.Biz.Products.Entity.DataModels
{
	public class Property :
		Property<Product, ProductDetail, ProductType, Category, CategoryItem, PropertyScope, Property, PropertyItem, Item, ProductSpec>
	{ }
	/// <summary>
	/// 产品属性
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
	[Table("BizProductProperty")]
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
        
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public long Id { get; set; }

		/// <summary>
		/// 分区ID
		/// </summary>
		[Index]
        public long ScopeId { get; set; }

		/// <summary>
		/// 产品类型ID
		/// </summary>
		[Index("name", IsUnique = true, Order = 1)]
        public long TypeId { get; set; }

		/// <summary>
		/// 父属性ID
		/// </summary>
		[Index("name", IsUnique = true, Order = 2)]
		[Index("order", Order = 1)]
        public long? ParentId { get; set; }

		/// <summary>
		/// 属性名
		/// </summary>
		[Index("name", IsUnique = true, Order = 3)]
		[Required]
		[MaxLength(50)]
        public string Name { get; set; }

		/// <summary>
		/// 属性图标
		/// </summary>
		public string Icon { get; set; }
		/// <summary>
		/// 属性图片
		/// </summary>

		public string Image { get; set; }

		/// <summary>
		/// 对象逻辑状态
		/// </summary>

		public EntityLogicState ObjectState { get; set; }

		/// <summary>
		/// 排位
		/// </summary>
		[Index("order", Order = 2)]
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
