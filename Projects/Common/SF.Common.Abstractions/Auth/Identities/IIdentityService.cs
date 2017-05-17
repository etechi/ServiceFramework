using SF.Auth.Identities.Models;
using System.Threading.Tasks;

namespace SF.Auth.Identities
{
	public class SigninArgument
	{
		public string Credential { get; set; }
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
		public string Credential { get; set; }
	}
	public class ResetPasswordByRecorveryCodeArgument
	{
		public string Credential { get; set; }
		public string VerifyCode { get; set; }
		public string NewPassword { get; set; }
		public bool ReturnToken { get; set; }
	}
	public class CreateIdentityArgument
	{
		public Identity Identity { get; set; }
		public string Credential { get; set; }
		public string Password { get; set; }
		public string CaptchaCode { get; set; }
		public string VerifyCode { get; set; }
		public bool ReturnToken { get; set; }
		public int? Expires { get; set; }
		public IIdentityCredentialProvider CredentialProvider { get; set; }
	}

	public class SendCreateIdentityVerifyCodeArgument
	{
		public string Credetial { get; set; }
		public string CaptchaCode { get; set; }
	}

	public interface IPassportService
	{
		Task<string> Signin(SigninArgument Arg);

		Task Signout();

		Task<string> SendPasswordRecorveryCode(SendPasswordRecorveryCodeArgument Arg);

		Task<string> ResetPasswordByRecoveryCode(ResetPasswordByRecorveryCodeArgument Arg);

		[Authorize]
		Task<string> SetPassword(SetPasswordArgument Arg);
	}
	public interface IIdentityService
    {
		Task<long?> GetCurIdentityId();

		Task<Identity> GetCurIdentity();



		Task<string> Signin(SigninArgument Arg, IIdentityCredentialProvider[] CredentialProvider);

		Task Signout();

		Task<string> SendPasswordRecorveryCode(SendPasswordRecorveryCodeArgument Arg, IIdentityCredentialProvider CredentialProvider);

		Task<string> ResetPasswordByRecoveryCode(ResetPasswordByRecorveryCodeArgument Arg);

		Task<string> SetPassword(SetPasswordArgument Arg);



		Task UpdateIdentity(Identity Identity);

		Task<long?> ParseAccessToken(string AccessToken);

		Task<string> SendCreateIdentityVerifyCode(SendCreateIdentityVerifyCodeArgument Arg, IIdentityCredentialProvider CredentialProvider);

		Task<string> CreateIdentity(CreateIdentityArgument Arg, bool VerifyCode, IIdentityCredentialProvider CredentialProvider);

		Task<Identity> GetIdentity(long Id);
	}

}

