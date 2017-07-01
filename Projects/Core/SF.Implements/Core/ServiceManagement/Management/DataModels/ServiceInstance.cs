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

		[Comment("父服务实例Id")]
		[Index]
		public long? ParentInstanceId { get; set; }

		[ForeignKey(nameof(ParentInstanceId))]
		public ServiceInstance ParentInstance { get; set; }

		[InverseProperty(nameof(ParentInstance))]
		public ICollection<ServiceInstance> ChildInstances { get; set; }

		[Comment("服务定义")]
		[MaxLength(100)]
		[Required]
		[Index("service",1)]
		public string ServiceType { get; set; }

		[Comment("主接口实现")]
		[Required]
		[MaxLength(100)]
		[Index("impl", 1)]
		public string ImplementType { get; set; }

		[Comment("默认服务")]
		[Index("service", 2)]
		public bool IsDefaultService { get; set; }

		[InverseProperty(nameof(ServiceInstanceInterface.ServiceInstance))]
		public ICollection<ServiceInstanceInterface> Interfaces { get; set; }
	}
}
