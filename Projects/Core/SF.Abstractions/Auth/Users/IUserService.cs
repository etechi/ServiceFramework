using SF.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Auth.Users
{
	public class SendSignupVerifyCodeArgument
	{
		public string IdentProviderId { get; set; }
		public string Ident { get; set; }
	}

	public class UserSigninArgument
	{
		public string Ident { get; set; }
		public string Password { get; set; }
	}

	public class UserSignupArgument
	{
		public string Icon { get; set; }
		public string Image { get; set; }
		public SexType Sex { get; set; }
		public string Ident { get; set; }
		public string NickName { get; set; }
		public string Password { get; set; }
		public string VerifyCode { get; set; }
		public string CaptchaCode { get; set; }
	}
	public class SetPasswordArgument
	{
		public string OldPassword { get; set; }
		public string NewPassword { get; set; }
	}
	public class SendPasswordRecorveryCodeArgument
	{
		public string IdentProviderId { get; set; }
		public string Ident { get; set; }
	}
	public class ResePasswordByRecorveryCodeArgument
	{
		public string IdentProviderId { get; set; }
		public string Ident { get; set; }
		public string Code { get; set; }
		public string NewPassword { get; set; }
	}


	[NetworkService]
	public interface IUserService
    {
		Task<UserInfo> Signin(UserSigninArgument Arg);
		Task Signout();

		Task<UserInfo> Signup(UserSignupArgument Arg);
		Task<string> SendSignupVerifyCode(SendSignupVerifyCodeArgument Arg);

		Task<string> SendPasswordRecorveryCode(SendPasswordRecorveryCodeArgument Arg);
		Task ResetPasswordByRecoveryCode(ResePasswordByRecorveryCodeArgument Arg);


		[Authorize]
		Task SetPassword(SetPasswordArgument Arg);

		[Authorize]
		Task<UserInfo> GetCurUser();

		[Authorize]
		Task Update(UserInfo User);
    }

}

