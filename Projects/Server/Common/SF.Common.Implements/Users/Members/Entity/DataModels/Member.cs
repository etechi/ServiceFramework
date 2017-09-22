using SF.Data;
using SF.Entities.DataModels;
using SF.Metadata;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Users.Members.Entity.DataModels
{
	[Table("UserMember")]
	public class Member<TMember> : ObjectEntityBase
		where TMember: Member<TMember>
	{

		[Comment("电话")]
		[MaxLength(100)]
		[Index]
		public string PhoneNumber { get; set; }

		[Comment("图标")]
		[MaxLength(100)]
		public string Icon { get; set; }

		[Comment("注册用户描述")]
		public long SignupIdentityId { get; set; }
	}
}

