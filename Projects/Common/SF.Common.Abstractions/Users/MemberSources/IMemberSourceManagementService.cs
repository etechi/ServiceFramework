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

namespace SF.Users.MemberSources
{
	public class MemberSourceQueryArgument : Entities.IQueryArgument<long>
	{
		[Comment("Id")]
		public Option<long> Id { get; set; }


	}

	[EntityManager]
	[Authorize("admin")]
	[NetworkService]
	[Comment("会员来源")]
	[Category("用户管理", "会员来源管理")]
	public interface IMemberSourceManagementService : 
		Entities.IEntitySource<long,Models.MemberSourceInternal, MemberSourceQueryArgument>,
		Entities.IEntityManager<long, Models.MemberSourceEditable>
    {
		
	}

}

