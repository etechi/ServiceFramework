using SF.Entities;
using SF.Metadata;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Auth.Identities.Models
{
	[EntityObject]
	[Comment("身份标识")]
	public class Identity : IEntityWithId<long>
    {
		[Key]
		[ReadOnly(true)]
		[TableVisible]
		[Comment("ID")]
		public long Id { get; set; }

		[Comment("名称")]
		[MaxLength(100)]
		[Required]
		[TableVisible]
		public string Name { get; set; }

		[Comment("图标")]
		[MaxLength(100)]
		public string Icon { get; set; }

		[Comment("所属对象")]
		[MaxLength(100)]
		[Required]
		[EntityIdent]
		public string OwnerId { get; set; }

	}
}

