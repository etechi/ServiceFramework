using SF.Data;
using SF.Data.Models;
using SF.Metadata;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Core.ServiceManagement.Models
{
	[EntityObject("系统服务实例")]
	public class ServiceInstance : SF.Data.Models.UIEntityBase<long>
	{
		[Comment("默认服务")]
		[TableVisible]
		public bool IsDefaultService { get; set; }

	}
	public class ServiceInstanceInternal : ServiceInstance
	{
		[Comment("服务类型")]
		[EntityIdent("系统服务类型", nameof(ServiceName))]
		[Required]
		public string ServiceType { get; set; }

		[Ignore]
		[TableVisible]
		[Comment("服务名称")]
		public string ServiceName { get; set; }

		[Comment("服务实现")]
		[EntityIdent("系统服务实现", nameof(ImplementName), ScopeField = nameof(ServiceType))]
		[Required]
		public string ImplementType { get; set; }

		[Ignore]
		[TableVisible]
		[Comment("服务实现")]
		public string ImplementName { get; set; }

		[Comment("父服务实例ID")]
		public long? ParentInstanceId { get; set; }

		public string ParentInstanceName { get; set; }
	}

	public class ServiceInstanceInterface
	{
		[Key]
		[ReadOnly(true)]
		[Comment(Name = "接口类型")]
		public string InterfaceType { get; set; }

		[ReadOnly(true)]
		[Comment(Name = "接口名称")]
		public string InterfaceName { get; set; }

		[Key]
		[ReadOnly(true)]
		[Comment(Name = "实现类型")]
		public string ImplementType { get; set; }

		[ReadOnly(true)]
		[Comment(Name = "实现名称")]
		public string ImplementName { get; set; }


		[Comment(Name = "服务设置", Description = "此项设置和具体的服务实现相关，更改服务实现以后需要保存后才会生效，原服务实现的设置会丢失。")]
		[PropertyType(PropertyTypeSourceType.External, nameof(SettingType))]
		[Layout(50)]
		public string Setting { get; set; }

		[Ignore]
		public string SettingType { get; set; }
	}

	public class ServiceInstanceEditable : ServiceInstanceInternal
	{
		
		public UIDisplayData DisplayData { get; set; }

		[Comment("服务设置")]
		public IEnumerable<ServiceInstanceInterface> Interfaces { get; set; }
	}

}
