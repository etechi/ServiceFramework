using SF.Data;
using SF.Data.Storage;
using SF.Metadata;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Core.ServiceManagement.Management.DataModels
{
	[Table("SysServiceInstance")]
	public class ServiceInstance :IObjectWithId<string>
	{
		[Key]
		[Comment("ID")]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public string Id { get; set; }

		[Comment("应用ID")]
		[Index]
		public long AppId { get; set; }

		[Comment("服务实例名称")]
		[MaxLength(100)]
		[Required]
		public string Name { get; set; }

		[Comment("服务实例标题")]
		[MaxLength(100)]
		[Required]
		public string Title { get; set; }


		[Comment("服务实例说明","用于UI显示")]
		[MaxLength(200)]
		public string Description { get; set; }

		[Comment("服务实例备注","内部使用，不用于UI显示")]
		[MaxLength(200)]
		public string Remarks { get; set; }

		[Comment("服务实例图片")]
		[MaxLength(100)]
		public string Image { get; set; }

		[Comment("服务实例图标")]
		[MaxLength(100)]
		public string Icon { get; set; }

		[Comment("服务定义")]
		[MaxLength(100)]
		[Required]
		[Index("service",1)]
		public string DeclarationId { get; set; }

		[Comment("默认服务")]
		[Index("service", 2)]
		public bool IsDefaultService { get; set; }

		[Comment("对象状态")]
		public LogicObjectState LogicState { get; set; }


		[Comment("主要接口实现类")]
		public string ImplementId { get; set; }

		[Comment("服务设置")]
		public string Setting { get; set; }
	}
}
