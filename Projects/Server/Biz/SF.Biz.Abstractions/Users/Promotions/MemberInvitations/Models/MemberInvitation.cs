using SF.Data;
using SF.Data.Models;
using SF.KB;
using SF.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SF.Users.Promotions.MemberInvitations.Models
{
	[EntityObject]
	public class MemberInvitation : EventEntityBase
	{
		[Comment("被邀请人ID")]
		public override long Id { get; set; }
	}
}

