using SF.Metadata;
using SF.System.Auth;
using SF.System.Auth.Identity;
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
	[NetworkService]
	public interface IMemberService : IUserService
	{
		Task<string> SendSignupVerifyCode(SendSignupVerifyCodeArgument Arg);
		Task<MemberDesc> Signup(MemberSignupArgument Arg);
	}

}

