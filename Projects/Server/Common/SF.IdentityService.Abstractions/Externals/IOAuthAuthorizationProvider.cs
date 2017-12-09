using System.Threading.Tasks;

namespace SF.Auth.IdentityServices.Externals
{
	public interface IOAuthAuthorizationProvider : IExternalAuthorizationProvider
	{
		Task<string> RefreshToken(string Token);
	}


}
