using SF.Metadata;
using SF.Auth;
using SF.Auth.Identity;
using SF.Users.Members.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Users.Members
{
	public class MemberSignupArgument:CreateIdentArgument
	{
		public MemberDesc Desc { get; set; }
		public int? Expires { get; set; }
	}
	public class SendSignupVerifyCodeArgument: 
		SendCreateIdentVerifyCodeArgument
	{
	}
	public class SignupResult
	{
		public MemberDesc Desc { get; set; }
		public string Token { get; set; }
	}
	[NetworkService]
	public interface IMemberService : IUserService
	{
		Task<string> SendSignupVerifyCode(SendSignupVerifyCodeArgument Arg);
		Task<SignupResult> Signup(MemberSignupArgument Arg);
	}

}

