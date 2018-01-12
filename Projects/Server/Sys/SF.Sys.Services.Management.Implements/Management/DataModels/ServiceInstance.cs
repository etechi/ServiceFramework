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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Sys.Services.Management.DataModels
{
	/// <summary>
	/// 服务实例
	/// </summary>
	[Table("ServiceInstance")]
	public class DataServiceInstance : SF.Sys.Entities.DataModels.DataUITreeNodeEntityBase<DataServiceInstance>
	{
		//[ForeignKey(nameof(ServiceCategoryItem))]
		//public override long Id { get => base.Id; set => base.Id = value; }

		//public ServiceCategoryItem ServiceCategoryItem { get; set; }
		
		[Index("TypedService",1)]
		public override long? ContainerId { get; set; }

		[ForeignKey(nameof(ContainerId))]
		public override DataServiceInstance Container { get; set; }

		[InverseProperty(nameof(Container))]
		public override IEnumerable<DataServiceInstance> Children { get; set; }

		/// <summary>
		/// 服务定义ID
		/// </summary>
		[MaxLength(40)]
		[Required]
		[Index("TypedService", 2)]
		public string ServiceId { get; set; }

		/// <summary>
		/// 服务定义
		/// </summary>
		[Required]
		public string ServiceType { get; set; }

		/// <summary>
		/// 接口实现ID
		/// </summary>
		[Required]
		[MaxLength(40)]
		[Index("impl", 1)]
		public string ImplementId { get; set; }

		/// <summary>
		/// 接口实现
		/// </summary>
		[Required]
		public string ImplementType { get; set; }

		/// <summary>
		/// 服务优先级
		/// </summary>
		public override int ItemOrder { get; set; }

		/// <summary>
		/// 服务标识
		/// </summary>
		[Index]
		[MaxLength(200)]
		public string ServiceIdent { get; set; }

		/// <summary>
		/// 服务设置
		/// </summary>
		public string Setting { get; set; }
	}
}
