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
using SF.Sys.Entities.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Biz.Products
{
	[EntityObject]
    public class ProductType : UIObjectEntityBase
	{
		/// <summary>
		/// 产品数量
		/// </summary>
		[TableVisible]
		[ReadOnly(true)]
		public int ProductCount { get; set; }

        /// <summary>
        /// 发货类型
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string DeliveryProvider { get; set; }

        [Ignore]
		public PropertyScope[] PropertyScopes { get; set; }

    }
	public class ProductTypeEditable : ProductType
	{

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

	}

}
