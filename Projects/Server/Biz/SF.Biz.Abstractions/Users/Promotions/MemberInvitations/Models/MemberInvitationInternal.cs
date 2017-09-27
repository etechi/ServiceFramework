using SF.Auth.Identities.Models;
using SF.Data;
using SF.Data.Models;
using SF.KB;
using SF.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Users.Promotions.MemberInvitations.Models
{
	[EntityObject(nameof(MemberInvitation))]
	public class MemberInvitationInternal : EventEntityBase
	{
		[Comment("被邀请人ID")]
		[EntityIdent(typeof(Identity),nameof(InviteeName))]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public override long Id { get; set; }

		[Comment("被邀请人")]
		[Hidden]
		[TableVisible]
		public string InviteeName { get; set; }

		[Comment("邀请人ID")]
		[EntityIdent(typeof(Identity), nameof(InvitorName))]
		public long InvitorId { get; }

		[Comment("邀请人")]
		[Hidden]
		[TableVisible]
		public string InvitorName { get; set; }

		[MaxLength(100000)]
		[Hidden]
		public string Invitors { get; set; }

	}
}

