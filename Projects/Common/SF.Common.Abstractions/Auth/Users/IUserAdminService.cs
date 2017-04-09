using SF.Auth.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Auth.Users
{
	public class UserQueryArgument : Data.Entity.IQueryArgument<long>
	{
		public Option<long> Id { get; set; }
		public string NickName { get; set; }
	}
	public interface IUserAdminService : 
		Data.Entity.IEntitySource<long,UserInternal,UserQueryArgument>,
		Data.Entity.IEntityManager<long,UserEditable>
    {
    }

}

