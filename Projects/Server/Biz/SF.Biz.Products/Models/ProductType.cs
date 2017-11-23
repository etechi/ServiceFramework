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

using SF.Entities;
using SF.Metadata;
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
		[Comment(Name ="ID")]
		[TableVisible]
		[Key]
		[ReadOnly(true)]
		public long Id { get; set; }

		[Comment(Name = "名称")]
		[TableVisible]
		[Required]
		[StringLength(100)]
		public string Name { get; set; }


		[Comment(Name = "标题")]
		[TableVisible]
		[Required]
		[StringLength(100)]
		public string Title { get; set; }



		[Comment(Name = "图片")]
		[Layout(0, 1)]
		[Image]
		public string Image { get; set; }

		[Comment(Name = "图标")]
		[Layout(0, 2)]
		[Image]
		public string Icon { get; set; }

		[TableVisible]
		[ReadOnly(true)]
		[Comment(Name ="产品数量")]
		public int ProductCount { get; set; }

		[Ignore]
		public PropertyScope[] PropertyScopes { get; set; }

    }
	public class ProductTypeEditable : ProductType
	{
		[Comment(Name = "状态")]
		[Required]
		public EntityLogicState ObjectState { get; set; }

		[ReadOnly(true)]
		[Comment(Name = "显示排位")]
        [Optional]
        public int Order { get; set; }

		[Comment(Name = "单位")]
		[TableVisible]
		[Required]
		[StringLength(4)]
		public string Unit { get; set; }
	}
	public class ProductTypeInternal : ProductTypeEditable
	{
		[Comment(Name = "更新时间")]
		[TableVisible]
		public DateTime UpdatedTime { get; set; }
		public DateTime CreatedTime { get; set; }

	}

}
