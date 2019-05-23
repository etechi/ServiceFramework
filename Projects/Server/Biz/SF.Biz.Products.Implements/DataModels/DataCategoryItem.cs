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

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Data;

namespace SF.Biz.Products.Entity.DataModels
{
	public class DataCategoryItem
    {
		/// <summary>
		/// 分类ID
		/// </summary>
		[Key]
		[Column(Order = 1)]
		[Index("order",Order=1)]
		public long CategoryId { get; set; }

		[ForeignKey(nameof(CategoryId))]
		public DataCategory Category { get; set; }

		/// <summary>
		/// 项目ID
		/// </summary>
		[Key]
		[Column(Order = 2)]
		[Index]
        public long ItemId { get; set; }

		/// <summary>
		/// 排位
		/// </summary>
		[Index("order", Order = 2)]
        public long Order { get; set; }

		[ForeignKey(nameof(ItemId))]
		public DataItem Item { get; set; }


	}
}
