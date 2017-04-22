using SF.Auth.Identities.Models;
using SF.Core.ManagedServices.Models;
using SF.KB;
using SF.Metadata;
using System.Threading.Tasks;
using SF.Auth;

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
	}

	public class SendCreateIdentityVerifyCodeArgument
	{
		public string Ident { get; set; }
		public string CaptchaCode { get; set; }
	}

	[NetworkService]
	public interface IIdentityService
    {
		[Authorize]
		Task<long?> GetCurIdentityId();

		[Authorize]
		Task<Identity> GetCurIdentity();

		[Public]
		Task<string> Signin(SigninArgument Arg);

		[Public]
		Task Signout();

		[Public]
		Task<string> SendPasswordRecorveryCode(SendPasswordRecorveryCodeArgument Arg);

		[Public]
		Task<string> ResetPasswordByRecoveryCode(ResetPasswordByRecorveryCodeArgument Arg);

		[Authorize]
		Task<string> SetPassword(SetPasswordArgument Arg);

		Task UpdateIdentity(Identity Identity);

		Task<long?> ParseAccessToken(string AccessToken);

		Task<string> SendCreateIdentityVerifyCode(SendCreateIdentityVerifyCodeArgument Arg);

		Task<string> CreateIdentity(CreateIdentityArgument Arg, bool VerifyCode);

		Task<Identity> GetIdentity(long Id);
	}

}

