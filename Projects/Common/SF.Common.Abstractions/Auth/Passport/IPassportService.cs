using SF.Auth.Passport.Models;
using SF.Metadata;
using System.Threading.Tasks;

namespace SF.Auth.Passport
{
	public class SigninArgument
	{
		public string Ident { get; set; }
		public string Password { get; set; }
		public int? Expires { get; set; }
	}

	
	[NetworkService]
	public interface IPassportService
    {
		Task<UserDesc> Signin(SigninArgument SigninArgument);
		Task Signout();

		Task<UserDesc> SigninByAccessToken(string AccessToken,int? Expires);

		[Authorize]
		Task<UserDesc> GetCurUser();

		[Authorize]
		Task<UserDesc> UpdateSession();
	}

}

