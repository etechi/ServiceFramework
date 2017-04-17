using SF.System.Auth.Identity.Models;
using SF.Core.ManagedServices.Models;
using SF.KB;
using SF.Metadata;
using System.Threading.Tasks;

namespace SF.System.Auth.Identity
{
	public class CreateIdentArgument
	{
		public string Ident { get; set; }
		public string Password { get; set; }
		public IdentDesc Desc { get; set; }
	}

	public class CreateAccessTokenArgument
	{
		public string Ident { get; set; }
		public string Password { get; set; }
		public int Expires { get; set; }
	}
	
	public class SetPasswordArgument
	{
		public long UserId { get; set; }
		public string OldPassword { get; set; }
		public string NewPassword { get; set; }
	}
	
	public interface IIdentService
    {
		Task<string> CreateAccessToken(CreateAccessTokenArgument Arg);
		Task<long> ParseAccessToken(string AccessToken);

		Task<string> Create(CreateIdentArgument Arg);

		Task SetPassword(SetPasswordArgument Arg);

		Task Update(IdentDesc Arg);

		Task<ServiceInstance[]> GetBindProviders();
		IIdentBindProvider GetBindProvider(string ProviderId);
	}

}

