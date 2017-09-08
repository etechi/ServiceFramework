using SF.Auth;
using SF.Auth.Identities;
using SF.Auth.Identities.Models;
using SF.Metadata;
using SF.Users.Promotions.MemberSources.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Users.Promotions.MemberSources
{
	public class MemberSourceQueryArgument : Entities.IQueryArgument<long>
	{
		[Comment("Id")]
		public Option<long> Id { get; set; }

		[Comment("名称")]
		public string Name { get; set; }
	}

	
	[EntityManager]
	[Authorize("admin")]
	[NetworkService]
	[Comment("会员渠道")]
	[Category("用户管理", "会员渠道管理")]
	public interface IMemberSourceManagementService : 
		Entities.IEntitySource<long,MemberSourceInternal,MemberSourceQueryArgument>,
		Entities.IEntityManager<long,MemberSourceInternal>
    {
		Task AddSourceMember(long SourceId, long MemberId);
	}

}

