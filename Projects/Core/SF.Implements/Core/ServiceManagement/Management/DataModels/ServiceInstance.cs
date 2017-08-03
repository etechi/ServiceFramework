using SF.Data;
using SF.Data.Storage;
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
	public class ServiceInstance : SF.Data.DataModels.UIObjectEntityBase<long>
	{
		//[ForeignKey(nameof(ServiceCategoryItem))]
		//public override long Id { get => base.Id; set => base.Id = value; }

		//public ServiceCategoryItem ServiceCategoryItem { get; set; }
		
		[Index("TypedService",1)]
		public long? ParentId { get; set; }

		[ForeignKey(nameof(ParentId))]
		public ServiceInstance Parent { get; set; }

		[InverseProperty(nameof(Parent))]
		public ICollection<ServiceInstance> Children { get; set; }

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
		public int Priority { get; set; }

		[Comment("服务标识")]
		[Index]
		[MaxLength(200)]
		public string ServiceIdent { get; set; }

		[Comment("服务设置")]
		public string Setting { get; set; }
	}
}
