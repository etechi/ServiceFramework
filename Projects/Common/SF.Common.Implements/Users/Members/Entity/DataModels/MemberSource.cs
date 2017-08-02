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

