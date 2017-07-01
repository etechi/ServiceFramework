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
	

	public class SendPasswordRecorveryCodeArgument
	{
		public long? CredentialProviderId { get; set; }
		public string CaptchaCode { get; set; }
		public string Credential { get; set; }
	}
	public class ResetPasswordByRecorveryCodeArgument
	{
		public long? CredentialProviderId { get; set; }
		public string Credential { get; set; }
		public string VerifyCode { get; set; }
		public string NewPassword { get; set; }
		public bool ReturnToken { get; set; }
	}
	public class CreateIdentityArgument
	{
		public long? CredentialProviderId { get; set; }
		public Identity Identity { get; set; }
		public string Credential { get; set; }
		public string Password { get; set; }
		public string CaptchaCode { get; set; }
		public string VerifyCode { get; set; }
		public bool ReturnToken { get; set; }
		public int? Expires { get; set; }
	}

	public class SendCreateIdentityVerifyCodeArgument
	{
		public long? CredentialProviderId { get; set; }
		public string Credetial { get; set; }
		public string CaptchaCode { get; set; }
	}

	public class SetPasswordArgument
	{
		public string OldPassword { get; set; }
		public string NewPassword { get; set; }
		public bool ReturnToken { get; set; }
	}


	public interface IIdentityService
    {
		Task<long?> GetCurIdentityId();

		Task<Identity> GetCurIdentity();


		Task<string> Signin(SigninArgument Arg);

		Task Signout();

		Task<string> SendPasswordRecorveryCode(SendPasswordRecorveryCodeArgument Arg);

		Task<string> ResetPasswordByRecoveryCode(ResetPasswordByRecorveryCodeArgument Arg);

		Task<string> SetPassword(SetPasswordArgument Arg);


		Task UpdateIdentity(Identity Identity);

		Task<long?> ParseAccessToken(string AccessToken);

		Task<string> SendCreateIdentityVerifyCode(SendCreateIdentityVerifyCodeArgument Arg);

		Task<string> CreateIdentity(CreateIdentityArgument Arg, bool VerifyCode);

		Task<Identity> GetIdentity(long Id);
	}

}

