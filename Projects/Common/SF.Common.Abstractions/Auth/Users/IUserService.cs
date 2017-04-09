using SF.Auth.Users.Models;
using SF.KB;
using SF.Metadata;
using System.Threading.Tasks;

namespace SF.Auth.Users
{
	public class SendSignupVerifyCodeArgument
	{
		public string IdentProviderId { get; set; }
		public string Ident { get; set; }
	}

	public class CreateAccessTokenArgument
	{
		public string Ident { get; set; }
		public string Password { get; set; }
		public int Expires { get; set; }
	}
	
	public class UserSignupArgument
	{
		public string Ident { get; set; }
		public string Password { get; set; }
		public string VerifyCode { get; set; }
		public string CaptchaCode { get; set; }
		public UserInfo UserInfo { get; set; }
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
		Task<string> CreateAccessToken(CreateAccessTokenArgument Arg);

		Task<string> Signup(UserSignupArgument Arg);
		Task<string> SendSignupVerifyCode(SendSignupVerifyCodeArgument Arg);

		Task<string> SendPasswordRecorveryCode(SendPasswordRecorveryCodeArgument Arg);
		Task<string> ResetPasswordByRecoveryCode(ResePasswordByRecorveryCodeArgument Arg);

		[Authorize]
		Task SetPassword(SetPasswordArgument Arg);

		[Authorize]
		Task Update(UserInfo Arg);
    }

}

