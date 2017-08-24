using SF.Data;
using SF.Entities.DataModels;
using SF.Metadata;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Users.Members.Entity.DataModels
{
	[Table("UserMember")]
	public class Member<TMember,TMemberSource> : ObjectEntityBase
		where TMember: Member<TMember, TMemberSource>
		where TMemberSource: MemberSource<TMember, TMemberSource>
	{

		[Comment("电话")]
		[MaxLength(100)]
		[Index]
		public string PhoneNumber { get; set; }

		[Comment("图标")]
		[MaxLength(100)]
		public string Icon { get; set; }

		[Comment("来源渠道ID")]
		[ForeignKey(nameof(MemberSource))]
		public long? MemberSourceId { get; set; }
		public TMemberSource MemberSource { get; set; }

		[Comment("来源子渠道ID")]
		[ForeignKey(nameof(ChildMemberSource))]
		public long? ChildMemberSourceId { get; set; }
		public TMemberSource ChildMemberSource { get; set; }

		[Comment("邀请人ID")]
		[ForeignKey(nameof(Invitor))]
		public long? InvitorId { get; set; }

		public TMember Invitor { get; set; }
	}
}

