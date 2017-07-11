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
	public class ServiceInstance : SF.Data.DataModels.UIDataEntityBase<long>
	{

		//[ForeignKey(nameof(ServiceCategoryItem))]
		//public override long Id { get => base.Id; set => base.Id = value; }

		//public ServiceCategoryItem ServiceCategoryItem { get; set; }

		
		[Index("Parent",1)]
		public long? ParentId { get; set; }

		[ForeignKey(nameof(ParentId))]
		public ServiceInstance Parent { get; set; }

		[InverseProperty(nameof(Parent))]
		public ICollection<ServiceInstance> Children { get; set; }

		[Comment("服务定义")]
		[MaxLength(100)]
		[Required]
		[Index("service",1)]
		[Index("Parent", 2)]
		public string ServiceType { get; set; }

		[Comment("主接口实现")]
		[Required]
		[MaxLength(100)]
		[Index("impl", 1)]
		public string ImplementType { get; set; }

		[Comment("默认服务")]
		[Index("service", 2)]
		public bool IsDefaultService { get; set; }

		[Comment("服务设置")]

		public string Setting { get; set; }
	}
}
