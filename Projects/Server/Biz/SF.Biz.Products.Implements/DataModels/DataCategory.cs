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

using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Biz.Products.Entity.DataModels
{
	public class DataCategory :DataUIObjectEntityBase
    {
		

		/// <summary>
		/// 标签
		/// </summary>
		[Index]
		[MaxLength(20)]
        public string Tag { get; set; }

		/// <summary>
		/// 父分类ID
		/// </summary>
		[Index]
        public long? ParentId { get; set; }

		[ForeignKey(nameof(ParentId))]
		public DataCategory Parent { get; set; }

		/// <summary>
		/// 排位
		/// </summary>
        public int Order { get; set; }


		/// <summary>
		/// PC广告图链接
		/// </summary>
		[MaxLength(200)]
        public string BannerUrl { get; set; }

		/// <summary>
		/// PC广告图
		/// </summary>
		[MaxLength(200)]
        public string BannerImage { get; set; }

		/// <summary>
		/// 移动广告图
		/// </summary>
		[MaxLength(200)]
        public string MobileBannerImage { get; set; }

		/// <summary>
		/// 移动广告图链接
		/// </summary>
		[MaxLength(200)]
        public string MobileBannerUrl { get; set; }

		[InverseProperty(nameof(DataCategoryItem.Category))]
		public ICollection<DataCategoryItem> Items { get; set; }

		[InverseProperty(nameof(DataCategory.Parent))]
		public ICollection<DataCategory> Children { get; set; }

	
		/// <summary>
		/// 项目数量
		/// </summary>
        public int ItemCount { get; set; }

        [Index]
        public long? ShopId { get; set; }
	
	}
}
