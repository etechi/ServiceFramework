using SF.Data;
using SF.Data.Models;
using SF.Metadata;
using System.Collections.Generic;
using System.ComponentModel;
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
		public int Priority { get; set; }

	}
	public class ServiceInstanceInternal : ServiceInstance,IHierarachicalEntity<long?>
	{
		[Comment("服务定义")]
		[EntityIdent("系统服务定义", nameof(ServiceName))]
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

		[Comment("父服务实例")]
		[EntityIdent("系统服务实例", nameof(ParentName))]
		public long? ParentId { get; set; }

		[Ignore]
		[TableVisible]
		[Comment("父服务")]
		public string ParentName { get; set; }
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
