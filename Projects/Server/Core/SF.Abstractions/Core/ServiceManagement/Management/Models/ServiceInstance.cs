using SF.Core.ServiceManagement.Management;
using SF.Entities;
using SF.Metadata;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SF.Core.ServiceManagement.Models
{
	[EntityObject("系统服务实例")]
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
		public string ServiceType { get; set; }

		[Ignore]
		[TableVisible] 
		[Comment("服务名称")]
		public string ServiceName { get; set; }

		[Comment("服务实现")]
		[EntityIdent(typeof(ServiceImplement), nameof(ImplementName), ScopeField = nameof(ServiceType))]
		[Required]
		public string ImplementType { get; set; }


		[Ignore]
		[TableVisible]
		[Comment("服务实现")]
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
