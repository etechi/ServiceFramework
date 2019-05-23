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
using SF.Sys.Entities.DataModels;

namespace SF.Biz.Products.Entity.DataModels
{
	public class DataProperty : DataUIObjectEntityBase
    {
      
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
        public override string Name { get; set; }

		/// <summary>
		/// 排位
		/// </summary>
		[Index("order", Order = 2)]
        public int Order { get; set; }


		[ForeignKey(nameof(ParentId))]
		public DataProperty Parent { get; set; }

		[InverseProperty(nameof(DataProperty.Parent))]
		public ICollection<DataProperty> Children { get; set; }

		[InverseProperty(nameof(DataPropertyItem.Property))]
		public ICollection<DataPropertyItem> ProductItems { get; set; }

		[ForeignKey(nameof(ScopeId))]
		public DataPropertyScope Scope { get; set; }

		[ForeignKey(nameof(TypeId))]
		public DataProductType Type { get; set; }
	}
}
