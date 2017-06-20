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
	[Table("SysServiceInstanceInterface")]
	[Comment("服务实例接口")]
	public class ServiceInstanceInterface
	{
		[Comment("服务ID")]
		[Key]
		[Column(Order =1)]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[ForeignKey(nameof(ServiceInstance))]
		public string ServiceInstanceId { get; set; }

		[Comment("接口")]
		[MaxLength(100)]
		[Required]
		[Key]
		[Column(Order = 2)]
		public string InterfaceType { get; set; }

		public ServiceInstance ServiceInstance { get; set; }

		[MaxLength(100)]
		[Required]
		public string ImplementType { get; set; }

		public string Setting { get; set; }
		
	}
}
