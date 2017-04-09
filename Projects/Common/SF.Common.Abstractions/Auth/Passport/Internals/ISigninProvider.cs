using SF.Auth.Passport.Models;
using SF.Metadata;
using System.Threading.Tasks;

namespace SF.Auth.Passport.Internals
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
		Task<UserDesc> GetUserDesc(long UserId);
    }

}

