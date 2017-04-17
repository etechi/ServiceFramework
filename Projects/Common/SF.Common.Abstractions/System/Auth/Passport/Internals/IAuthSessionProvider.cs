using System.Threading.Tasks;
using SF.System.Auth.Passport.Models;
namespace SF.System.Auth.Passport.Internals
{
	public interface IAuthSessionProvider
    {
		Task<UserSession> GetUserSession();
		Task BindSession(UserSession Session);
		Task UnbindSession();
	}
}

