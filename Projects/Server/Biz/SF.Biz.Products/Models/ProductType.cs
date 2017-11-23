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

using SF.Sys.Annotations;
using SF.Sys.Entities;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Products
{
	[EntityObject]
    public class ProductType :
		IEntityWithId<long>,
		IEntityWithName
	{
		/// <summary>
		/// ID
		/// </summary>
		[TableVisible]
		[Key]
		[ReadOnly(true)]
		public long Id { get; set; }

		/// <summary>
		/// 名称
		/// </summary>
		[TableVisible]
		[Required]
		[StringLength(100)]
		public string Name { get; set; }


		/// <summary>
		/// 标题
		/// </summary>
		[TableVisible]
		[Required]
		[StringLength(100)]
		public string Title { get; set; }



		/// <summary>
		/// 图片
		/// </summary>
		[Layout(0, 1)]
		[Image]
		public string Image { get; set; }

		/// <summary>
		/// 图标
		/// </summary>
		[Layout(0, 2)]
		[Image]
		public string Icon { get; set; }

		/// <summary>
		/// 产品数量
		/// </summary>
		[TableVisible]
		[ReadOnly(true)]
		public int ProductCount { get; set; }

		[Ignore]
		public PropertyScope[] PropertyScopes { get; set; }

    }
	public class ProductTypeEditable : ProductType
	{
		/// <summary>
		/// 状态
		/// </summary>
		[Required]
		public EntityLogicState ObjectState { get; set; }

		/// <summary>
		/// 显示排位
		/// </summary>
		[ReadOnly(true)]
        [Optional]
        public int Order { get; set; }

		/// <summary>
		/// 单位
		/// </summary>
		[TableVisible]
		[Required]
		[StringLength(4)]
		public string Unit { get; set; }
	}
	public class ProductTypeInternal : ProductTypeEditable
	{
		/// <summary>
		/// 更新时间
		/// </summary>
		[TableVisible]
		public DateTime UpdatedTime { get; set; }
		public DateTime CreatedTime { get; set; }

	}

}
