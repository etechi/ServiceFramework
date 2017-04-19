using SF.System.Auth.Identity.Models;
using SF.Core.ManagedServices.Models;
using SF.KB;
using SF.Metadata;
using System.Threading.Tasks;
using SF.Auth;

namespace SF.System.Auth.Identity
{
	public class CreateIdentArgument
	{
		public int ScopeId { get; set; }
		public string Ident { get; set; }
		public string Password { get; set; }
		public string CaptchaCode { get; set; }
		public string VerifyCode { get; set; }
	}


	public class SendCreateIdentVerifyCodeArgument
	{
		public int ScopeId { get; set; }
		public string IdentProviderId { get; set; }
		public string Ident { get; set; }
		public string CaptchaCode { get; set; }
	}
	public interface IIdentAdminService
    {
		Task<string> SendCreateIdentVerifyCode(SendCreateIdentVerifyCodeArgument Arg);

		Task<string> CreateIdent(CreateIdentArgument Arg, bool VerifyCode);

		Task<ServiceInstance[]> GetBindProviders();
		IIdentBindProvider GetBindProvider(string ProviderId);
	}

}

