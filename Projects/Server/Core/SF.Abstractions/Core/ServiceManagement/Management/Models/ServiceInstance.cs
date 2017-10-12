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

using SF.Core.ServiceManagement.Management;
using SF.Entities;
using SF.Metadata;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SF.Core.ServiceManagement.Models
{
	[EntityObject]
	public class ServiceInstance : 
		SF.Data.Models.UIObjectEntityBase<long>
	{
		[Comment("服务标识")]
		[TableVisible]
		[MaxLength(100)]
		public string ServiceIdent { get; set; }

		[Comment("优先级")]
		[TableVisible]
		public int ItemOrder { get; set; }

	}
	public class ServiceInstanceInternal : ServiceInstance,IItemEntity<long?>,ITreeNodeEntity<ServiceInstanceInternal>
	{
		[Comment("服务定义")]
		[EntityIdent(typeof(ServiceDeclaration), nameof(ServiceName))]
		[Required]
		public string ServiceId { get; set; }

		[Comment("服务实现类型")]
		[Required]
		public string ServiceType { get; set; }

		[Ignore]
		[TableVisible] 
		[Comment("服务名称")]
		public string ServiceName { get; set; }

		[Comment("服务实现ID")]
		[EntityIdent(typeof(ServiceImplement), nameof(ImplementName), ScopeField = nameof(ServiceId))]
		[Required]
		public string ImplementId { get; set; }

		[Comment("服务实现类型")]
		[Required]
		public string ImplementType { get; set; }

		[Ignore]
		[TableVisible]
		[Comment("服务实现名称")]
		public string ImplementName { get; set; }

		[Comment("父服务实例")]
		[EntityIdent(typeof(ServiceInstance), nameof(ContainerName))]
		public long? ContainerId { get; set; }

		[Ignore]
		[TableVisible]
		[Comment("父服务")]
		public string ContainerName { get; set; }

		public IEnumerable<ServiceInstanceInternal> Children { get; set; }
	}

	
	public class ServiceInstanceEditable : ServiceInstanceInternal
	{

		[Comment(Name = "服务设置", Description = "此项设置和具体的服务实现相关，更改服务实现以后需要保存后才会生效，原服务实现的设置会丢失。")]
		[PropertyType(PropertyTypeSourceType.External, nameof(SettingType))]
		[Layout(50)]
		public string Setting { get; set; }

		[Ignore]
		public string SettingType { get; set; }
	}

}
