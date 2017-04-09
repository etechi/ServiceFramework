using System.Threading.Tasks;
using SF.Auth.Passport.Models;
namespace SF.Auth.Passport.Internals
{
	public interface IAuthSessionProvider
    {
		Task<UserSession> GetUserSession();
		Task BindSession(UserSession Session);
		Task UnbindSession();
	}
}

