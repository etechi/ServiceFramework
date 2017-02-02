using System.Threading.Tasks;

namespace SF.Auth.Users
{
	public interface IAuthSessionProvider
    {
		Task<long?> GetCurrentUserId();
		Task BindUser(UserInfo user);
		Task UnbindUser();
	}
}

