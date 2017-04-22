using SF.Auth;
using SF.Auth.Identities.Models;
using SF.Data;
using SF.Metadata;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SF.Users.Members.Models
{
	[EntityObject("会员")]
	public class MemberDesc : IObjectWithId<long>
	{
		[Comment("ID")]
		[ReadOnly(true)]
		public long Id { get; set; }

		[Comment("名称")]
		[MaxLength(100)]
		public string Name { get; set; }
	}

}

