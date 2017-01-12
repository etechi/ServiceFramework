using ServiceProtocol;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Auth.Users
{
	public class UserCreateArgument
	{
		public UserInfo User { get; set; }
		public string Password { get; set; }
		public UserIdent[] Idents { get; set; }
	}
	public interface IAuthUserFactory
    {
		Task<UserInfo> Create(UserCreateArgument Arg);
	}

}

