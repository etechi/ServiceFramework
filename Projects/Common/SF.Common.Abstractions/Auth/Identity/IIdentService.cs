using SF.Auth.Identity.Models;
using SF.Core.ManagedServices.Models;
using SF.KB;
using SF.Metadata;
using System.Threading.Tasks;
using SF.Auth;

namespace SF.Auth.Identity
{
	public class SigninArgument
	{
		public string Ident { get; set; }
		public string Password { get; set; }
		public int? Expires { get; set; }
		public string CaptchaCode { get; set; }
		public bool ReturnToken { get; set; }
	}
	
	public class SetPasswordArgument
	{
		public string OldPassword { get; set; }
		public string NewPassword { get; set; }
		public bool ReturnToken { get; set; }
	}

	public class SendPasswordRecorveryCodeArgument
	{
		public string CaptchaCode { get; set; }
		public string Ident { get; set; }
	}
	public class ResetPasswordByRecorveryCodeArgument
	{
		public string Ident { get; set; }
		public string VerifyCode { get; set; }
		public string NewPassword { get; set; }
		public bool ReturnToken { get; set; }
	}
	public class CreateIdentArgument
	{
		public string Ident { get; set; }
		public string Password { get; set; }
		public string CaptchaCode { get; set; }
		public string VerifyCode { get; set; }
		public bool ReturnToken { get; set; }
		public int? Expires { get; set; }
	}

	public class SendCreateIdentVerifyCodeArgument
	{
		public string Ident { get; set; }
		public string CaptchaCode { get; set; }
	}

	[NetworkService]
	public interface IIdentService
    {
		[Authorize]
		Task<long?> GetCurUserId();

		Task<long?> ParseAccessToken(string AccessToken);

		Task<string> Signin(SigninArgument Arg);
		Task Signout();

		Task<string> SendPasswordRecorveryCode(SendPasswordRecorveryCodeArgument Arg);
		Task<string> ResetPasswordByRecoveryCode(ResetPasswordByRecorveryCodeArgument Arg);

		[Authorize]
		Task<string> SetPassword(SetPasswordArgument Arg);

		
		[Hidden]
		Task<string> SendCreateIdentVerifyCode(SendCreateIdentVerifyCodeArgument Arg);

		[Hidden]
		Task<string> CreateIdent(CreateIdentArgument Arg, bool VerifyCode);

	}

}

