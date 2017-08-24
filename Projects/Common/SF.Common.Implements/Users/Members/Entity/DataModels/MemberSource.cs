using SF.Entities.DataModels;
using SF.Metadata;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Users.Members.Entity.DataModels
{
	[Table("UserMemberSource")]
	public class MemberSource<TMember, TMemberSource> : ObjectEntityBase
		where TMember : Member<TMember, TMemberSource>
		where TMemberSource : MemberSource<TMember, TMemberSource>
	{
		[Comment("父渠道ID")]
		[ForeignKey(nameof(Parent))]
		public long? ParentId { get; set; }

		public TMember Parent { get; set; }
	}
}

