using System.Threading.Tasks;

namespace SF.Auth.Users
{
	public interface IAuthSessionProvider
    {
		Task<long?> GetCurrentUserId();
		Task<long?> GetCurrentSessionId();
		Task BindSession(UserSession Session);
		Task UnbindSession();
	}
}

