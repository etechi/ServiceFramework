using SF.Core.ServiceManagement.Models;
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
	[Table("SysServiceCategoryItem")]
	[Comment("服务目录项目")]
	public class ServiceCategoryItem : SF.Data.DataModels.UIDataEntityBase<long>
	{
		[Comment("父项目Id")]
		[Index]
		public long? ParentId { get; set; }

		[ForeignKey(nameof(ParentId))]
		public ServiceCategoryItem Parent { get; set; }

		[InverseProperty(nameof(Parent))]
		public ICollection<ServiceCategoryItem> Children { get; set; }

		[Comment("服务目录项目类型")]
		public ServiceCategoryItemType Type { get; set; }

		
		public ServiceInstance ServiceInstance { get; set; }
	}
}
