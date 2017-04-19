using SF.System.Auth.Identity.Models;
using SF.Core.ManagedServices.Models;
using SF.KB;
using SF.Metadata;
using System.Threading.Tasks;
using SF.Auth;

namespace SF.System.Auth.Identity
{
	
	public class CreateAccessTokenArgument
	{
		public int ScopeId { get; set; }
		public string Ident { get; set; }
		public string Password { get; set; }
		public int? Expires { get; set; }
		public string Code { get; set; }
	}
	
	public class SetPasswordArgument
	{
		public string OldPassword { get; set; }
		public string NewPassword { get; set; }
	}
	
	public class SendPasswordRecorveryCodeArgument
	{
		public int ScopeId { get; set; }
		public string CaptchaCode { get; set; }
		public string IdentProviderId { get; set; }
		public string IdentValue { get; set; }
	}
	public class ResetPasswordByRecorveryCodeArgument
	{
		public int ScopeId { get; set; }
		public string IdentProviderId { get; set; }
		public string IdentValue { get; set; }
		public string Code { get; set; }
		public string NewPassword { get; set; }
	}
	[NetworkService]
	public interface IIdentService
    {
		Task<string> CreateAccessToken(CreateAccessTokenArgument Arg);
		Task<long> ParseAccessToken(string AccessToken);

		Task<string> SendPasswordRecorveryCode(SendPasswordRecorveryCodeArgument Arg);
		Task<string> ResetPasswordByRecoveryCode(ResetPasswordByRecorveryCodeArgument Arg);

		[Authorize]
		Task SetPassword(SetPasswordArgument Arg);

	}

}

