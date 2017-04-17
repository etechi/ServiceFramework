using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.System.Auth.Passport.Models;

namespace SF.System.Auth.Passport
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

