using SF.Auth.Identities;
using SF.Auth.Identities.Models;
using SF.Metadata;
using SF.Users.Members.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Users.Members
{
	public class MemberQueryArgument : Data.Entity.IQueryArgument<long>
	{
		[Comment("Id")]
		public Option<long> Id { get; set; }

		[Comment("名称")]
		public string Name { get; set; }

		[Comment("电话")]
		public string PhoneNumber { get; set; }

		[Comment("来源")]
		[EntityIdent("会员来源")]
		public long? MemberSourceId { get; set; }

		[Comment("邀请人")]
		[EntityIdent("会员")]
		public long? InvitorId { get; set; }

	}
	public interface IMemberManagementService : 
		Data.Entity.IEntitySource<long,MemberInternal,MemberQueryArgument>,
		Data.Entity.IEntityManager<long,MemberEditable>
    {
		Task<string> CreateMemberAsync(
			CreateIdentityArgument Arg
			);
	}

}

