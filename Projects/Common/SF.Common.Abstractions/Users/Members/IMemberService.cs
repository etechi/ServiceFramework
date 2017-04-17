using SF.Metadata;
using SF.System.Auth.Identity;
using SF.System.Auth.Passport;
using SF.Users.Members.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Users.Members
{
	public class MemberSignupArgument
	{
		public int ScopeId { get; set; }
		public string Ident { get; set; }
		public string Password { get; set; }
		public string VerifyCode { get; set; }
		public MemberDesc MemberInfo { get; set; }
		public string CaptchaCode { get; set; }
	}
	public class SendSignupVerifyCodeArgument
	{
		public int ScopeId { get; set; }
		public string IdentProviderId { get; set; }
		public string Ident { get; set; }
		public string CaptchaCode { get; set; }
	}
	[NetworkService]
	public interface IMemberService : IPassportService
	{
		Task<string> SendSignupVerifyCode(SendSignupVerifyCodeArgument Arg);
		Task<MemberDesc> Signup(MemberSignupArgument Arg);
	}

}

