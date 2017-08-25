using SF.Entities;
using SF.Metadata;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Core.ServiceManagement.Models
{
	[EntityObject("系统服务定义")]
	public class ServiceDeclaration : 
		IEntityWithId<string>
	{
		[Key]
		[Comment("ID")]
		[ReadOnly(true)]
		[TableVisible]
		public string Id { get; set; }

		[Comment("名称")]
		[ReadOnly(true)]
		[TableVisible]
		public string Name { get; set; }

		[Comment("描述")]
		[ReadOnly(true)]
		[TableVisible]
		public string Description { get; set; }

		[ReadOnly(true)]
		[Comment("分组")]
		[TableVisible]
		public string Group { get; set; }

		[Comment("是否禁用")]
		[TableVisible]
		public bool Disabled { get; set; }
	}
}
