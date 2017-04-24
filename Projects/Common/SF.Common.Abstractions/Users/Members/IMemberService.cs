using SF.Metadata;
using SF.Auth;
using SF.Auth.Identities;
using SF.Users.Members.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Users.Members
{
	public class MemberSignupArgument:CreateIdentityArgument
	{
	}
	public class SendSignupVerifyCodeArgument: 
		SendCreateIdentityVerifyCodeArgument
	{
	}
	
	[NetworkService]
	public interface IMemberService : IPassportService
	{
		Task<string> SendSignupVerifyCode(SendSignupVerifyCodeArgument Arg);
		Task<string> Signup(MemberSignupArgument Arg);
	}

}

