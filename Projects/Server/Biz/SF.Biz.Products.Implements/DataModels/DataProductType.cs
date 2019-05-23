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
	public class DataProductType : DataUIObjectEntityBase
    {

		/// <summary>
		/// 产品类型名称
		/// </summary>
		[Index(IsUnique = true)]
		[Required]
		[MaxLength(100)]
		public override string Name { get; set; }

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

		
		[InverseProperty(nameof(DataPropertyScope.Type))]
		public ICollection<DataPropertyScope> PropertyScopes { get; set; }


		/// <summary>
		/// 产品数量
		/// </summary>
		public int ProductCount { get; set; }
	}
}
