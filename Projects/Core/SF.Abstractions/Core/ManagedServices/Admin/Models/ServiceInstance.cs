using SF.Data;
using SF.Metadata;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Core.ManagedServices.Models
{
	[EntityObject("系统服务实例")]
	public class ServiceInstance : IObjectWithId<string>
	{
		[Key]
		[Comment("ID")]
		[TableVisible]
		[ReadOnly(true)]
		public string Id { get; set; }
		[Comment("服务实例名称")]
		[TableVisible]
		[MaxLength(100)]
		[Required]
		public string Name { get; set; }

		[Comment("默认服务")]
		[TableVisible]
		public bool IsDefaultService { get; set; }

		[Comment("对象状态")]
		[TableVisible]
		public LogicObjectState LogicState { get; set; }


		[Comment("服务实例标题", "用于UI显示")]
		[TableVisible]
		[MaxLength(100)]
		[Required]
		[Layout(100)]
		public string Title { get; set; }

		[Comment("服务实例说明", "用于UI显示")]
		[MaxLength(200)]
		public string Description { get; set; }

		[Comment("服务实例图标")]
		[Image]
		public string Icon { get; set; }

		[Comment("服务实例图片")]
		[Image]
		public string Image { get; set; }
	}
	public class ServiceInstanceInternal : ServiceInstance
	{
		
		[Comment("服务定义")]
		[EntityIdent("系统服务定义", nameof(DeclarationName))]
		[Required]
		public string DeclarationId { get; set; }

		[Ignore]
		[TableVisible]
		[Comment("服务定义")]
		public string DeclarationName { get; set; }

		[Comment("服务实现")]
		[EntityIdent("系统服务实现", nameof(ImplementName), ScopeField = nameof(DeclarationId))]
		[Required]
		public string ImplementId { get; set; }

		[Ignore]
		[TableVisible]
		[Comment("服务实现")]
		public string ImplementName { get; set; }


	}

	public class ServiceInstanceEditable : ServiceInstanceInternal
	{

		[Comment("备注",Description="内部使用")]
		[MaxLength(200)]
		public string Remarks { get; set; }


		[Comment(Name = "服务设置", Description = "此项设置和具体的服务实现相关，更改服务实现以后需要保存后才会生效，原服务实现的设置会丢失。")]
		[PropertyTypeAttribute(PropertyTypeSourceType.External, nameof(SettingType))]
		[Layout(50)]
		public string Setting { get; set; }

		[Ignore]
		public string SettingType { get; set; }
	}

}
