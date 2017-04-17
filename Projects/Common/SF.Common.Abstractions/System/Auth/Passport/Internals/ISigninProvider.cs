using SF.System.Auth.Passport.Models;
using SF.Metadata;
using System.Threading.Tasks;
using SF.System.Auth.Identity.Models;

namespace SF.System.Auth.Passport.Internals
{
	public class CreateAccessTokenArgument
	{
		public string Ident { get; set; }
		public string Password { get; set; }
	}


	public interface ISigninProvider
    {
		Task<string> CreateAccessToken(CreateAccessTokenArgument Argument);
		Task<long> ParseAccessToken(string AccessToken);
		Task<IdentDesc> GetIdentDesc(long UserId);
    }

}

