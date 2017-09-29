using SF.Auth;
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
	public class MemberQueryArgument : Entities.IQueryArgument<long>
	{
		[Comment("Id")]
		public Option<long> Id { get; set; }

		[Comment("名称")]
		public string Name { get; set; }

		[Comment("电话")]
		public string PhoneNumber { get; set; }

		//[Comment("来源")]
		//[EntityIdent(typeof(MemberSources.IMemberSourceManagementService))]
		//public long? MemberSourceId { get; set; }

		//[Comment("邀请人")]
		//[EntityIdent(typeof(IMemberManagementService))]
		//public long? InvitorId { get; set; }

	}

	public class CreateMemberArgument : CreateIdentityArgument
	{
		public long? MemberSourceId { get; set; }

		public long? InvitorId { get; set; }
	}
	[EntityManager("会员")]
	[Authorize("admin")]
	[NetworkService]
	[Comment("会员")]
	[Category("用户管理", "会员管理")]
	public interface IMemberManagementService : 
		Entities.IEntitySource<MemberInternal,MemberQueryArgument>,
		Entities.IEntityManager<MemberEditable>
    {
		Task<string> CreateMemberAsync(
			CreateMemberArgument Arg,
			IIdentityCredentialProvider CredentialProvider
			);
	}

}

