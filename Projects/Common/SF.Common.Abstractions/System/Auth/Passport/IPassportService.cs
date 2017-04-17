using SF.System.Auth.Identity.Models;
using SF.System.Auth.Passport.Models;
using SF.Metadata;
using System.Threading.Tasks;
using SF.Auth;

namespace SF.System.Auth.Passport
{
	public class SigninArgument
	{
		public int ScopeId { get; set; }
		public string Ident { get; set; }
		public string Password { get; set; }
		public int? Expires { get; set; }
	}


	public class SendPasswordRecorveryCodeArgument
	{
		public int ScopeId { get; set; }
		public string CaptchaCode { get; set; }
		public string IdentProviderId { get; set; }
		public string Ident { get; set; }
	}
	public class ResePasswordByRecorveryCodeArgument
	{
		public int ScopeId { get; set; }
		public string IdentProviderId { get; set; }
		public string Ident { get; set; }
		public string Code { get; set; }
		public string NewPassword { get; set; }
	}

	
	public class SetPasswordArgument
	{
		public string OldPassword { get; set; }
		public string NewPassword { get; set; }
	}

	public interface IPassportService
	{
		Task<string> SendPasswordRecorveryCode(SendPasswordRecorveryCodeArgument Arg);
		Task<string> ResetPasswordByRecoveryCode(ResePasswordByRecorveryCodeArgument Arg);

		Task SetPassword(SetPasswordArgument Arg);

		Task Update(IdentDesc Desc);

		Task<IdentDesc> Signin(SigninArgument SigninArgument);
		Task Signout();

		Task<IdentDesc> SigninByAccessToken(string AccessToken,int? Expires);

		[Authorize]
		Task<IdentDesc> GetCurIdent(int ScopeId);

		[Authorize]
		Task<IdentDesc> UpdateSession();
	}

}

