using SF.System.Auth.Identity.Models;
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
		public Option<long> Id { get; set; }
		public string NickName { get; set; }
	}
	public interface IMemberManagementService : 
		Data.Entity.IEntitySource<long,MemberInternal,MemberQueryArgument>,
		Data.Entity.IEntityManager<long,MemberEditable>
    {
    }

}

