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
using SF.Sys.Entities.DataModels;

namespace SF.Biz.Products.Entity.DataModels
{
    public class DataItem: DataUIObjectEntityBase
    {
        /// <summary>
		/// 名称
		/// </summary>
		[MaxLength(100)]
        [Index]
        public override string Name { get; set; }

        ///<title>标题</title>
        /// <summary>
        /// 用于UI显示
        /// </summary>
        [MaxLength(100)]
        [Required]
        public override string Title { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        [Index]
        public long ProductId { get; set; }

		[ForeignKey(nameof(ProductId))]
		public DataProduct Product { get; set; }

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
		public DataItem SourceItem { get; set; }

		/// <summary>
		/// 源等级
		/// </summary>
		public int SourceLevel { get; set; }

		/// <summary>
		/// 价格
		/// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 店铺
        /// </summary>
        [Index]
        public long? ShopId { get; set; }

        [InverseProperty(nameof(SourceItem))]
		public ICollection<DataItem> ChildItems { get; set; }

		[InverseProperty(nameof(DataCategoryItem.Item))]
		public ICollection<DataCategoryItem> CategoryItems { get; set; }

		

	
	}
}
