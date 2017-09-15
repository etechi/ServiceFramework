using SF.Entities;
using SF.Metadata;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Auth.Identities.Models
{
	[EntityObject("身份标识")]
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

		[Comment("身份类型")]
		[MaxLength(100)]
		[Required]
		[TableVisible]
		public string Entity { get; set; }

	}
}

