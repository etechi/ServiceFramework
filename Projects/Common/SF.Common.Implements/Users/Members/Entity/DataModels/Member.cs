using SF.Metadata;
using SF.Auth;
using SF.Auth.Identities;
using SF.Users.Members.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Auth.Identities.Models;
using SF.Data.Entity;
using SF.Data.Storage;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data;
using System.ComponentModel.DataAnnotations;
using SF.Data.DataModels;

namespace SF.Users.Members.Entity.DataModels
{
	[Table("UserMember")]
	public class Member<TMember,TMemberSource> : DataEntityBase
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

