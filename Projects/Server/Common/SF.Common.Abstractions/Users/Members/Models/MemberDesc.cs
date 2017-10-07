using SF.Entities;
using SF.Metadata;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Users.Members.Models
{
	[EntityObject]
	public class MemberDesc : IEntityWithId<long>
	{
		[Comment("ID")]
		[ReadOnly(true)]
		public long Id { get; set; }

		[Comment("名称")]
		[MaxLength(100)]
		public string Name { get; set; }
	}

}

