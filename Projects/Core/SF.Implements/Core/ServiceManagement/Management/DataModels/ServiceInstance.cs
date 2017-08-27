using SF.Data;
using SF.Entities;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Core.ServiceManagement.Management.DataModels
{
	[Table("SysServiceInstance")]
	[Comment("服务实例")]
	public class ServiceInstance : SF.Entities.DataModels.UITreeNodeEntityBase<ServiceInstance>
	{
		//[ForeignKey(nameof(ServiceCategoryItem))]
		//public override long Id { get => base.Id; set => base.Id = value; }

		//public ServiceCategoryItem ServiceCategoryItem { get; set; }
		
		[Index("TypedService",1)]
		public override long? ContainerId { get; set; }

		[ForeignKey(nameof(ContainerId))]
		public override ServiceInstance Container { get; set; }

		[InverseProperty(nameof(Container))]
		public override IEnumerable<ServiceInstance> Children { get; set; }

		[Comment("服务定义")]
		[MaxLength(100)]
		[Required]
		[Index("TypedService", 2)]
		public string ServiceType { get; set; }

		[Comment("接口实现")]
		[Required]
		[MaxLength(300)]
		[Index("impl", 1)]
		public string ImplementType { get; set; }

		[Comment("服务优先级")]
		public override int ItemOrder { get; set; }

		[Comment("服务标识")]
		[Index]
		[MaxLength(200)]
		public string ServiceIdent { get; set; }

		[Comment("服务设置")]
		public string Setting { get; set; }
	}
}
