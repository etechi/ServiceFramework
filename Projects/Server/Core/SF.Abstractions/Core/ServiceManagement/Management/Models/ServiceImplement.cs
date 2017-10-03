using SF.Core.ServiceManagement.Management;
using SF.Entities;
using SF.Metadata;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Core.ServiceManagement.Models
{
	[EntityObject]
	public class ServiceImplement: IEntityWithId<string>
	{
		[Key]
		[Comment("ID")]
		[ReadOnly(true)]
		[TableVisible]
		public string Id { get; set; }

		[Comment("名称")]
		[ReadOnly(true)]
		[TableVisible]
		public string Type { get; set; }

		[Comment("名称")]
		[ReadOnly(true)]
		[TableVisible]
		public string Name { get; set; }

		[Comment("描述")]
		[ReadOnly(true)]
		[TableVisible]
		public string Description { get; set; }

		[Comment("分组")]
		[ReadOnly(true)]
		[TableVisible]
		public string Group { get; set; }

		[Comment("是否禁用")]
		[TableVisible]
		public bool Disabled { get; set; }

		[Comment("服务定义")]
		[EntityIdent(typeof(ServiceDeclaration), nameof(DeclarationName))]
		[ReadOnly(true)]
		public string DeclarationId { get; set; }

		[Ignore]
		[TableVisible]
		[Comment("服务定义")]
		public string DeclarationName { get; set; }
	}
}
