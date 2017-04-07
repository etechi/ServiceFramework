using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Auth.Users
{
	public class UserSessionQueryArgument : Data.Entity.IQueryArgument<long>
	{
		public Option<long> Id { get; set; }
		public string NickName { get; set; }
	}
	public interface IUserSessionAdminService : 
		Data.Entity.IEntitySource<long,UserSessionInternal,UserSessionQueryArgument>
    {
    }

}

