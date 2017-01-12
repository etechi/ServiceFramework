using ServiceProtocol;
using SF.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Auth.Users
{
    public class SigninArgument
	{
		public string Ident { get; set; }
		public string Password { get; set; }
	}
	[NetworkService]
	public interface ISigninService
    {
		Task<UserInfo> Signin(SigninArgument Arg);
		Task Signout();
	}
}

